using Engine;
using Engine.Collision;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    abstract class KCCollidingTile : CollidingTile, IDrawableRemovable
    {
        public KCTileInstance TileInstance { get; private set; }
      
        public KCCollidingTile(KCTileInstance tile, TileCollisionView collisionView)
            : base(tile, collisionView)
        {
            TileInstance = tile;
        }

        public override void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            if (!this.ShouldInteract(collision, response))
                this.Kill(Engine.ExitCode.Cancelled);
        }    

       
       
        protected virtual SpecialTile? FinalTile { get { return null; } }
        protected abstract SoundResource HitSound { get; }

        protected override void OnEntrance()
        {
            SoundManager.PlaySound(this.HitSound);
            this.TileLayer.AddObject(this);
            this.TileInstance.ReplaceWith(TileDef.BlankSolid);
        }
        
        protected virtual bool ShouldInteract(CollisionEvent collision, CollisionResponse response)
        {
            return false;
        }

        protected override void OnExit()
        {
            if (this.ExitCode == Engine.ExitCode.Cancelled)
                return;

            if (!FinalTile.HasValue)
            {
                 Tile.ReplaceWith(TileDef.Blank);
            }
            else
            {
                var finalTile = this.TileLayer.Map.Tileset.GetSpecialTile(FinalTile.Value);
                Tile.ReplaceWith(finalTile);
            }
        }

        
        protected abstract void Draw(Engine.Graphics.Painter p, RGRectangleI canvas);

        void IDrawable.Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            this.Draw(p, canvas);
        }
    }
}
