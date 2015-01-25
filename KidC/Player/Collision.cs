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

        public PlayerHitController(Sprite s, bool secondaryHitboxIsDamaging) : base(s, 60)
        {
            mSecondaryHitboxIsDamaging = secondaryHitboxIsDamaging;
        }

        protected override void OnEntrance()
        {

            mGravityController = this.Sprite.GetBehavior<GravityController>();
            base.OnEntrance();
        }

        protected override bool ShouldHandleCollision(Engine.Collision.CollisionEvent evt)
        {
            return evt.OtherType.Is(KCObjectType.Enemy);
        }

        public override int GetAttackDamage(Engine.Collision.CollisionEvent evt)
        {
            if (evt.OtherHitboxType == HitboxType.Secondary)
                return 0;

            if (evt.IsStomp())
                return 1; //todo

            if(evt.ThisHitboxType == HitboxType.Secondary && mSecondaryHitboxIsDamaging)
                return 1; //todo

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

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(KCObjectType.Enemy) && cEvent.IsStomp())
            {
                var ySpeed = cEvent.CollisionSpeed.Y;

                if (ySpeed >= 0)
                    mHitBounceSpeed = Math.Max(2f, ySpeed);

                response.ShouldBlock = true;
                response.CorrectionVector = new RGPoint(0, -(cEvent.ThisCollisionTimeArea.Bottom - cEvent.OtherCollisionTimeArea.Top));
            }

            base.HandleCollisionEx(cEvent, response);
        }

        protected override void OnHit()
        {
            Context.GetStats().CurrentHealth--;

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

        protected override SoundResource GetHitSound(Engine.Collision.CollisionEvent evt)
        {
            return Sounds.PlayerHit;
        }
    }

    class PlayerDieController : SpriteBehavior
    {
        public PlayerDieController(Sprite s) : base(s) { }

        protected override void OnExit()
        {
            if (this.ExitCode == Engine.ExitCode.Destroyed)
            {
                SoundManager.PlaySound(Sounds.PlayerDie);
                Presets.Debris.Create(this.Sprite, this.Sprite.GetAnimation(KCAnimation.Dead));

                this.Context.SetCameraCenter(null);
            }
        }


    }
}
