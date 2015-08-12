using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Collision
{
    class ObjectCollisionManager<T> : CollisionManager<T> where T : LogicObject, ICollidable
    {

        private ObjectType[] mCollisionTypes;

        public ObjectCollisionManager(T obj, ObjectType[] collisionTypes)
            : base(obj)
        {
            mCollisionTypes = collisionTypes;
        }
        protected override IEnumerable<CollisionEvent> CheckCollisions(WorldCollisionInfo info)
        {
            foreach (var collidable in info.CollisionListener.GetObjects())
            {
                if (collidable == this.CollidingObject)
                    continue;

                if (!this.CollidingObject.CanCollideWith(collidable))
                    continue;

                //primary - primary

                //secondary - primary

                var primaryCollide = this.CollidingObject.Area.CollidesWith(collidable.Area);
                var secondaryCollide = this.CollidingObject.Area.CollidesWith(collidable.SecondaryCollisionArea);

                DebugTracker<bool>.AddLog(this, secondaryCollide, p => secondaryCollide);

                if (primaryCollide || secondaryCollide)
                {
                    var evt= new CollisionEvent(this.CollidingObject, collidable, true, true, true, true, false, HitboxType.Primary, (secondaryCollide ? HitboxType.Secondary : HitboxType.Primary));
                    yield return evt;
                }
            }
        }
    }
    
}
