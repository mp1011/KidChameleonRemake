using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public class GravityController : SpriteBehavior
    {
        private XYMotion mGravity;

        public float CurrentYSpeed { get { return mGravity.YSpeed.Current; } set { mGravity.YSpeed.Current = value; } }

        public GravityController(Sprite sprite)
            : base(sprite)
        {
            mGravity = new XYMotion("gravity");
            mGravity.YSpeed.Target = 9f;
            mGravity.YSpeed.Accel = .2f;

            Sprite.MotionManager.AddComponent(mGravity);
        }

        public bool GravityEnabled
        {
            get
            {
                return !mGravity.Inactive;
            }
            set
            {
                mGravity.Inactive = !value;
            }
        }

        protected override void OnEntrance()
        {
           
        }

        protected override void Update()
        {
          
        }

        protected override void HandleCollisionEx(CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(ObjectType.Block))
            {
                if (response.CorrectionVector.Y < 0 || cEvent.CollisionSide == Side.Bottom)
                    this.Sprite.MotionManager.StopMotionInDirection(new DirectionFlags(Direction.Down));
            }
        }           
    }

    public class DestroyWhenOutOfFrame : SpriteBehavior
    {
        public DestroyWhenOutOfFrame(Sprite sprite)
            : base(sprite)
        {
        }

        protected override void Update()
        {
            if (!Sprite.Area.CollidesWith(Context.ScreenLocation))            
                Sprite.Kill(ExitCode.Removed);            
        }
    }

    public class DestroyWhenAnimationFinished : SpriteBehavior
    {
        public DestroyWhenAnimationFinished(Sprite sprite)
            : base(sprite)
        {
        }

        protected override void Update()
        {
            if (Sprite.CurrentAnimation.Finished)            
                this.Sprite.Kill(Engine.ExitCode.Removed);            
        }
    }

    public class CreateObjectWhenDestroyed : SpriteBehavior
    {
        private ObjectType mType;
        private RGPoint mOffset;
       
        public CreateObjectWhenDestroyed(Sprite sprite, ObjectType objType, RGPoint offset)
            : base(sprite)
        {
            mType = objType;
            mOffset = offset;
        }

        protected override void OnExit()
        {
            if (mType.Is(ObjectType.None))
                return;

            var obj = mType.CreateSprite(this.Sprite.DrawLayer, this.Context).Sprite;
            obj.Location = this.Sprite.Location.Offset(mOffset);
        }
    }

    public class SeekPointController : LogicObject 
    {
        private IMoveableWithPosition mObject;
        private IWithPosition mTarget;
        public float Speed { get; set; }
        public bool ReachedTarget { get; private set; }

        public SeekPointController(ILogicObject owner, IMoveableWithPosition obj, IWithPosition target, float speed)
            : base(LogicPriority.Behavior, owner, RelationFlags.Normal)
        {
            mObject = obj;
            mTarget = target;
            this.Speed = speed;
        }


        protected override void Update()
        {
            var dir = this.mObject.Location.GetDirectionTo(mTarget.Location);
            if (this.mObject.Location.GetDistanceTo(mTarget.Location) <= this.Speed)
            {
                this.mObject.Location = mTarget.Location;
                this.ReachedTarget = true;
                this.Kill(Engine.ExitCode.Finished);
                mObject.MotionManager.StopAllMotion();
            }
            else
            {
                this.mObject.MotionManager.MainMotion.Set(dir, Speed);
                this.ReachedTarget = false;
            }
        }
    }


    //public class SeekPointController : SpriteBehavior
    //{
    //    private IWithPosition mTarget;
    //    public int Speed { get; set; }
    //    public bool ReachedTarget { get; private set; }

    //    public SeekPointController(Sprite sprite, IWithPosition target)
    //        : base(sprite)
    //    {
    //        mTarget = target;
    //        this.Speed = 5;
    //    }

    //    protected override void Update()
    //    {
    //        var dir = this.Sprite.Location.GetDirectionTo(mTarget.Location);
    //        if (this.Sprite.Location.GetDistanceTo(mTarget.Location) <= this.Speed)
    //        {
    //            this.Sprite.Location = mTarget.Location;
    //            this.ReachedTarget = true;
    //            this.Kill(Engine.ExitCode.Finished);
    //        }
    //        else
    //        {
    //            this.Sprite.MotionManager.MainMotion.Set(dir, 5f);
    //            this.ReachedTarget = false;
    //        }
    //    }

    //}

    public interface ITriggerable
    {
        int ID { get; }
        bool Triggered { get; }
    }

    public interface ITriggerable<T> : ITriggerable 
    {
        void Trigger(T arg);
    }

    public enum Switch
    {
        Off,
        On
    }

    public abstract class TriggeredController<T> : SpriteBehavior, ITriggerable <T>
    {
        public TriggeredController(Sprite s) : base(s) { }
        protected abstract bool AllowRetrigger { get; }

        public bool Triggered { get; private set; }
        private bool mRanStartMethod;
        private T mState;
        private ulong mTriggerStartFrame;

        public void Trigger(T state)
        {
            if (this.Triggered && !AllowRetrigger)
                return;

            this.mState = state;
            this.Triggered = true;
            this.mRanStartMethod = false;
        }

        protected int TriggerDuration { get { return Context.ElapsedFramesSince(mTriggerStartFrame); } }

        protected override void Update()
        {

            if (Triggered && !mRanStartMethod)
            {
                mRanStartMethod = true;
                mTriggerStartFrame = Context.CurrentFrameNumber;
                if (OnTriggered(mState) == Switch.On)
                    this.Triggered = true;
                else
                {
                    OnTriggerStop();
                    this.Triggered = false;
                }
            }

            if (Triggered)
            {
                if (OnTriggerUpdate(mState) == Switch.On)
                    this.Triggered = true;
                else
                {
                    OnTriggerStop();
                    this.Triggered = false;
                }
            }
        }

        protected abstract Switch OnTriggered(T state);
        protected virtual Switch OnTriggerUpdate(T state)
        {
            return Switch.Off;
        }

        protected virtual void OnTriggerStop()
        {
        }
    }

    public class RandomActionController<T> : SpriteBehavior
    {
        private ITriggerable<T> mAction;
        private ulong mLastCheckFrame;

        private List<Tuple<int, T>> mArgs = new List<Tuple<int, T>>();
        private bool mAllWeightsZero = true;

        private int mPeriod;
        private float mChance;

        public RandomActionController(Sprite sprite, ITriggerable<T> action, int period, float chance, params T[] args)
            : base(sprite)
        {
            mAction = action;
            mPeriod = period;
            mChance = chance;

            mArgs.AddRange(args.Select(p => new Tuple<int, T>(0, p)));

            if (args.IsNullOrEmpty())
                mArgs.Add(new Tuple<int, T>(0, default(T)));
        }

        public void AddArgument(int weight, T arg)
        {
            mArgs.Add(new Tuple<int, T>(weight, arg));
            if (weight > 0)
                mAllWeightsZero = false;
        }

        protected override void Update()
        {
            if (mAction.Triggered)
                mLastCheckFrame = Context.CurrentFrameNumber;

            if (Context.ElapsedFramesSince(mLastCheckFrame) > mPeriod)
            {
                mLastCheckFrame = Context.CurrentFrameNumber;
                if (!mAction.Triggered && Util.RandomChance(mChance))
                {
                    T arg;
                    if (mAllWeightsZero)
                        arg = mArgs.RandomItem().Item2;
                    else
                        arg = mArgs.RandomItem(p => p.Item1).Item2;

                    mAction.Trigger(arg);
                }
            }
        }
    }


    public class BehaviorExclusionController : SpriteBehavior 
    {

        private ITriggerable mTrigger;
        private SpriteBehavior[] mBehaviorsToPause;
    
        public BehaviorExclusionController(Sprite s, ITriggerable trigger, params SpriteBehavior[] behaviorsToPause)
            : base(s)
        {
            mTrigger = trigger;
            mBehaviorsToPause = behaviorsToPause;
        }

        private bool mWasTriggered;
        protected override void Update()
        {

            if (mTrigger.Triggered)
            {
                mWasTriggered = true;
                foreach (var behavior in mBehaviorsToPause)
                    behavior.Pause();
            }

            if (mWasTriggered && !mTrigger.Triggered)
            {
                mWasTriggered = false;
                foreach (var behavior in mBehaviorsToPause)
                    behavior.Resume();
            }
        }
    }

    public class KillObject : LogicObject
    {
        private ILogicObject mObject;
        private ExitCode mExitCode;

        public KillObject(ILogicObject obj, ExitCode exitCode) : base(LogicPriority.Behavior,obj)
        {
            mObject = obj;
            mExitCode = exitCode;
        }

        protected override void Update()
        {
            mObject.Kill(mExitCode);
        }
    }
  
}
