using Engine;
using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{

    class MushroomBlock : KCCollidingTile 
    {
        enum MushroomState
        {
            Solid,
            Growing,
            Shrinking
        }

        protected override SpecialTile? FinalTile
        {
            get
            {
                if (mState == MushroomState.Shrinking)
                    return null;
                else
                    return SpecialTile.Mushroom;
            }
        }

        private SimpleAnimation mAnimation;
        private MushroomState mState = MushroomState.Solid;

        public MushroomBlock(KCTileInstance tile, TileCollisionView collisionView)
            : base(tile, collisionView) 
        {
            mAnimation = KidCGraphic.Mushroom.CreateSimpleAnimation(this.Context);
        }

        protected override SoundResource HitSound
        {
            get { return Sounds.None; }
        }

        protected override bool ShouldInteract(CollisionEvent collision, CollisionResponse response)
        {
            return collision == null || collision.CollisionSide == Side.Bottom;
        }

        protected override void OnEntrance()
        {
            if (mState == MushroomState.Solid)
            {
                var aboveLeft = this.Tile.GetAdjacentTile(-1, -1) as KCTileInstance;
                var above = this.Tile.GetAdjacentTile(0, -1) as KCTileInstance;
                var aboveRight = this.Tile.GetAdjacentTile(1, -1) as KCTileInstance;

                if (!GrowBlockIfEmpty(aboveLeft) & !GrowBlockIfEmpty(above) & !GrowBlockIfEmpty(aboveRight))
                {
                    mAnimation = KidCGraphic.MushroomShrink.CreateSimpleAnimation(this.Context);
                    mState = MushroomState.Shrinking;
                }
            }

            base.OnEntrance();
        }

        private bool GrowBlockIfEmpty(KCTileInstance tile)
        {
            if (tile.TileDef.IsBlank)
            {
                var cv = new TileCollisionView();
                cv.Instance = tile;
                cv.Layer = this.CollisionView.Layer;
                cv.CollisionInfo = this.CollisionView.CollisionInfo;
                var block = new MushroomBlock(tile, cv);
                block.mState = MushroomState.Growing;
                block.mAnimation = KidCGraphic.MushroomGrow.CreateSimpleAnimation(this.Context);
                this.TileLayer.AddObject(block);
                return true;
            }
            else
                return false;
        }

        protected override void Update()
        {
            if (mState == MushroomState.Solid)
                this.Kill(ExitCode.Finished);
            else if (this.Age > 30)
                this.Kill(Engine.ExitCode.Finished);
        }
        

        protected override void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            if (mAnimation == null)
                return;

            mAnimation.Location = this.Location;
            mAnimation.Draw(p, canvas);
        }
    }
}
