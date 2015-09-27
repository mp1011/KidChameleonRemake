using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Collision;

namespace KidC
{
    interface IDamager  
    {
        int AttackStrength { get; }
        SoundResource HitSound { get; }
    }

    enum Precedence
    {
        Ignore = -1,
        Low = 0,
        Medium = 50,
        High = 100
    }

    interface IDamageable
    {
        ulong InvincibleUntil { get; set; } 
        void Damage(HitInfo hitInfo);
    }

    class HitInfo
    {
        public ObjectType DamagingType { get; private set;}
        public ObjectType DamagedType { get; private set;}
        public ulong HitFrame { get; private set; }
        public Precedence Precedence { get; private set; }
        public int Damage { get; private set; }
        public SoundResource Sound{get; private set;}

        public HitInfo(ICollidable collider, IDamager damager, CollisionEvent evt)
        {
            evt = evt.AdjustTo(collider);

            DamagingType = evt.ThisType;
            DamagedType = evt.OtherType;
            HitFrame = evt.CollisionTime;
            Precedence = KidC.Precedence.Low;          
            this.Damage = damager.AttackStrength;       
            this.Sound = damager.HitSound;
        }

        public HitInfo(ICollidable collider, IDamager damager, CollisionEvent evt, Precedence precedence)
            : this(collider, damager, evt)
        {
            this.Precedence = precedence;
        }
    }

    
    /// <summary>
    /// Causes the player to damage enemies by jumping on them
    /// </summary>
    class StompDamager : ITypedCollisionResponder<IDamageable>, IDamager
    {
        private GravityController mGravityController;
        private PlayerSprite mPlayer;

        public StompDamager(PlayerSprite player, GravityController gravityCtl)
        {
            this.RegisterTypedCollider(player);
            mGravityController = gravityCtl;
            mPlayer = player;
        }

        public void HandleCollision(IDamageable other, CollisionEvent collision, CollisionResponse response)
        {
            collision = collision.AdjustTo(mGravityController.Sprite);
            if (collision.OtherType.IsNot(KCObjectType.Enemy) || !IsStomp(collision))
                return;

            Bounce(collision, response);
            other.Damage(new HitInfo(mGravityController.Sprite, this, collision, Precedence.High));

            mPlayer.InvincibleUntil = mPlayer.Context.CurrentFrameNumber + 10;
        }

        private void Bounce(CollisionEvent cEvent, CollisionResponse response)
        {
            var ySpeed = cEvent.CollisionSpeed.Y;

            if (ySpeed >= 0)
                mGravityController.CurrentYSpeed = -Math.Max(2f, ySpeed);

            response.ShouldBlock = true;
            response.CorrectionVector = new RGPointI(0, -(cEvent.ThisCollisionTimeArea.Bottom - cEvent.OtherCollisionTimeArea.Top));
        }

        private bool IsStomp(CollisionEvent evt)
        {
            if (evt.ThisCollisionTimeSpeed.Y < 0)
                return false;

            var prevArea = evt.ThisObjectPreviousPosition;
            return prevArea.Bottom <= evt.OtherArea.Top + 2;
        }

        public int AttackStrength
        {
            get { return 1;  }
        }

        public SoundResource HitSound
        {
            get { return Sounds.EnemyBounce; }
        }
    }

    /// <summary>
    /// Causes enemies to damage the player on collision
    /// </summary>
    class TouchDamager : ITypedCollisionResponder<IDamageable>, IDamager 
    {
        private Sprite mSprite;
        public TouchDamager(Sprite s)
        {
            mSprite = s;
            this.RegisterTypedCollider(s);
        }

        public int AttackStrength
        {
            get { return 1; }
        }

        public SoundResource HitSound
        {
            get { return Sounds.PlayerHit; }
        }

        public void HandleCollision(IDamageable other, CollisionEvent collision, CollisionResponse response)
        {
            collision = collision.AdjustTo(this.mSprite);
            other.Damage(new HitInfo(mSprite, this, collision));
        }
    }
}
