using Engine;
using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class MetalBlock : CollidingTile
    {
      //  private KCTileInstance mInstance;
     
        public MetalBlock(TileInstance tile, TileLayer layer)
            : base(tile, layer) 
        {
           //mInstance = tile as KCTileInstance;
        }

        public override void HandleCollision(CollisionEvent collision, CollisionResponse collisionResponse)
        {
            if (collision.CollisionSide == Side.Bottom)
            {
                SoundManager.PlaySound(Sounds.MetalBlockPing);
                var bouncingTile = new BouncingMetalBlock(TileLayer.Map.Tileset.TileSize, Tile.TileLocation, TileLayer);
                this.TileLayer.AddObject(bouncingTile);     
            }
        }        
    }

    class BouncingMetalBlock  : LogicObject, IDrawableRemovable, IWithPosition 
    {
        private TileLayer mLayer;
        private RGPointI mTileLocation;

        private RGPointI mInitialLocation;
        private bool mRestoredTile = false;

        private SimpleAnimation mAnimation;

        public BouncingMetalBlock(RGSizeI tileSize, RGPointI tileLocation, TileLayer layer) :base(LogicPriority.Behavior, layer) 
        {
            this.Location = new RGPointI((tileLocation.X * tileSize.Width) + (tileSize.Width / 2), (tileLocation.Y * tileSize.Height) + (tileSize.Height / 2));
            this.mLayer = layer;
        
            mTileLocation = tileLocation;
            mInitialLocation = this.Location;
            mAnimation = new SimpleAnimation("metalblock", 4, layer.Context, 0, 1, 2, 3, 4,4,4,4,4,4);
            
            layer.AddObject(this);
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

            if (this.Age < 10)
                this.Location = this.Location.Offset(0, -1);
            else if (this.Age < 20 && this.Location.Y < mInitialLocation.Y)
                this.Location = this.Location.Offset(0, 1);
            else
            {
                if (!mRestoredTile)
                {
                    var tileDef = mLayer.Map.Tileset.GetTiles().FirstOrDefault(p => p.TileID == (int)SpecialTile.Metal + 1000);
                    mLayer.Map.SetTile(mTileLocation.X, mTileLocation.Y, tileDef.TileID);
                    mRestoredTile = true;
                }
                else
                {
                    //if we already replaced the tile, and it is blank again, the player has hit the tile again and a new bouncing block would have been spawned
                    if (mLayer.Map.GetTile(mTileLocation.X, mTileLocation.Y).TileID == TileDef.BlankSolid.TileID)
                        this.Kill(Engine.ExitCode.Removed);
                }

                this.Location = mInitialLocation;
            }

            if (this.Age > 60)
            {
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
}
