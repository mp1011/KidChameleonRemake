using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Collision;

namespace KidC
{
    class IronKnightBrickBreakerController : SpriteBehavior 
    {
        public IronKnightBrickBreakerController(Sprite s)
            : base(s)
        {
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(ObjectType.Block) && cEvent.CollisionSide == Engine.Collision.Side.Bottom) //TBD- block types
                response.AddInteraction(new IronKnightHitsBrick(), this);

            base.HandleCollisionEx(cEvent, response);
        }
    }

    class IronKnightHitsBrick : Interaction<IronKnightBrickBreakerController, IBreakableTile>
    {
        protected override void DoAction(IronKnightBrickBreakerController controller1, IBreakableTile controller2)
        {
            var knight = controller1.Sprite;

            if(knight.OriginalSpeed.Y > 2f)
                controller2.Break();
        }
    }



    class IronKnightClimbController : SpriteBehavior 
    {
        private Player mPlayer;
        private XYMotion mClimbMotion;
        private float mCurrentWallX;
        private PlatformerPlayerController mPlatformController;

        public IronKnightClimbController(Sprite s, Player player, PlatformerPlayerController platformerController) : base(s)
        {
            mPlayer = player;
            mPlatformController = platformerController;
        }

        private bool mClimbing;
        private bool Climbing
        {
            get { return mClimbing; }
            set
            {
                if (value == mClimbing)
                    return;

                mClimbing = value;

                if (value)
                {
                    mPlatformController.Pause();
                    mPlatformController.GravityController.GravityEnabled = false;
                    this.Sprite.MotionManager.StopMotionInDirection(new DirectionFlags(false, false, true, true));
                    this.Sprite.SetAnimation(KCAnimation.IronKnightClimb);
                }
                else
                {
                    mPlatformController.Resume();
                    mPlatformController.GravityController.GravityEnabled = true;
                    this.mClimbMotion.YSpeed.Set(0f);
                }

            }
        }
        protected override void OnEntrance()
        {
            mClimbMotion = new XYMotion("Iron Knight Climb");
            this.Sprite.MotionManager.AddComponent(mClimbMotion);
        }

        private bool CanStartClimbing()
        {
            return this.Sprite.IsOnGround() &&
                IsAgainstWall();
                
        }

        private bool CanContinueClimbing()
        {
            return IsAgainstWall();
        }

        private bool IsAgainstWall()
        {
            return IsPointFacingWall(this.Sprite.Location.Offset(0, -16));
        }

        private void ClimbStep()
        {
            SoundManager.PlaySound(Sounds.IronKnightClimb);
            mClimbMotion.YSpeed.Current -= 1.0f;
            this.Climbing = true;

            this.Sprite.MotionManager.StopMotionInDirection(new DirectionFlags(Direction.Down));
            mClimbMotion.YSpeed.Target = 2f;

            mClimbMotion.YSpeed.Decel = .05f;
            mClimbMotion.YSpeed.Accel = .05f;

            if (mClimbMotion.YSpeed.Current < -4f)
                mClimbMotion.YSpeed.Current = -4;
        }

        private void EndClimb()
        {
            if (this.Climbing)
            {
                this.Climbing = false;
                //check if we're at the top, if so give a slight jump
                if (IsPointFacingWall(this.Sprite.Location))
                    mPlatformController.GravityController.CurrentYSpeed = -3f;
            }
        }

        protected override void Update()
        {
            var climbKeyPressed = mPlayer.Input.KeyPressed(KCButton.Special);

            if (Climbing && mPlatformController.CheckWallJump())
            {
                this.Climbing = false;
                return;
            }

            if (!Climbing && CanStartClimbing() && climbKeyPressed)
                ClimbStep();
            else if (Climbing)
            {
                if (!CanContinueClimbing())
                    EndClimb();
                else if(climbKeyPressed)
                    ClimbStep();
            }

            //if (!IsPointFacingWall(this.Sprite.Location.Offset(0, -16)))
            //{
            //    if (this.Climbing)
            //    {
            //        this.Climbing = false;
            //        //check if we're at the top, if so give a slight jump
            //        if (IsPointFacingWall(this.Sprite.Location))                 
            //            mPlatformController.GravityController.CurrentYSpeed = -3f;                    
            //    }
            //}
            //else if ()
            //{
                
            
            //}
  
        }

        private bool IsPointFacingWall(RGPointI pt)
        {
            var tile = this.Context.CurrentWorld.CollisionInfo.GetTile(pt);

            var wallTile = tile.GetTilesInLine(this.Sprite.Direction).Take(2).FirstOrDefault(p => p.TileDef.IsSolid);
            if (wallTile == null)
                return false;

            var wallPoint = wallTile.TileArea.GetClosestSurfacePoint(pt);
            if (!wallPoint.HasValue)
                return false;

            mCurrentWallX = wallPoint.Value.X;
            return wallPoint.Value.GetDistanceTo(pt) <= 10;
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (Climbing && cEvent.OtherType.Is(ObjectType.Block) && cEvent.CollisionSide == Engine.Collision.Side.Bottom)
            {
                if(Sprite.MotionManager.Vector.MotionOffset.Y >= 0)
                    this.Climbing = false;
            }
        }

        public bool CanJump()
        {
            return this.Climbing;
        }

    }

}
