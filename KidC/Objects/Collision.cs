using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Collision;

namespace KidC
{

    class HealthController : SpriteBehavior
    {
        public int HitPoints { get; private set; }
    
        public HealthController(Sprite s, int initialHitpoints)
            : base(s)
        {
            this.HitPoints = initialHitpoints;
        }

        protected override void Update()
        {
            if (this.HitPoints <= 0)
                this.Kill(Engine.ExitCode.Destroyed);
         
        }

        public void Damage(int amount)
        {
            HitPoints = Math.Max(0, HitPoints - amount);
        }
    }

    abstract class HitController : SpriteBehavior, ITriggerable  
    {
        private HitInfo mCurrentHitInfo;        
        private int mDuration;
        private HealthController mHealthController;


        public HitController(Sprite s, int hitDuration, HealthController healthController) : base(s)
        {
            mDuration = hitDuration;
            mHealthController = healthController;
        }

        public void RegisterHit(HitInfo hitInfo)
        {
            if (mCurrentHitInfo != null && mCurrentHitInfo.HitFrame < this.Context.CurrentFrameNumber)
                return; 

            if (mCurrentHitInfo == null || hitInfo.Precedence > mCurrentHitInfo.Precedence)
            {
                if(ShouldHandleCollision(hitInfo))
                    mCurrentHitInfo = hitInfo;
            }
        }

        protected override void Update()
        {
            if (mCurrentHitInfo == null)
                return;

            if (mCurrentHitInfo.HitFrame == Context.CurrentFrameNumber)
            {

                if (this.IsInvincible)
                {
                    mCurrentHitInfo = null;
                    return; //cancel the hit
                }

                SoundManager.PlaySound(this.GetHitSound(mCurrentHitInfo));
                OnHit(mCurrentHitInfo);
                mHealthController.Damage(mCurrentHitInfo.Damage);
            }
            else if (Context.ElapsedFramesSince(mCurrentHitInfo.HitFrame) < this.mDuration)
                WhileHit();
            else
            {
                AfterHit();
                mCurrentHitInfo = null;
                return;
            }

        }
        protected abstract void OnHit(HitInfo hitInfo);
        protected abstract void WhileHit();
        protected abstract void AfterHit();

        protected virtual SoundResource GetHitSound(HitInfo hitInfo)
        {
            return hitInfo.Sound;
        }

        protected abstract bool ShouldHandleCollision(HitInfo hitInfo);
        public abstract int GetAttackDamage(CollisionEvent evt);


        public bool Triggered
        {
            get { return mCurrentHitInfo != null;}
        }

        private bool IsInvincible
        {
            get
            {
                return Context.CurrentFrameNumber < this.InvincibleUntil;
            }
        }

        public ulong InvincibleUntil { get; set; }
    }

    interface IDamagerxxx
    {
        int GetAttackDamage(CollisionEvent collision);
    }



}
