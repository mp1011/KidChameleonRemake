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

        private Lockable<Direction> mDirection = new Lockable<Direction>(Direction.Right);

        public Direction Direction
        {
            get { return mDirection.Value; }
            set
            {
                mDirection.SetValue(value);
            }
        }

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

        private Lockable<int> mCurrentAnimationKey = new Lockable<int>(0);   
        
        public int CurrentAnimationKey
        {
            get { return mCurrentAnimationKey.Value; }
        }

        public bool SetAnimation(int key)
        {
            return SetAnimation(key, null,false);
        }

        public bool SetAnimation(int key, ILogicObject owner, bool claimLock)
        {
            if (claimLock)
                owner.ClaimLock(mCurrentAnimationKey);

            if (mCurrentAnimationKey.Value == key)
                return false;

            if (!mAnimations.ContainsKey(key))
                return false;

            if (!mCurrentAnimationKey.SetValue(key, owner))
                return false;

            CurrentAnimation.Reset();

            return this.CurrentAnimationKey == key;
        }

        public void ReleaseAnimationKeyLock(ILogicObject owner)
        {
            mCurrentAnimationKey.ClearOwner(owner);
        }

        public void LockDirection(Direction d, ILogicObject lockOwner)
        {
            this.ClaimLock(mDirection);
            mDirection.SetValue(d, lockOwner);
        }

        public void UnlockDirection(ILogicObject lockOwner)
        {
            this.ReleaseLock(mDirection);
        }

        public GeneralLogicObject BehaviorState { get; private set; }
      
        public SpriteAnimation CurrentAnimation { get { return mAnimations[mCurrentAnimationKey.Value]; } }

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
            mCollisionTypes = mCollisionTypes.NeverNull().Where(p => p.IsNot(i)).ToArray();
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

        public Sprite(Layer drawLayer, ObjectType type)
            : this(drawLayer.Context, drawLayer, type) { }
  
        public Sprite(GameContext ctx, Layer drawLayer, ObjectType type)
            : base(LogicPriority.Behavior, drawLayer, RelationFlags.DestroyWhenParentDestroyed | RelationFlags.PauseWhenParentPaused)
        {
            this.Location = RGPointI.Empty;
            this.LayerDepth = drawLayer.Depth;
            this.ObjectType = type;

            this.DrawLayer = drawLayer;
            this.BehaviorState = new GeneralLogicObject(this);

            if(Context.Listeners != null)
                Context.Listeners.SpriteListener.Register(this);
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

            if (this.mCurrentAnimationKey.Value == 0)
                mCurrentAnimationKey.SetValue(key);

            return anim;
        }

        public void SetSingleAnimation(Animation a)
        {
            mAnimations.Add(0, new SpriteAnimation(this, a, this.mRenderOptions));
            this.SetAnimation(0);
        }

        public void AddAnimation(int key, GameResource<Animation> resource)
        {
            if (mCurrentAnimationKey.Value == 0)
                mCurrentAnimationKey.SetValue(key);

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

        /// <summary>
        /// Creates a copy of this sprite and adds it to the same layer at the same location.
        /// 
        /// Does not copy behaviors
        /// </summary>
        /// <returns></returns>
        public Sprite Copy(bool copyAnimations, bool copyBehaviors)
        {
            var newSprite = new Sprite(this.Context, this.DrawLayer, this.ObjectType);

            if(copyAnimations)
                newSprite.mAnimations = this.mAnimations;

            if (copyBehaviors)
                throw new NotImplementedException();

            this.DrawLayer.AddObject(newSprite);

            newSprite.Location = this.Location;
            newSprite.Direction = this.Direction;
            newSprite.SetAnimation(this.CurrentAnimationKey);

            return newSprite;
        }


        #region Helpers

        public Sprite SetSingleAnimation(GameResource<SpriteSheet> spriteSheetResource, int frame)
        {
            var spriteSheet = spriteSheetResource.GetObject(this.Context);
            this.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, frame));
            return this;
        }

        public Sprite SetMotion(Direction direction, float speed)
        {
            this.MotionManager.MainMotion.Set(direction, speed);
            return this;
        }

        public static Sprite Create(ObjectType type, Layer layer, RGPointI location)
        {
            var sprite = new Sprite(layer, type);
            layer.AddObject(sprite);
            return sprite;
        }

        public static Sprite Create(Layer layer, RGPointI location)
        {
            return Create(ObjectType.Thing, layer, location);
        }

        public Sprite CreateChild(ObjectType newType, RGPointI offset)
        {
            return Sprite.Create(newType,this.DrawLayer, this.Location.Offset(offset));
        }

        #endregion 

    }    
}
