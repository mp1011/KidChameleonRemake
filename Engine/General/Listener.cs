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
            SpriteListener = new SpriteListener(owner);
        }

        public SpriteListener SpriteListener { get; private set; }
    }

    public class Listener<T> : LogicObject 
    {
        protected LinkedList<T> Objects { get; private set; }

        public IEnumerable<T> GetObjects() { return this.Objects; }

        public Listener(ILogicObject owner)
            : base(LogicPriority.World, owner)
        {
            this.Objects = new LinkedList<T>();
        }

        public void Register(T obj)
        {
            Objects.AddLast(obj);
        }

        
    }

    public class CollidableListener : Listener<ICollidable>
    {
        public CollidableListener(ILogicObject owner)
            : base(owner)
        {
        }

        protected override void Update()
        {
            var o = Objects.First;
            while (o != null)
            {
                if (!o.Value.Alive)
                    Objects.Remove(o);
                o = o.Next;
            }
        }
    }

    public class SpriteListener : Listener<Sprite>
    {
        public SpriteListener(ILogicObject owner) : base(owner) { }
    }
}
