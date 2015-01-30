using Engine;

namespace KidC
{

    class EnemyHitController : HitController
    {
        public EnemyHitController(Sprite s) : base(s, 30) { }
        protected override bool AllowRetrigger { get { return true; } }

        protected override bool ShouldHandleCollision(Engine.Collision.CollisionEvent evt)
        {
            return evt.OtherType.Is(KCObjectType.Player);
        }

        public override int GetAttackDamage(Engine.Collision.CollisionEvent evt)
        {
            if (evt.OtherHitboxType == HitboxType.Secondary)
                return 0;

            if (evt.Invert().IsStomp())
                return 0;

            return 1; //todo
        }

        protected override void OnHit()
        {
            this.Sprite.MotionManager.MainMotion.Set(0f);
            this.Sprite.CurrentAnimationKey = KCAnimation.Hurt;
        }

        protected override void AfterHit()
        {
        }

        protected override void WhileHit()
        {
        }



        protected override SoundResource GetHitSound(Engine.Collision.CollisionEvent evt)
        {
            if (evt.IsStomp())
                return Sounds.EnemyBounce;
            else
                return Sounds.EnemyHit;
        }
    }


    class DyingSpriteController : TriggeredController<bool>
    {

        public DyingSpriteController(Sprite s) : base(s) { }
        protected override bool AllowRetrigger { get { return false; } }

        protected override Switch OnTriggered(bool state)
        {
            this.Sprite.CurrentAnimationKey = KCAnimation.Dying;
            this.Sprite.RemoveCollisionType(ObjectType.Thing);
            return Switch.On;
        }

        private bool mDying = false;

        protected override Switch OnTriggerUpdate(bool state)
        {
            if (mDying)
                return Switch.On;

            this.Sprite.MotionManager.MainMotion.Set(0f);
            if (this.Sprite.CurrentAnimation.Finished)
            {
                if (this.Sprite.CurrentAnimationKey != KCAnimation.Dead)
                {
                    mDying = true;
                    this.Sprite.CurrentAnimationKey = KCAnimation.Dead;
                    TimedAction<Sprite>.DelayedAction(h => h.Kill(Engine.ExitCode.Destroyed), this.Sprite, 120);
                }
            }

            return Switch.On;
        }

    }

}