using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;

namespace KidC
{

    class RubberBlock : KCCollidingTile
    {
        private SimpleAnimation mAnimation;

        public bool DoBounce { get; set; }

        public RubberBlock(KCTileInstance tile, TileCollisionView collisionView)
            : base(tile, collisionView) 
        {
            mAnimation = KidCGraphic.RubberBlockBounce.CreateSimpleAnimation(this.Context);
        }

        protected override SoundResource HitSound
        {
            get { return Sounds.RubberBounce; }
        }

        protected override SpecialTile? FinalTile
        {
            get
            {
                return SpecialTile.Rubber;
            }
        }

        protected override bool ShouldInteract(CollisionEvent collision, CollisionResponse response)
        {
      //      response.AddInteraction(new PlayerBounce(collision.Invert()), this);
            return DoBounce;
        }

        protected override void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mAnimation.Location = this.Location;
            mAnimation.Draw(p, canvas);
        }

        protected override void Update()
        {
            if (this.Age >= 30)           
                this.Kill(Engine.ExitCode.Removed);            
        }
    }

    class PlayerBounceController : ITypedCollisionResponder<RubberBlock>
    {
        private PlatformerPlayerController mPlatformCtl;
        private TransformationStats Stats { get { return mPlatformCtl.MotionStats; } }
        private DirectedMotion mHorizontalBounce;

        public PlayerBounceController(Sprite s, PlatformerPlayerController platformCtl)
        {
            this.RegisterTypedCollider(s);
            mPlatformCtl = platformCtl;
        }

        public void HandleCollision(RubberBlock rubberBlock, CollisionEvent collision, CollisionResponse response)
        {

            if(mHorizontalBounce == null)
            {
                mHorizontalBounce = new DirectedMotion("Horizontal Bounce");
                mHorizontalBounce.Inactive=true;
                mHorizontalBounce.Decel = this.Stats.SideBounceDecel;
                mPlatformCtl.Sprite.MotionManager.AddComponent(mHorizontalBounce);
            }

            if (collision.CollisionSide == Side.Bottom)
            {
                var ySpeed = collision.CollisionSpeed.Y;

                if (ySpeed <= .5)
                    return;

                mPlatformCtl.DoBounce((Math.Min(ySpeed, this.Stats.VerticalBounceStrength)));
                rubberBlock.DoBounce = true;
            }
            else if (collision.CollisionSide == Side.Left)
            {
                rubberBlock.DoBounce = true;
                mHorizontalBounce.Inactive = false;
                mHorizontalBounce.Direction = Direction.Right;
                mHorizontalBounce.TargetSpeed = 0;
                mHorizontalBounce.CurrentSpeed = this.Stats.SideBounceSpeed;
            }
            else if (collision.CollisionSide == Side.Right)
            {
                rubberBlock.DoBounce = true;
                mHorizontalBounce.Inactive = false;
                mHorizontalBounce.Direction = Direction.Left;
                mHorizontalBounce.TargetSpeed = 0;
                mHorizontalBounce.CurrentSpeed = this.Stats.SideBounceSpeed;
            }
        }
    }
}
