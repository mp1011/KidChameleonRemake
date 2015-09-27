using Engine;
using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class MetalBlock : KCCollidingTile
    {
        private SimpleAnimation mAnimation;
        private bool mRestoredTile = false;

        protected override SpecialTile? FinalTile
        {
            get
            {
                return SpecialTile.Metal;
            }
        }

        public MetalBlock(KCTileInstance tile, TileCollisionView collisionView) : base(tile,collisionView) 
        {
            mAnimation = KidCGraphic.MetalBlockShine.CreateSimpleAnimation(this.Context);
        }

        protected override SoundResource HitSound
        {
            get { return Sounds.MetalBlockPing; }
        }

        protected override void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mAnimation.Location = this.Location;
            mAnimation.Draw(p, canvas);
        }

        protected override bool ShouldInteract(CollisionEvent collision, CollisionResponse response)
        {
            return collision.CollisionSide == Side.Bottom;
        }

        protected override void Update()
        {
            var initialLocation = this.Tile.TileArea.Center;
            if (this.Age < 10)
                this.Location = this.Location.Offset(0, -1);
            else if (this.Age < 20 && this.Location.Y < initialLocation.Y)
                this.Location = this.Location.Offset(0, 1);
            else
            {
                if (!mRestoredTile)
                {
                    var tileDef = TileLayer.Map.Tileset.GetSpecialTile(SpecialTile.Metal);
                    this.Tile.ReplaceWith(tileDef);
                    mRestoredTile = true;
                }
                else
                {
                    //if we already replaced the tile, and it is blank again, the player has hit the tile again and a new bouncing block would have been spawned
                    if (TileLayer.Map.GetTile(Tile.TileLocation.X, Tile.TileLocation.Y).TileID == TileDef.BlankSolid.TileID)
                        this.Kill(Engine.ExitCode.Cancelled);
                }
            }

            if (this.Age > 60)
            {
                this.Kill(Engine.ExitCode.Removed);
            }
        }

    }
}
