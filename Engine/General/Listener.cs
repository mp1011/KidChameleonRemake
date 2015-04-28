using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public class Listeners
    {
        public Listeners(ILogicObject owner)
        {
            CollisionListener = new CollisionListener(owner);
        }

        public CollisionListener CollisionListener { get; private set; }
    }

    public abstract class Listener<T> : LogicObject
    {
        protected List<T> Objects { get; private set; }

        public IEnumerable<T> GetObjects() { return this.Objects.ToArray(); }

        public Listener(ILogicObject owner)
            : base(LogicPriority.World, owner)
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
        public CollisionListener(ILogicObject owner) : base(owner) { }
    }
}
