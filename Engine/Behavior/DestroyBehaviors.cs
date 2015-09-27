using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public class DestroyWhenOutOfFrameGeneric<T> : LogicObject where T : ILogicObject, IWithPosition
    {
        private T mObject;
        private bool mIsOnScreenLayer;

        public DestroyWhenOutOfFrameGeneric(T obj, bool isOnScreenLayer)
            : base(LogicPriority.Behavior, obj)
        {
            mObject = obj;
            mIsOnScreenLayer = isOnScreenLayer;
        }

        protected override void Update()
        {
            if (mIsOnScreenLayer)
            {
                if (!mObject.Area.CollidesWith(Context.CurrentWorld.ScreenLayer.Location))
                    mObject.Kill(ExitCode.Removed);
            }
            else
            {
                if (!mObject.Area.CollidesWith(Context.ScreenLocation))
                    mObject.Kill(ExitCode.Removed);
            }
        }
    }

    public class DestroyWhenOutOfFrame : DestroyWhenOutOfFrameGeneric<Sprite>
    {
        public DestroyWhenOutOfFrame(Sprite obj) : base(obj, false) { }
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

    public class DestroyOnCollision : SpriteBehavior
    {
        public DestroyOnCollision(Sprite s) : base(s) 
        {
            s.AddCollisionChecks(ObjectType.Block);
        }

        protected override void HandleCollisionEx(Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            this.Sprite.Kill(Engine.ExitCode.Destroyed);
        }
    }
}
