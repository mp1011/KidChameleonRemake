using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;

namespace KidC
{
    class RubberBlock : CollidingTile
    {
        public RubberBlock(TileInstance tile, TileLayer layer)
            : base(tile, layer) 
        {
         
        }

        public override void HandleCollision(CollisionEvent collision, CollisionResponse collisionResponse)
        {
            collisionResponse.AddInteraction(new PlayerBounce(collision.Invert()), this);

            var bouncingTile = new BouncingRubberBlock(TileLayer.Map.Tileset.TileSize, Tile.TileLocation, TileLayer);
            this.TileLayer.AddObject(bouncingTile);
           // var bouncingTile = new BouncingMetalBlock(TileLayer.Map.Tileset.TileSize, Tile.TileLocation, TileLayer);
          //  this.TileLayer.AddObject(bouncingTile);                 
        }        
    }


    class BouncingRubberBlock : LogicObject, IDrawableRemovable, IWithPosition
    {
        private TileLayer mLayer;
        private RGPointI mTileLocation;
        private SimpleAnimation mAnimation;
        private static KCTileDef BlankRubber;

        public BouncingRubberBlock(RGSizeI tileSize, RGPointI tileLocation, TileLayer layer)
            : base(LogicPriority.Behavior, layer)
        {
            this.Location = new RGPointI((tileLocation.X * tileSize.Width) + (tileSize.Width / 2), (tileLocation.Y * tileSize.Height) + (tileSize.Height / 2));
            this.mLayer = layer;

            mTileLocation = tileLocation;
            mAnimation = new SimpleAnimation("rubberblock", 2, layer.Context, 0, 1);
            layer.AddObject(this);

            if (BlankRubber == null)
            {
                BlankRubber = new KCTileDef(TileDef.BlankSolid);
                BlankRubber.SpecialType = SpecialTile.Rubber;
            }
        }

        protected override void OnEntrance()
        {
            mLayer.Map.SetTile(mTileLocation.X, mTileLocation.Y, TileDef.BlankSolid.TileID);
        }

        protected override void OnExit()
        {

        }

        protected override void Update()
        {
            if (this.Age >= 30)
            {
                var tileDef = mLayer.Map.Tileset.GetTiles().FirstOrDefault(p => p.TileID == (int)SpecialTile.Rubber + 1000);
                mLayer.Map.SetTile(mTileLocation.X, mTileLocation.Y, tileDef.TileID);
                this.Kill(Engine.ExitCode.Removed);
            }
        }

        public RGPointI Location
        {
            get;
            set;
        }

        public RGRectangleI Area
        {
            get { throw new NotImplementedException(); }
        }

        public Direction Direction
        {
            get { throw new NotImplementedException(); }
        }

        public void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mAnimation.Location = this.Location;
            mAnimation.Draw(p, canvas);
        }
    }


    class PlayerBounce : Interaction<PlatformerPlayerController, CollidingTile>
    {
        private CollisionEvent mCollisionEvent;
        private DirectedMotion mHorizontalBounce;
        private GameContext mContext;

        class BounceStats : Stats 
        {
            public float SideBounceDecel { get; set; }
            public float SideBounceSpeed { get; set; }
            public float VerticalBounceStrength { get; set; }
        }
        private GameResource<BounceStats> mBounceStatsResource;

        private BounceStats Stats { get { return mBounceStatsResource.GetObject(this.mContext); } }

        public PlayerBounce(CollisionEvent collisionEvent)
        {
            mCollisionEvent = collisionEvent;
            mBounceStatsResource = new DevelopmentResource<BounceStats>(new GamePath(PathType.Stats, "Bounce"));
            mContext = collisionEvent.OtherObject.Context;
        }

        protected override void DoAction(PlatformerPlayerController controller1, CollidingTile controller2)
        {
            if(mHorizontalBounce == null)
            {
                mHorizontalBounce = new DirectedMotion("Horizontal Bounce");
                mHorizontalBounce.Inactive=true;
                mHorizontalBounce.Decel = this.Stats.SideBounceDecel;
                controller1.Sprite.MotionManager.AddComponent(mHorizontalBounce);
            }

            if (mCollisionEvent.CollisionSide == Side.Bottom)
            {
                var ySpeed = mCollisionEvent.CollisionSpeed.Y;

                if (ySpeed <= 0)
                    return;

                SoundManager.PlaySoundIfNotPlaying(Sounds.RubberBounce);
                controller1.DoBounce((Math.Min(ySpeed, this.Stats.VerticalBounceStrength)));
            }
            else if (mCollisionEvent.CollisionSide == Side.Left)
            {
                SoundManager.PlaySoundIfNotPlaying(Sounds.RubberBounce);
                mHorizontalBounce.Inactive = false;
                mHorizontalBounce.Direction = Direction.Right;
                mHorizontalBounce.TargetSpeed = 0;
                mHorizontalBounce.CurrentSpeed = this.Stats.SideBounceSpeed;
            }
            else if (mCollisionEvent.CollisionSide == Side.Right)
            {
                SoundManager.PlaySoundIfNotPlaying(Sounds.RubberBounce);
                mHorizontalBounce.Inactive = false;
                mHorizontalBounce.Direction = Direction.Left;
                mHorizontalBounce.TargetSpeed = 0;
                mHorizontalBounce.CurrentSpeed = this.Stats.SideBounceSpeed;

            }

        }
    }

    class BounceController : SpriteBehavior
    {
        public PlatformerPlayerController mPlatformerController;

        public BounceController(Sprite s, PlatformerPlayerController platformerController)
            : base(s) 
        {
            this.mPlatformerController = platformerController;
        }

        protected override void HandleCollisionEx(CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(ObjectType.Block))
                response.AddInteraction(new PlayerBounce(cEvent), mPlatformerController);
        }

       

    }
}
