using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public class Sprite : LogicObject, IDrawableRemovable, IWithPosition, IMoveable, ICollidable
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

        private List<SpriteBehavior> mBehaviors = new List<SpriteBehavior>();

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
            : base(LogicPriority.Behavior, ctx)
        {
            this.Location = RGPointI.Empty;
            this.LayerDepth = drawLayer.Depth;
            this.ObjectType = type;

            this.DrawLayer = drawLayer;
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

        public T AddBehavior<T>(T b) where T:SpriteBehavior 
        {
            if (!mBehaviors.Contains(b))
                mBehaviors.Add(b);

            return b;
        }

        public void AddBehaviorChain(params SpriteBehavior[] behaviors)
        {
            foreach (var behavior in behaviors)
                AddBehavior(behavior);

            AddBehavior(new BehaviorChain(this, behaviors));

        }

        #region Behaviors

        public void ClearBehaviors()
        {
            foreach (var behavior in mBehaviors)
                behavior.Kill(ExitCode.Removed);
        }

        public void ClearBehaviorsExcept(params SpriteBehavior[] exclusions)
        {
            foreach (var behavior in mBehaviors)
            {
                if (!exclusions.Contains(behavior))
                    behavior.Kill(ExitCode.Removed);
            }
        }

        public T GetBehavior<T>()
        {
            return mBehaviors.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetBehaviors<T>()
        {
            return mBehaviors.OfType<T>();
        }


        #endregion

        protected override void OnEntrance()
        {
            //this.DrawLayer.AddObject(new DebugRectangle<Sprite>(this, s => s.SecondaryCollisionArea, DebugRectangle.RecColor.Blue));          
            //this.DrawLayer.AddObject(new DebugRectangle<Sprite>(this, s => s.Area, DebugRectangle.RecColor.Orange));

            //this.DrawLayer.AddObject(new DebugRectangle<Sprite>(this, s => RGRectangle.FromXYWH(s.X - 2, s.Y - 2, 4f, 4f), DebugRectangle.RecColor.Green));
        }

        protected override void Update()
        {
            OriginalSpeed = this.CurrentSpeed;
            mBehaviors.RemoveAll(p => !p.Alive);
        }
    

        public void Draw(Engine.Graphics.Painter painter, RGRectangleI canvas)
        {
            painter.Paint(canvas, this.CurrentAnimation);
        }

        private ObjectMotion mMotionManager;
        public ObjectMotion MotionManager
        {
            get { return mMotionManager ?? (mMotionManager = new ObjectMotion(this.Context, this)); }
        }


        public void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            if (!this.Started)
            {
                response.ShouldContinueHandling = false;
                response.ShouldBlock = false;
                return;
            }

            foreach (var behavior in mBehaviors)
            {
                behavior.HandleCollision(collision,response);
                if (!response.ShouldContinueHandling)
                    return;
            }
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
