﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class Dragon
    {

        public static Sprite Create(GameContext context, Layer layer)
        {

            var dragon = new Sprite(context, layer, KCObjectType.Dragon);

            var spriteSheet = SpriteSheet.Load("dragon", context);

            dragon.AddAnimation(KCAnimation.Walk, new Animation(spriteSheet, Direction.Left, 1, 2, 3, 4, 5, 6, 7)).SetFrameDuration(8);
            dragon.AddAnimation(KCAnimation.Hurt, new Animation(spriteSheet, Direction.Left, 0));
            dragon.AddAnimation(KCAnimation.Attack, new Animation(spriteSheet, Direction.Left, 8, 9)).SetFrameDuration(2);
            dragon.AddAnimation(KCAnimation.AttackAlt, new Animation(spriteSheet, Direction.Left, 10)).SetFrameDuration(2);

            dragon.AddAnimation(KCAnimation.Dying, new Animation(spriteSheet, Direction.Left, false, 12, 11)).SetFrameDuration(8);
            dragon.AddAnimation(KCAnimation.Dead, new Animation(spriteSheet, Direction.Left, 13, 13, 13, 14)).SetFrameDuration(8);


            dragon.AddBehavior(new GravityController(dragon));
            var walkController = dragon.AddBehavior(new WalkController(dragon, .5f, 2f) { WalksOffLedges = false });
            var attackController = dragon.AddBehavior(new DragonAttackController(dragon));
            var hitController = dragon.AddBehavior(new EnemyHitController(dragon));
            var deathController = dragon.AddBehavior(new DyingSpriteController(dragon));

            dragon.AddBehavior(new HealthController(dragon, 2));
            
            dragon.AddBehavior(new RandomActionController<Direction?>(dragon, walkController, 120,1f, Direction.Left, Direction.Right));                      
            dragon.AddBehavior(new RandomActionController<bool>(dragon, attackController, 180, .6f));
            dragon.AddBehavior(new BehaviorExclusionController(dragon, attackController, walkController));
            dragon.AddBehavior(new BehaviorExclusionController(dragon, hitController, walkController, attackController));
            dragon.AddBehavior(new BehaviorExclusionController(dragon, deathController, hitController, walkController, attackController));

            dragon.AddCollisionChecks(ObjectType.Block, ObjectType.Border, KCObjectType.Player);

            return dragon;
        }

        private class DragonAttackController : TriggeredController<bool>
        {
 
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