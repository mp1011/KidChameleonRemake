using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public class Listeners
    {
        private GameContext mContext;
        private CollisionListener mCollisionListener;

        public Listeners(GameContext ctx)
        {
            mContext = ctx;
        }

        public CollisionListener CollisionListener { get { return mCollisionListener ?? (mCollisionListener = new CollisionListener(mContext)); } }
    }

    public abstract class Listener<T> : LogicObject
    {
        protected List<T> Objects { get; private set; }

        public IEnumerable<T> GetObjects() { return this.Objects.ToArray(); }

        public Listener(GameContext ctx)
            : base(LogicPriority.World, ctx)
        {
            this.Objects = new List<T>();
        }

        public void Register(T obj)
        {
            Objects.Add(obj);
        }

        public void Unregister(T obj)
        {
            //todo Objects.Remove(obj);
        }
    }

    public class CollisionListener : Listener<ICollidable>
    {
        public CollisionListener(GameContext ctx) : base(ctx) { }
    }
}
