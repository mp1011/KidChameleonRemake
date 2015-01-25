using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Editor
{
    class TileUsageHelper
    {
        private IEnumerable<TileDef> GetPossibleReplacements(TileInstance tile)
        {
            var tileAbove = tile.GetAdjacentTile(0, -1).TileDef;
            var tileRight = tile.GetAdjacentTile(1, 0).TileDef;
            var tileLeft = tile.GetAdjacentTile(-1, 0).TileDef;
            var tileBelow = tile.GetAdjacentTile(0, 1).TileDef;

            var possibleTiles = tile.Map.Tileset.GetTiles().Where(p => p.Usage.RandomUsageWeight > 0);

            //if tile is sloped, match direction
            if (tile.TileDef.IsSloped)
                possibleTiles = possibleTiles.Where(p => p.IsSloped && p.Sides.Equals(tile.TileDef.Sides));

            //filter by solid/passable
            possibleTiles = possibleTiles.Where(p => p.IsSolid == tile.TileDef.IsSolid);

            //limit to the tiles that can fit next to the adjacent tiles
            possibleTiles = possibleTiles.Where(t =>
                {
                    return CanTileFitWith(t,tileAbove, tileRight, tileBelow, tileLeft);
                });

            return possibleTiles;
        }

        private bool CanTileFitWith(TileDef tile, TileDef tileAbove, TileDef tileRight, TileDef tileBelow, TileDef tileLeft)
        {
            var ua = tile.Usage;

            var above = tileAbove.Usage;
            var below = tileBelow.Usage;
            var left = tileLeft.Usage;
            var right = tileRight.Usage;

           
            var result=
                GroupsMatch(ua.TopLeftGroup, above.BottomLeftGroup) &&
                GroupsMatch(ua.TopRightGroup, above.BottomRightGroup) &&
                GroupsMatch(ua.RightTopGroup, right.LeftTopGroup) &&
                GroupsMatch(ua.RightBottomGroup, right.LeftBottomGroup) &&
                GroupsMatch(ua.BottomRightGroup, below.TopRightGroup) &&
                GroupsMatch(ua.BottomLeftGroup, below.TopLeftGroup) &&
                GroupsMatch(ua.LeftBottomGroup, left.RightBottomGroup) &&
                GroupsMatch(ua.LeftTopGroup, left.RightTopGroup);

            if(result && tile.Usage.SideGroups.Any(p=>p.Contains("empty")))
                Console.WriteLine("Y");

            return result;
        }

        private bool GroupsMatch(string g1, string g2)
        {
            if (g1 == null || g2 == null)
                return g1 == null && g2 == null;

            var g1a = g1.Split(',');
            var g2a = g2.Split(',');

            return g1a.Any(p => g2a.Contains(p)) || g2a.Any(p=> g1a.Contains(p));
        }

        public void RandomizeTile(TileInstance tile)
        {
            var chosenTile = GetPossibleReplacements(tile).RandomItem(p => p.Usage.RandomUsageWeight);
            tile.ReplaceWith(chosenTile);
        }
    }
}
