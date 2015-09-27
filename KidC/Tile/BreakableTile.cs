using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;

namespace KidC
{

    public interface IBreakableTile 
    {
        KCTileInstance TileInstance { get; }
        bool ShouldBreak { get; }
        void Break(CollisionEvent evt);
    }

    static class IBreakableTileExtensions
    {
        public static void HandleBreakabileTileCollision(this IBreakableTile tile,CollisionEvent collision, CollisionResponse response)
        {
          //  response.AddInteraction(new BlockBreakerHitsBlock(), tile);
        }

        public static bool ShouldBreak(this KCTileInstance tile,float collisionX)
        {
            return tile.CanBreak() && collisionX >= tile.TileArea.Left && collisionX < tile.TileArea.Right;
        }

        private static bool CanBreak(this KCTileInstance tile)
        {
            switch (tile.KCTileDef.SpecialType)
            {
                case SpecialTile.Rock:
                case SpecialTile.Prize: 
                case SpecialTile.Ice:
                return true;
                default: return false;
            }
        }

    }
  
}
