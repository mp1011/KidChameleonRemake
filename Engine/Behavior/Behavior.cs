using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public abstract class SpriteBehavior : LogicObject, ICollisionResponder 
    {
        public Sprite Sprite { get; private set; }

        public SpriteBehavior(Sprite sprite)
            : this(sprite, RelationFlags.Normal) 
        {
        }


        public SpriteBehavior(Sprite sprite, RelationFlags relationFlags)
            : base(LogicPriority.Behavior, sprite, relationFlags)
        {
            this.Sprite = sprite;
            this.Sprite.CollisionResponders.Add(this);
        }

        public void HandleCollision(CollisionEvent cEvent, CollisionResponse response)
        {
            if (!this.Alive || this.Paused)
                return;

            HandleCollisionEx(cEvent,response);
        }

        protected virtual void HandleCollisionEx(CollisionEvent cEvent,CollisionResponse response)
        {        
        }
    }

}
