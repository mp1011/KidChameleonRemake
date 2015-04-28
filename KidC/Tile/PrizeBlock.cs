using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;

namespace KidC
{
    class PrizeBlock : CollidingTile, IBreakableTile 
    {
        private ObjectType mPrize;
        private KCTileInstance mInstance;

        public PrizeBlock(KCTileInstance tile, TileLayer layer, ObjectType prize)
            : base(tile, layer) 
        {
            mPrize = prize;
            mInstance = tile;
        }

        public override void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            if (collision.OtherType.Is(KCObjectType.IronKnight))
                response.AddInteraction(new IronKnightHitsBrick(), this);
            else if (collision.OtherType.Is(KCObjectType.RedStealth))
                response.AddInteraction(new RedStealthHitsBrick(), this);

            if (mInstance.ShouldBreak(collision))
            {
                this.Break();
            }
        }

        public void Break()
        {
            SoundManager.PlaySound(Sounds.BlockHit);
            BreakingPrizeTile tile = new BreakingPrizeTile(TileLayer.Map.Tileset.TileSize, Tile.TileLocation, TileLayer, mPrize);

            this.TileLayer.AddObject(tile);               
        }
    }

    class BreakingPrizeTile : SimpleObject
    {

        private TileLayer mLayer;
        private RGPointI mTileLocation;

        private ObjectType mPrize;
        private int mInitialY;
        private int mMotion = 2;
        private int mRange = 2;

        public BreakingPrizeTile(RGSizeI tileSize, RGPointI tileLocation, TileLayer layer, ObjectType prize)
            : base(layer, BreakingPrizeTile.CreateGraphic(layer))
        {
            this.Location = new RGPointI((tileLocation.X * tileSize.Width) + (tileSize.Width / 2), (tileLocation.Y * tileSize.Height) + (tileSize.Height / 2));
            this.mLayer = layer;
            this.mPrize = prize;

            mTileLocation = tileLocation;
            mInitialY = this.Location.Y;           
        }

        private static SimpleGraphic CreateGraphic(TileLayer layer)
        {
            var tileDef = layer.Map.Tileset.GetTiles().FirstOrDefault(p => p.TileID == (int)SpecialTile.Rock + 1000);
            return new SimpleGraphic(new TextureResource("SpriteSheets_Woods"), tileDef.SourcePosition);
        }

        protected override void OnEntrance()
        {
            mLayer.Map.SetTile(mTileLocation.X, mTileLocation.Y, TileDef.BlankSolid.TileID);
        }
       
        protected override void Update()
        {
            this.Location = this.Location.Offset(0, mMotion);

            if (this.Location.Y > mInitialY + mRange)
                mMotion = -1 * Math.Abs(mMotion);
            else if (this.Location.Y < mInitialY - mRange)
                mMotion = Math.Abs(mMotion);


            if (this.Age > 30)
            {
                this.Kill(Engine.ExitCode.Removed);

                var tileDef = mLayer.Map.Tileset.GetTiles().FirstOrDefault(p => p.TileID == (int)SpecialTile.Rock + 1000);
                mLayer.Map.SetTile(mTileLocation.X, mTileLocation.Y, tileDef.TileID);

                var t = mLayer.Map.GetTile(mTileLocation.X, mTileLocation.Y);

                var puff = KCObjectType.Puff.CreateSpriteInstance(mLayer, this.Context).Sprite;
                puff.Location = this.Location.Offset(0, -24);

                new CreateObjectWhenDestroyed(puff, mPrize, RGPoint.Empty);
            }
        }


    }

}
