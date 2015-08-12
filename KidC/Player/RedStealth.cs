using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class RedStealthSwordController : SpriteBehavior 
    {
        private PlatformerPlayerController mPlatformCtl;
        private TileLayer mLayer;
        private bool mNoAirSword = false;

        public RedStealthSwordController(Sprite s, PlatformerPlayerController platformCtl)
            : base(s)
        {
            mPlatformCtl = platformCtl;
        }

        protected override void OnEntrance()
        {
            mLayer = this.Sprite.DrawLayer as TileLayer;
        }

        private bool CanAttack()
        {
            if (this.Sprite.CurrentAnimationKey == KCAnimation.Crawl)
                return false;

            if (this.Sprite.CurrentAnimationKey == KCAnimation.ClimbDown)
                return false;

            return true;
        }

        protected override void Update()
        {
            if (this.Sprite.CurrentAnimationKey == KCAnimation.Attack)
            {
                this.Sprite.MotionManager.StopMotionInDirection(DirectionFlags.Horizontal);

                if (this.Sprite.CurrentAnimation.Finished)
                {
                    mPlatformCtl.Resume();
                    return;
                }

                return;
            }

            if (mPlatformCtl.Player.Input.KeyPressed(KCButton.Special))
            {
                if (this.Sprite.CurrentAnimationKey == KCAnimation.Attack || this.Sprite.CurrentAnimationKey == KCAnimation.AttackAlt)
                    return;

                if (!this.Sprite.IsOnGround())
                {
                    if (!mNoAirSword)
                    {
                        SoundManager.PlaySound(Sounds.RedStealthShout);
                        this.Sprite.SetAnimation(KCAnimation.AttackAlt);
                        mPlatformCtl.NoAnimationChanges = true;
                    }
                }
                else if(CanAttack())
                {
                    mPlatformCtl.Pause();
                    this.Sprite.SetAnimation(KCAnimation.Attack);
                    SoundManager.PlaySound(Sounds.RedStealthAttack);
                }
            }
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(ObjectType.Block) && cEvent.CollisionSide == Engine.Collision.Side.Bottom) //TBD- block types
            {
                if (Sprite.CurrentAnimationKey == KCAnimation.AttackAlt)
                    response.AddInteraction(new RedStealthHitsBrick(), this);
                else
                {
                    mPlatformCtl.NoAnimationChanges = false;
                    mNoAirSword = false;
                }
            }

            base.HandleCollisionEx(cEvent, response);
        }

        public void OnBlockBroken()
        {
            mPlatformCtl.NoAnimationChanges = false;
            mNoAirSword = true;            
        }
    }

    class RedStealthHitsBrick : Interaction<RedStealthSwordController, IBreakableTile>
    {
        protected override void DoAction(RedStealthSwordController controller1, IBreakableTile controller2)
        {
            var ninja = controller1.Sprite;

            if (ninja.CurrentAnimationKey == KCAnimation.AttackAlt && ninja.OriginalSpeed.Y > 2f)
            {
                controller2.Break();
                controller1.OnBlockBroken();
            }           
        }
    }

}
