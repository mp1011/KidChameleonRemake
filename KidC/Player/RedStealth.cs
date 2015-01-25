using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class RedStealthController : PlatformerPlayerController
    {
        public RedStealthController(Sprite s, Player player)
            : base(s, player)
        {
            this.WalkSpeed = 1.8f;
            this.WalkAccel = .1f;
            this.StopAccel = 0.3f;

            this.CrawlSpeed = 1.0f;
            this.CrawlAccel = .01f;
            this.CrawlDecel = .1f;

            this.JumpVaryDuration = 20;
            this.JumpSpeed = 3.0f;

            this.CanClimbSlopes = false;
            this.DownHillSpeedMod = 0;

        }
    }

    class RedStealthSwordController : SpriteBehavior 
    {
        private RedStealthController mPlatformCtl;
        private TileLayer mLayer;
        private bool mNoAirSword = false;

        public RedStealthSwordController(Sprite s, RedStealthController platformCtl)
            : base(s)
        {
            mPlatformCtl = platformCtl;
        }

        protected override void OnEntrance()
        {
            mLayer = this.Sprite.DrawLayer as TileLayer;
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

                if (!this.Sprite.IsOnGround(mLayer))
                {
                    if (!mNoAirSword)
                    {
                        SoundManager.PlaySound(Sounds.RedStealthShout);
                        this.Sprite.CurrentAnimationKey = KCAnimation.AttackAlt;
                        mPlatformCtl.NoAnimationChanges = true;
                    }
                }
                else
                {
                    mPlatformCtl.Pause();
                    this.Sprite.CurrentAnimationKey = KCAnimation.Attack;
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
