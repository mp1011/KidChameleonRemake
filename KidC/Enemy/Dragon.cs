using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class Dragon
    {

        public static SpriteCreationInfo Create(GameContext context, Layer layer)
        {

            var dragon = new Sprite(context, layer, KCObjectType.Dragon);

            var spriteSheet = SpriteSheet.Load("dragon", context);

            dragon.AddAnimation(KCAnimation.Walk, new Animation(spriteSheet, Direction.Left, 1, 2, 3, 4, 5, 6)).SetFrameDuration(8);
            dragon.AddAnimation(KCAnimation.Hurt, new Animation(spriteSheet, Direction.Left, 0));
            dragon.AddAnimation(KCAnimation.Attack, new Animation(spriteSheet, Direction.Left, 8, 9)).SetFrameDuration(2);
            dragon.AddAnimation(KCAnimation.AttackAlt, new Animation(spriteSheet, Direction.Left, 10)).SetFrameDuration(2);
            dragon.AddAnimation(KCAnimation.Dying, new Animation(spriteSheet, Direction.Left, false, 12, 11)).SetFrameDuration(8);
            dragon.AddAnimation(KCAnimation.Dead, new Animation(spriteSheet, Direction.Left, 13, 13, 13, 14)).SetFrameDuration(8);

            var healthCtl = new HealthController(dragon, 2);
            new GravityController(dragon);
            var walkController = new WalkController(dragon, .5f, 2f) { WalksOffLedges = false };
            var attackController = new DragonAttackController(dragon);
            var hitController = new EnemyHitController(dragon, healthCtl);
            var deathController = new DyingSpriteController(dragon);

            healthCtl.ContinueWith(deathController);
            
            new RandomActionController<Direction?>(dragon, walkController, 120,1f, Direction.Left, Direction.Right);                      
            new RandomActionController<bool>(dragon, attackController, 180, .6f);
            new BehaviorExclusionController(dragon, attackController, walkController);
            new BehaviorExclusionController(dragon, hitController, walkController, attackController);
            new BehaviorExclusionController(dragon, deathController, hitController, walkController, attackController);

            dragon.AddCollisionChecks(ObjectType.Block, ObjectType.Border, KCObjectType.Player);

            return new SpriteCreationInfo(dragon);
        }

        private class DragonAttackController : TriggeredController<bool>
        {
            protected override bool AllowRetrigger { get { return false; } }

            public DragonAttackController(Sprite s)
                : base(s)
            {
            }

            protected override Switch OnTriggered(bool arg)
            {
                this.Sprite.MotionManager.MainMotion.Set(0f);
                this.Sprite.CurrentAnimationKey = KCAnimation.AttackAlt;
                return Switch.On;
            }

            protected override Switch OnTriggerUpdate(bool state)
            {
                this.Sprite.MotionManager.MainMotion.Set(0f);
                if (this.TriggerDuration > 30)
                    this.Sprite.CurrentAnimationKey = KCAnimation.Attack;

                if (this.TriggerDuration > 160)
                    return Switch.Off;
                
                return Switch.On;
            }

            protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
            {
                base.HandleCollisionEx(cEvent, response);
            }
        }
    
    

    
    }
}
