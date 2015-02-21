using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{

    class KidController : PlatformerPlayerController
    {
        public KidController(Sprite sprite, Player player) : base(sprite, player)
        {                     
        }
    }

    class KidFlipController : TriggeredController<Direction>
    {

        private Player mPlayer;
        private TileInstance mFlipBlock;
        private TileLayer mLayer;
        protected override bool AllowRetrigger { get { return false; } }

        public KidFlipController(Sprite s, Player player) : base(s) 
        {
            mPlayer = player;
        }

        protected override Switch OnTriggered(Direction state)
        {
            this.Sprite.CurrentAnimationKey = KCAnimation.Flip;
            this.Sprite.CurrentAnimation.Reset();

            SoundManager.PlaySound(Sounds.KidFlip);
            return Switch.On;
        }

        protected override Switch OnTriggerUpdate(Direction flipDir)
        {
            this.Sprite.CurrentAnimationKey = KCAnimation.Flip;
            
            if (flipDir == Direction.Left)
                this.Sprite.Location = new RGPointI(mFlipBlock.TileArea.Left, mFlipBlock.TileArea.Top);
            if (flipDir == Direction.Right)
                this.Sprite.Location = new RGPointI(mFlipBlock.TileArea.Right, mFlipBlock.TileArea.Top);

            this.Sprite.MotionManager.StopMotionInDirection(new DirectionFlags(Direction.Down));
            if (this.Sprite.CurrentAnimation.Finished)           
                return Switch.Off;            
            else
                return Switch.On;
        }

        protected override void OnEntrance()
        {
            mLayer = Sprite.DrawLayer as TileLayer;
            base.OnEntrance();
        }

        protected override void Update()
        {
            if (CheckForFlip())
            {

                if (this.Sprite.X < mFlipBlock.TileArea.Center.X)
                    this.Trigger(Direction.Left);
                if (this.Sprite.X > mFlipBlock.TileArea.Center.X)
                    this.Trigger(Direction.Right);           
            }

            base.Update();
        }

      
        private bool CheckForFlip()
        {
            if (mLayer == null)
                return false;

            if (this.Sprite.IsOnGround(mLayer))
                 return false;

            if (!mPlayer.Input.KeyPressed(KCButton.Jump))
                return false;

            var flipDir = mPlayer.Input.InputDirection(Orientation.Horizontal);
            if (!flipDir.HasValue)
                return false;
        
            var blockPoint = this.Sprite.Location;

            blockPoint = blockPoint.Offset(0, -8);
            if (flipDir == Direction.Left)
                blockPoint = new RGPointI(this.Sprite.Area.Left - 2, blockPoint.Y);
            else if (flipDir == Direction.Right)
                blockPoint = new RGPointI(this.Sprite.Area.Right + 2, blockPoint.Y);

            var tile = mLayer.Map.GetTileAtLocation(blockPoint);
            if (tile.TileDef.IsPassable)
                return false;

            if (tile.GetAdjacentTile(0, -1).TileDef.IsPassable)
            {
                mFlipBlock = tile;               
                return true;
            }

            return false;

        }

    }
}
