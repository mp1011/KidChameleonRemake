using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;

namespace KidC
{
    class BreakableTile : CollidingTile, IBreakableTile 
    {
        private KCTileInstance mInstance;

        public BreakableTile(TileInstance tile, TileLayer layer) : base(tile, layer) 
        {
            mInstance = tile as KCTileInstance;
        }

        public override void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            if (collision.OtherType.Is(KCObjectType.IronKnight))
                response.AddInteraction(new IronKnightHitsBrick(), this);
            else if (collision.OtherType.Is(KCObjectType.RedStealth))
                response.AddInteraction(new RedStealthHitsBrick(), this);

            if (mInstance.ShouldBreak(collision))            
                Break();
        }

        public void Break() 
        {
            SoundManager.PlaySound(Sounds.RockBlockDestroyed);
            BreakingTile tile = new BreakingTile(this.Context, TileLayer.Map.Tileset.TileSize, Tile.TileLocation, TileLayer);

            this.TileLayer.AddObject(tile);            
        }

        
    }

    class BreakingTile : SimpleObject
    {

        private TileLayer mLayer;
        private RGPointI mTileLocation;

        public BreakingTile(GameContext ctx, RGSizeI tileSize, RGPointI tileLocation, TileLayer layer)
            : base(ctx, BreakingTile.CreateGraphic(ctx))
        {
            this.Location = new RGPoint((tileLocation.X * tileSize.Width) + (tileSize.Width /2), (tileLocation.Y * tileSize.Height) + (tileSize.Height /2));
            this.mLayer = layer;
            this.mTileLocation = tileLocation;
        }

        private static SimpleGraphic CreateGraphic(GameContext ctx)
        {
            return new SimpleGraphic(SpriteSheet.Load("rockblock", ctx),0);
        }

        protected override void OnEntrance()
        {
            mLayer.Map.SetTile(mTileLocation.X, mTileLocation.Y, TileDef.Blank.TileID);
        }

        protected override void Update()
        {
            if (this.Age > 2)
            {
                this.Kill(Engine.ExitCode.Removed);

                var loc = this.Location;
                FlyingDebris.Create(loc, mLayer, Direction.Left, 1f, -4, "rockblock", 1);
                FlyingDebris.Create(loc, mLayer, Direction.Right, 1f, -4, "rockblock", 1);
                FlyingDebris.Create(loc, mLayer, Direction.Left, 1.5f, -1, "rockblock", 1);
                FlyingDebris.Create(loc, mLayer, Direction.Right, 1.5f, -1, "rockblock", 1);
            }
        }


    }


}
