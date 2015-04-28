using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public class Sprite : LogicObject, IDrawableRemovable, IWithPosition, IMoveableWithPosition, ICollidable, ICollisionResponder 
    {
        private RenderOptions mRenderOptions = new RenderOptions();

        private Dictionary<int, SpriteAnimation> mAnimations = new Dictionary<int, SpriteAnimation>();

        private RGPointI mLocation;
        public RGPointI Location
        {
            get
            {
                return mLocation;
            }
            set
            {
                mLocation = value;
            }
        }

        public RGPoint CurrentSpeed { get { return MotionManager.MotionOffset; } }

        /// <summary>
        /// The sprite's speed at the beginning of the frame
        /// </summary>
        public RGPoint OriginalSpeed { get; private set; }

        public Direction Direction { get; set; }

        public RGRectangleI Area
        {
            get
            {
                return CurrentAnimation.CollisionRec;
            }
        }

        public RGRectangleI SecondaryCollisionArea
        {
            get
            {
                return CurrentAnimation.SecondaryCollisionRec;
            }
        }

        public LayerDepth LayerDepth { get; private set; }

        public int X { get { return this.Location.X; } }
        public int Y { get { return this.Location.Y; } }

        private int mCurrentAnimationKey = 0;   
        public int CurrentAnimationKey
        {
            get { return mCurrentAnimationKey; }
            set
            {
                if (mCurrentAnimationKey == value)
                    return;

                if (!mAnimations.ContainsKey(value))
                    return;

                mCurrentAnimationKey = value;
                CurrentAnimation.Reset();
            }
        }

        public bool SetAnimation(int key)
        {
            this.CurrentAnimationKey = key;
            return this.CurrentAnimationKey == key;
        }

        public GeneralLogicObject BehaviorState { get; private set; }
      
        public SpriteAnimation CurrentAnimation { get { return mAnimations[mCurrentAnimationKey]; } }

        public ObjectType ObjectType { get; private set; }

        public ObjectType[] mOriginalCollisionTypes { get; set; }

        private ObjectType[] mCollisionTypes;
        public ObjectType[] CollisionTypes
        {
            get { return mCollisionTypes; }
            set
            {
                mCollisionTypes = value;
                mOriginalCollisionTypes = value;
            }
        }

        public Layer DrawLayer { get; private set; }

        public void RemoveCollisionType(ObjectType i)
        {
            mCollisionTypes = CollisionTypes.Where(p => p.IsNot(i)).ToArray();
        }

        public void ReAddCollisionType(ObjectType i)
        {
            if(CollisionTypes.Any(p=> p.Equals(i)))
                return;

            if(!mOriginalCollisionTypes.Any(p=> p.Equals(i)))
                return;

            var list = CollisionTypes.ToList();
            list.Add(i);
            mCollisionTypes = list.ToArray();
        }

        public Sprite(GameContext ctx, Layer drawLayer, ObjectType type)
            : base(LogicPriority.Behavior, drawLayer, RelationFlags.DestroyWhenParentDestroyed | RelationFlags.PauseWhenParentPaused)
        {
            this.Location = RGPointI.Empty;
            this.LayerDepth = drawLayer.Depth;
            this.ObjectType = type;

            this.DrawLayer = drawLayer;
            this.BehaviorState = new GeneralLogicObject(this);
        }

        //public Sprite(GameContext ctx, ObjectType type)
        //    : base(LogicPriority.Behavior, ctx)
        //{
        //    this.Location = RGPoint.Empty;
        //    this.LayerDepth = LayerDepth.Screen;
        //    this.ObjectType = type;
        //}

        public SpriteAnimation AddAnimation(int key, Animation a)
        {
            var anim = new SpriteAnimation(this, a, this.mRenderOptions );
            mAnimations.Add(key, anim);

            if (this.mCurrentAnimationKey == 0)
                mCurrentAnimationKey = key;

            return anim;
        }

        public void SetSingleAnimation(Animation a)
        {
            mAnimations.Add(0, new SpriteAnimation(this, a, this.mRenderOptions));
            this.CurrentAnimationKey = 0;
        }

        public void AddAnimation(int key, GameResource<Animation> resource)
        {
            if (mCurrentAnimationKey == 0)
                mCurrentAnimationKey = key;

            mAnimations.Add(key, new SpriteAnimation(this, resource.GetObject(this.Context), this.mRenderOptions));
        }

        public SpriteAnimation GetAnimation(int key)
        {
            return mAnimations[key];
        }    

        protected override void Update()
        {
            OriginalSpeed = this.CurrentSpeed;
        }
    
        public void Draw(Engine.Graphics.Painter painter, RGRectangleI canvas)
        {
            painter.Paint(canvas, this.CurrentAnimation);
        }

        private ObjectMotion mMotionManager;
        public ObjectMotion MotionManager
        {
            get { return mMotionManager ?? (mMotionManager = ObjectMotion.Create(this)); }
        }


        private List<ICollisionResponder> mCollisionResponders;
        public ICollection<ICollisionResponder> CollisionResponders
        {
            get
            {
                if (mCollisionResponders == null)
                {
                    mCollisionResponders = new List<ICollisionResponder>();
                    mCollisionResponders.Add(this);
                }

                return mCollisionResponders;
            }
        }

        public void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            if (!this.Started)
            {
                response.ShouldContinueHandling = false;
                response.ShouldBlock = false;
                return;
            }

            //foreach (var behavior in mBehaviors)
            //{
            //    behavior.HandleCollision(collision,response);
            //    if (!response.ShouldContinueHandling)
            //        return;
            //}
        }

        public void BeforeCollision(CollisionEvent collision)
        {
        }

        public void Move(RGPointI offset)
        {
            this.Location = this.Location.Offset(offset);
        }

        public override string ToString()
        {
            return this.ObjectType.ToString() + ":" + this.CurrentAnimation.Texture.Path;
        }





    }

}
