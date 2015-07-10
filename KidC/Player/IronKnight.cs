﻿using System;
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
        private TileLayer mLayer;

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
            mLayer = this.Sprite.DrawLayer as TileLayer;
        }

        protected override void Update()
        {        
            if (Climbing && mPlatformController.CheckWallJump())
            {
                this.Climbing = false;
                return;
            }

            if (!IsPointFacingWall(this.Sprite.Location.Offset(0, -16)))
            {
                if (this.Climbing)
                {
                    this.Climbing = false;
                    //check if we're at the top, if so give a slight jump
                    if (IsPointFacingWall(this.Sprite.Location))                 
                        mPlatformController.GravityController.CurrentYSpeed = -3f;                    
                }
            }
            else if (mPlayer.Input.KeyPressed(KCButton.Special))
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
          
//            if (Climbing)
  //              this.Sprite.Location = new RGPoint(mCurrentWallX, this.Sprite.Y);          
        }

        private bool IsPointFacingWall(RGPointI pt)
        {
            if (mLayer == null)
                return false;

            var tile = mLayer.Map.GetTileAtLocation(pt);

            var wallTile = tile.GetTilesInLine(this.Sprite.Direction).Take(2).FirstOrDefault(p => p.TileDef.IsSolid);
            if (wallTile == null)
                return false;

            var wallPoint = wallTile.TileArea.GetClosestSurfacePoint(pt);
            if (!wallPoint.HasValue)
                return false;

            mCurrentWallX = wallPoint.Value.X;
            return wallPoint.Value.GetDistanceTo(pt) < 20;
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
