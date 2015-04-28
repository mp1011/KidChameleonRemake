using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class CollectedItem
    {
        public static void Create(Sprite item)
        {
            IWithPosition target = null;
            var hud = item.Context.CurrentMapHUD(); 
         
            if (item.ObjectType.Is(KCObjectType.Gem))
                target = hud.GemsCounter;
            else
                return;

            var sprite = item.ObjectType.CreateSprite(item.Context.CurrentMapHUD(), item.Context).Sprite;
            sprite.BehaviorState.Pause();

            sprite.MotionManager.Reset();
            sprite.Location = item.DrawLayer.LayerPointToScreenPoint(item.Location);          

            new SeekPointController(sprite,sprite, target,5f).ContinueWith(new CollectedItemBehavior(sprite));
        }
    }

    class CollectedItemBehavior : SpriteBehavior
    {

        public CollectedItemBehavior(Sprite collectedItem) : base(collectedItem)
        {
        }

        protected override void OnEntrance()
        {
            if (this.Sprite.ObjectType.Is(KCObjectType.Gem))
                this.Context.GetStats().Gems++;

            this.Sprite.Kill(Engine.ExitCode.Removed);
        }

    }
}
