using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Collision;
using Engine.Input;
namespace KidC
{

    public class PlayerMotionStats : Stats
    {
        public float WalkSpeed { get; set; }
        public float WalkAccel { get; set; }

        public float RunSpeed { get; set; }
        public float RunAccel { get; set; }

        public float StopAccel { get; set; }
        public float AirDecel { get; set; }
        public float TurnAccel { get; set; }
        public float AirTurnAccel { get; set; }


        public float CrawlSpeed { get; set; }
        public float CrawlAccel { get; set; }
        public float CrawlDecel { get; set; }

        public float UpHillSpeedMod { get; set; }
        public float DownHillSpeedMod { get; set; }

        public float JumpSpeed { get; set; }
        public float RunJumpSpeedMod { get; set; }

        public ulong LongJumpDuration { get; set; }
        public ulong ShortJumpDuration { get; set; }

    }


    public class PlatformerPlayerController : SpriteBehavior, IDamager
    {
        private TimedAction<GravityController> mJump;
        private float mJumpSpeed;
        private GravityController mGravityController;
        private ulong mJumpBeginFrame;
        private bool mIsOnGround = false;

        private GameResource<PlayerMotionStats> mStatsResource;
        private PlayerMotionStats MotionStats { get { return mStatsResource.GetObject(this.Context); } }
               
        public Player Player { get; private set;}
        public bool NoAnimationChanges { get; set; }

        public GravityController GravityController
        {
            get
            {
                return mGravityController;
            }
        }

        public PlatformerPlayerController(Sprite sprite, Player player)
            : base(sprite)
        {
            this.Player = player;
            this.Context.SetCameraCenter(this.Sprite);

            mStatsResource = new DevelopmentResource<PlayerMotionStats>(new GamePath(PathType.Stats, this.Sprite.ObjectType.ToString() + "_stats")); 
        }

        protected override void OnEntrance()
        {
            Sprite.MotionManager.MainMotion.Accel = WalkOrRunAccel;
            Sprite.MotionManager.MainMotion.Decel = this.MotionStats.StopAccel;

            mGravityController = Sprite.GetBehavior<GravityController>();
            mJump = new TimedAction<GravityController>(mGravityController,a=> a.CurrentYSpeed = -1 * mJumpSpeed);
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
                    this.CurrentAnimationKey = KCAnimation.Jump;
                else
                    this.CurrentAnimationKey = KCAnimation.Fall;
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
                this.CurrentAnimationKey = KCAnimation.Crawl;
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

        private bool PlayerCanJump()
        {
            return this.mIsOnGround && mJumpBeginFrame == 0;
        }

        private void HandleWalk()
        {
            var player = this.Context.FirstPlayer;

            if(mIsOnGround)
                Sprite.MotionManager.MainMotion.Decel = this.MotionStats.StopAccel;
            else
                Sprite.MotionManager.MainMotion.Decel = this.MotionStats.AirDecel;

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
                Sprite.MotionManager.MainMotion.Direction = dir.Value;
            }

            if (!mIsOnGround)
                return;

            if (this.Sprite.MotionManager.MotionOffset.X == 0)
                this.Sprite.CurrentAnimationKey = KCAnimation.Stand;
            else
            {
                var mo = this.Sprite.MotionManager.Vector.GetMagnitudeInDirection(this.Sprite.Direction);
                if (mo < 0)
                {
                    this.Sprite.CurrentAnimationKey = KCAnimation.Turn;

                    if (mIsOnGround)
                        Sprite.MotionManager.MainMotion.Accel = this.MotionStats.TurnAccel;
                    else
                        Sprite.MotionManager.MainMotion.Accel = this.MotionStats.AirTurnAccel;

                    if (this.Sprite.CurrentAnimationKey != KCAnimation.Turn)
                        this.Sprite.CurrentAnimationKey = KCAnimation.Walk;
                }
                else
                    this.Sprite.CurrentAnimationKey = KCAnimation.Walk;

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
                Sprite.CurrentAnimationKey = KCAnimation.ClimbDown;
                Sprite.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);
                Sprite.MotionManager.MainMotion.TargetSpeed = WalkOrRunSpeed + this.MotionStats.UpHillSpeedMod;
                Sprite.MotionManager.MainMotion.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);//.RotateTowards(Direction.Down, 45);
           
                //test Sprite.MotionManager.MainMotion.TargetSpeed =0;
            }
            else if (dir == mCurrentSlopeDirection && this.CanClimbSlopes) // moving up the slope
            {
                Sprite.CurrentAnimationKey = KCAnimation.ClimbUp;
                Sprite.Direction = mCurrentSlopeDirection;
                Sprite.MotionManager.MainMotion.TargetSpeed = WalkOrRunSpeed + this.MotionStats.UpHillSpeedMod;
                Sprite.MotionManager.MainMotion.Direction = mCurrentSlopeDirection;//.RotateTowards(Direction.Up, 45);
            }
            else //moving down the slope
            {
                Sprite.CurrentAnimationKey = KCAnimation.ClimbDown;
                Sprite.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);
                Sprite.MotionManager.MainMotion.TargetSpeed = WalkOrRunSpeed + this.MotionStats.DownHillSpeedMod;
                Sprite.MotionManager.MainMotion.Direction = mCurrentSlopeDirection.Reflect(Orientation.Horizontal);//.RotateTowards(Direction.Down, 45);
            }

            this.Sprite.CurrentAnimation.SpeedModifier = this.Sprite.MotionManager.MainMotion.Percentage;

            return true;

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
                    return this.MotionStats.RunAccel;
                else
                    return this.MotionStats.WalkAccel;
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
                    this.Sprite.CurrentAnimationKey = value;
            }
        }

        public int GetAttackDamage(CollisionEvent collision)
        {
            return 1;
        }
    }

}
