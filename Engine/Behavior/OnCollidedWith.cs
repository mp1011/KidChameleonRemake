using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class OnCollidedWithExtensions
    {
        public static ILogicObject OnCollision(this Sprite s, ObjectType t, ILogicObject continuation)
        {
            return new CollisionWaiter(s, t).ContinueWith(continuation);
        }
    }

    class CollisionWaiter : SpriteBehavior
    {
        private ObjectType mCollideType;

        public CollisionWaiter(Sprite s, ObjectType collideType)
            : base(s)
        {
            mCollideType = collideType;
        }

 
        protected override void HandleCollisionEx(Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(mCollideType))
                this.Kill(Engine.ExitCode.Finished);
        }
    }
}
