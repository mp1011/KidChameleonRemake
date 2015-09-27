using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Collision;
using Engine.Input;
namespace KidC
{



    class PlatformerPlayerController : SpriteBehavior, IDamagerxxx
    {
        private TimedAction<GravityController> mJump;
        private float mJumpSpeed;
        private ulong mJumpBeginFrame;
        private bool mIsOnGround = false;

        public TransformationStats MotionStats
        {
            get
            {
                return TransformationStats.GetStats(this.Sprite);
            }
        }
               
        public Player Player { get; private set;}
        public bool NoAnimationChanges { get; set; }

        public GravityController GravityController
        {
            get;
            private set;
        }

        public PlatformerPlayerController(Sprite sprite, Player player, GravityController gravityController)
            : base(sprite)
        {
            this.Player = player;
            this.Context.SetCameraCenter(this.Sprite);
            this.GravityController = gravityController;
        }

        protected override void OnEntrance()
        {
            Sprite.MotionManager.MainMotion.Accel = WalkOrRunAccel;
            Sprite.MotionManager.MainMotion.Decel = this.Deceleration;
            mJump = new TimedAction<GravityController>(GravityController,a=> a.CurrentYSpeed = -1 * mJumpSpeed);
        }

        protected override void Update()
        {
            if (Player.Input.KeyDown(GameKey.Editor1))
                this.Context.FPS = 10;
            else
                this.Context.FPS = 60;

            HandleOverSlope();
            
            if (!HandleSlope())
            {
                if (!HandleCrawl())                    
                    HandleWalk();                    
            }

            HandleJump();
           
            if (!mIsOnGround)
            {
                if (this.Sprite.MotionManager.MotionOffset.Y <= 0)
                    this.CurrentAnimationKey=KCAnimation.Jump;
                else
                    this.CurrentAnimationKey=KCAnimation.Fall;
            }
          

            mIsOnGround = false;
            mCurrentSlopeDirection = Direction.Down;
        }

       
        private bool HandleCrawl()
        {

            var dir = Player.Input.InputDirection(Orientation.None);

            if (!dir.HasValue)
                return false;

            if (dir == Direction.DownRight || dir == Direction.Down || dir == Direction.DownLeft)
            {
                Sprite.MotionManager.MainMotion.Decel = this.MotionStats.CrawlDecel;
                Sprite.MotionManager.MainMotion.Accel = this.MotionStats.CrawlAccel;
                this.CurrentAnimationKey=KCAnimation.Crawl;
            }
            else
                return false;

            var originalDirection = Sprite.MotionManager.MainMotion.Direction;
            if (dir == Direction.Down)
            {
                Sprite.MotionManager.MainMotion.TargetSpeed = 0f;
            }
            else if (dir == Direction.DownRight)
            {
                Sprite.MotionManager.MainMotion.TargetSpeed = this.MotionStats.CrawlSpeed;
                Sprite.MotionManager.MainMotion.Direction = Direction.Right;
                Sprite.Direction = Direction.Right;
            }
            else if (dir == Direction.DownLeft)
            {
                Sprite.MotionManager.MainMotion.TargetSpeed = this.MotionStats.CrawlSpeed;
                Sprite.MotionManager.MainMotion.Direction = Direction.Left;
                Sprite.Direction = Direction.Left;
            }

            if (originalDirection != Sprite.MotionManager.MainMotion.Direction)
                Sprite.MotionManager.MainMotion.CurrentSpeed = 0;

            Sprite.CurrentAnimation.SpeedModifier = Sprite.MotionManager.MainMotion.Percentage;
            return true;
        }

        private void HandleJump()
        {
          
            if (Player.Input.KeyPressed(KCButton.Jump) && PlayerCanJump())
            {
                DoJump();
            }

            if (Player.Input.KeyReleased(KCButton.Jump))
                mJump.Duration = this.MotionStats.ShortJumpDuration;
        }

        public bool CheckWallJump()
        {
            if (this.Player.Input.KeyPressed(KCButton.Jump))
            {
                DoJump();
                return true;
            }
            return false;
        }

        private void DoJump()
        {
            SoundManager.PlaySound(Sounds.Jump);
            mJumpBeginFrame = Context.CurrentFrameNumber;

            mJumpSpeed = this.MotionStats.JumpSpeed + this.Sprite.MotionManager.MainMotion.CurrentSpeed * this.MotionStats.RunJumpSpeedMod;
            mJump.Start(this.MotionStats.LongJumpDuration);
        }

        public void DoBounce(float speed)
        {
            mJumpBeginFrame = Context.CurrentFrameNumber;
            mJumpSpeed = speed;

            if (Player.Input.KeyDown(KCButton.Jump))
                mJump.Start(this.MotionStats.LongJumpDuration*2);
            else
                mJump.Start(this.MotionStats.ShortJumpDuration);
        }


        private bool PlayerCanJump()
        {
            return this.mIsOnGround && mJumpBeginFrame == 0;
        }

        private void HandleWalk()
        {
            var player = this.Context.FirstPlayer;
            Sprite.MotionManager.MainMotion.Decel = Deceleration;

            Sprite.MotionManager.MainMotion.Accel = WalkOrRunAccel;
            var dir = player.Input.InputDirection(Orientation.Horizontal);
            if (dir == null)
            {
                Sprite.MotionManager.MainMotion.TargetSpeed = 0;
            }
            else
            {
                Sprite.Direction = dir.Value;
                Sprite.MotionManager.MainMotion.TargetSpeed = WalkOrRunSpeed;              
                Sprite.MotionManager.MainMotion.ChangeDirectionAndPreserveVector(dir.Value);
            }

            if (!mIsOnGround)
                return;

            if (this.Sprite.MotionManager.MotionOffset.X == 0)
                this.Sprite.SetAnimation(KCAnimation.Stand);
            else
            {
                var mo = this.Sprite.MotionManager.Vector.GetMagnitudeInDirection(this.Sprite.Direction);
                if (mo < 0)
                {
                    this.Sprite.SetAnimation(KCAnimation.Turn);

                    if (mIsOnGround)
                        Sprite.MotionManager.MainMotion.Accel = this.MotionStats.TurnAccel;
                    else
                        Sprite.MotionManager.MainMotion.Accel = this.MotionStats.AirTurnAccel;

                    if (this.Sprite.CurrentAnimationKey != KCAnimation.Turn)
                        this.Sprite.SetAnimation(KCAnimation.Walk);
                }
                else
                    this.Sprite.SetAnimation(KCAnimation.Walk);

                if(this.Sprite.CurrentAnimationKey == KCAnimation.Walk)
                    this.Sprite.CurrentAnimation.SpeedModifier = this.Sprite.MotionManager.Vector.Magnitude / this.MotionStats.WalkSpeed;
            }

        }

        protected override void HandleCollisionEx(CollisionEvent cEvent, CollisionResponse response)
        {

            if (cEvent.OtherType.Is(ObjectType.Block))
            {
                if ((cEvent.CollisionSide == Side.Left && this.Sprite.CurrentSpeed.X < 0) ||
                    (cEvent.CollisionSide == Side.Right && this.Sprite.CurrentSpeed.X > 0))
                {
                    this.Sprite.MotionManager.StopMotionInDirection(DirectionFlags.Horizontal);
                }


                if (response.CorrectionVector.Y < 0 || cEvent.CollisionSide == Side.Bottom)
                {
                    mIsOnGround = true;
                    mJumpBeginFrame = 0;
                }

                if (response.CorrectionVector.Y > 0 || cEvent.CollisionSide == Side.Top)
                {
                    this.Sprite.MotionManager.StopMotionInDirection(new DirectionFlags(Direction.Up));
                    mJumpBeginFrame = 0;
                    mJump.Stop();
                }

                if (cEvent.IsSloped && cEvent.SlopeDirection.Down)
                {
                    if (cEvent.SlopeDirection.Left)
                        mCurrentSlopeDirection = Direction.Left;
                    else if (cEvent.SlopeDirection.Right)
                        mCurrentSlopeDirection = Direction.Right;
                }
            }
        }

        private Direction mCurrentSlopeDirection = Direction.Down;

        private void HandleOverSlope()
        {
            if (Sprite.MotionManager.Vector.MotionOffset.Y < 0)
                return;

            var layer = this.Sprite.DrawLayer as TileLayer;
            if (layer == null)
                return;

            var tile = layer.Map.GetTileAtLocation(this.Sprite.Location);

            var groundTile = tile.GetTilesInLine(0, 1).FirstOrDefault(p => p.TileDef.IsSloped);
            if (groundTile == null)
                return;

            int? yIntercept = groundTile.TileDef.GetYIntercept(groundTile.TileArea, this.Sprite.X);
            if (yIntercept.HasValue)
            {
                if (Math.Abs(yIntercept.Value - this.Sprite.Y) > 16)
                    return;

                this.Sprite.Location = new RGPointI(this.Sprite.X, yIntercept.Value);
                this.mIsOnGround = true;

                if (groundTile.TileDef.Sides.Left)
                    mCurrentSlopeDirection = Direction.Left;
                else if (groundTile.TileDef.Sides.Right)
                    mCurrentSlopeDirection = Direction.Right;
            }
            
        }

        private bool mCanClimbSlopes = true;
        protected bool CanClimbSlopes
        {
            get { return mCanClimbSlopes; }
            set { mCanClimbSlopes = value; }
        }

        private bool HandleSlope()
        {
 
            if (!mIsOnGround)
            {
                mCurrentSlopeDirection = Direction.Down;
                return false;
            }

            if (mCurrentSlopeDirection == Direction.Down)
                return false;

            var player = this.Context.FirstPlayer;
            var dir = player.Input.InputDirection(Orientation.Horizontal);
            Sprite.MotionManager.MainMotion.Decel = 1f;

            //no movement, player falls down hill
            if (dir == null)
            {
                Sprite.SetAnimation(KCAnimation.ClimbDown);
                Sprite.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);
                Sprite.MotionManager.MainMotion.TargetSpeed = WalkOrRunSpeed + this.MotionStats.UpHillSpeedMod;
                Sprite.MotionManager.MainMotion.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);//.RotateTowards(Direction.Down, 45);
           
                //test Sprite.MotionManager.MainMotion.TargetSpeed =0;
            }
            else if (dir == mCurrentSlopeDirection && this.CanClimbSlopes) // moving up the slope
            {
                Sprite.SetAnimation(KCAnimation.ClimbUp);
                Sprite.Direction = mCurrentSlopeDirection;
                Sprite.MotionManager.MainMotion.TargetSpeed = WalkOrRunSpeed + this.MotionStats.UpHillSpeedMod;
                Sprite.MotionManager.MainMotion.Direction = mCurrentSlopeDirection;//.RotateTowards(Direction.Up, 45);
            }
            else //moving down the slope
            {
                Sprite.SetAnimation(KCAnimation.ClimbDown);
                Sprite.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);
                Sprite.MotionManager.MainMotion.TargetSpeed = WalkOrRunSpeed + this.MotionStats.DownHillSpeedMod;
                Sprite.MotionManager.MainMotion.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);//.RotateTowards(Direction.Down, 45);
            }

            this.Sprite.CurrentAnimation.SpeedModifier = this.Sprite.MotionManager.MainMotion.Percentage;

            return true;

        }

        public bool OnIce { get; set; }

        private float IceMod
        {
            get
            {
                if (OnIce)
                    return this.MotionStats.IceMod;
                else
                    return 1f;
            }
        }

        private float Deceleration
        {
            get
            {
                if (mIsOnGround)
                    return this.MotionStats.StopAccel * IceMod;
                else
                    return this.MotionStats.AirDecel;
            }
        }

        private float WalkOrRunSpeed
        {
            get
            {
                if (Player.Input.KeyDown(KCButton.Run))
                    return this.MotionStats.RunSpeed;
                else
                    return this.MotionStats.WalkSpeed;
            }
        }

        private float WalkOrRunAccel
        {
            get
            {
                if (Player.Input.KeyDown(KCButton.Run))
                    return this.MotionStats.RunAccel * IceMod;
                else
                    return this.MotionStats.WalkAccel * IceMod;
            }
        }

        private int CurrentAnimationKey
        {
            get
            {
                return this.Sprite.CurrentAnimationKey;
            }
            set
            {
                if (!this.NoAnimationChanges)
                    this.Sprite.SetAnimation(value, this,false);
            }
        }

        public int GetAttackDamage(CollisionEvent collision)
        {
            return 1;
        }
    }

}
