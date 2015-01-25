using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public abstract class SpriteBehavior : LogicObject
    {
        public Sprite Sprite { get; private set; }

        public SpriteBehavior(Sprite sprite)
            : base(LogicPriority.Behavior, sprite, RelationFlags.Normal)
        {
            this.Sprite = sprite;
            this.Sprite.AddBehavior(this);
        }


        public SpriteBehavior(Sprite sprite, RelationFlags relationFlags)
            : base(LogicPriority.Behavior, sprite, relationFlags)
        {
            this.Sprite = sprite;
            this.Sprite.AddBehavior(this);
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
