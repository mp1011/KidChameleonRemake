using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class PlayerHitController : HitController 
    {
        private GravityController mGravityController;

        private float mHitBounceSpeed;
        private bool mSecondaryHitboxIsDamaging;
        private NoHitBonus mNoHitBonus;
        private PlayerSprite mPlayer;

        public PlayerHitController(PlayerSprite player, bool secondaryHitboxIsDamaging, GravityController gravityController, HealthController healthController)
            : base(player, 60, healthController)
        {
            mSecondaryHitboxIsDamaging = secondaryHitboxIsDamaging;
            mGravityController = gravityController;
            mNoHitBonus = this.Context.GetBonusTrackers().OfType<NoHitBonus>().FirstOrDefault();
            mPlayer = player;
        }

        protected override bool ShouldHandleCollision(HitInfo hitInfo)
        {
            return hitInfo.DamagingType.Is(KCObjectType.Enemy);
        }

        public override int GetAttackDamage(Engine.Collision.CollisionEvent evt)
        {
            //if (evt.OtherHitboxType == HitboxType.Secondary)
            //    return 0;

            //if (evt.IsStomp())
            //    return 1; //todo

            //if(evt.ThisHitboxType == HitboxType.Secondary && mSecondaryHitboxIsDamaging)
            //    return 1; //todo

            return 0;
        }

        protected override void Update()
        {
            if (mHitBounceSpeed > 0)
            {

                mHitBounceSpeed = mHitBounceSpeed.Fix(1f, 3f);
                if (mGravityController != null)
                    mGravityController.CurrentYSpeed = -(mHitBounceSpeed + .5f);

                mHitBounceSpeed = 0f;
            }

            base.Update();
        }

        protected override void OnHit(HitInfo hitInfo)
        {
            if (hitInfo.Damage > 0)
                mNoHitBonus.Reject();

            Context.GetStats().CurrentHealth -= hitInfo.Damage;

            if (Context.GetStats().CurrentHealth <= 0)
                this.Sprite.Kill(Engine.ExitCode.Destroyed);
        }

        protected override void AfterHit()
        {
            this.Sprite.CurrentAnimation.RenderOptions.SetFlashOff();            
        }

        protected override void WhileHit()
        {
            this.Sprite.CurrentAnimation.RenderOptions.SetFlashOn(Context.CurrentFrameNumber);
        }
    }
}
