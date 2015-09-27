using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public interface ICollisionResponder
    {
        void HandleCollision(CollisionEvent collision, CollisionResponse response);
    }

    public interface ITypedCollisionResponder<T>
    {
        void HandleCollision(T other, CollisionEvent collision, CollisionResponse response);
    }

    class TypedCollisionResponder<T> : ICollisionResponder where T:class
    {
        private ITypedCollisionResponder<T> mTypedResponder;

        public TypedCollisionResponder(ITypedCollisionResponder<T> typedResponder)
        {
            mTypedResponder = typedResponder;
        }

        void ICollisionResponder.HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            var typed = collision.OtherObject as T;
            if (typed != null)
                mTypedResponder.HandleCollision(typed, collision, response);
        }
    }

    public static class ITypedCollisionResponderExtensions
    {
        public static void RegisterTypedCollider<T>(this ITypedCollisionResponder<T> responder, ICollidable collidingObject) where T:class
        {
            collidingObject.CollisionResponders.Add(new TypedCollisionResponder<T>(responder));
        }
    }
}
