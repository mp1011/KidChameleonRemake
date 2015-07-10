using Engine;

namespace KidC
{

    class EnemyHitController : HitController
    {
        public EnemyHitController(Sprite s, HealthController healthController) : base(s, 30, healthController) { }
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

        protected override void OnHit(HitInfo hitInfo)
        {
            this.Sprite.MotionManager.MainMotion.Set(0f);
            this.Sprite.SetAnimation(KCAnimation.Hurt);
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


    class DyingSpriteController : SpriteBehavior, ITriggerable 
    {
        public DyingSpriteController(Sprite s) : base(s) { }

        protected override void OnResume()
        {
            this.Sprite.SetAnimation(KCAnimation.Dying);
            this.Sprite.RemoveCollisionType(ObjectType.Thing);
        }

        protected override void Update()
        {
            this.Sprite.SnapToGround(Sprite.DrawLayer as TileLayer);
            this.Sprite.MotionManager.MainMotion.Set(0f);
            if (this.Sprite.CurrentAnimation.Finished)
            {
                if (this.Sprite.CurrentAnimationKey != KCAnimation.Dead)
                {
                    this.Sprite.SetAnimation(KCAnimation.Dead);
                    new DelayWaiter(this,2.0f).ContinueWith(new KillObject(this.Sprite, ExitCode.Destroyed));                   
                }
            }
        }

        public bool Triggered
        {
            get { return this.Alive && !this.Paused; }
        }
    }

}