using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Editor
{
    class TileMatchInfo
    {
        public TileDef[] PossibleTiles { get; private set; }
        public TileInstance[] TilesToMatch { get; private set; }

        public TileMatchInfo(IEnumerable<TileDef> possibleTiles, IEnumerable<TileInstance> tilesToMatch)
        {
            this.PossibleTiles = possibleTiles.NeverNull().ToArray();
            this.TilesToMatch = tilesToMatch.NeverNull().ToArray();
        }

        public TileMatchInfo()
        {
            PossibleTiles = new TileDef[] { };
            TilesToMatch = new TileInstance[] { };
        }
    
    }

    class TileUsageHelper
    {
       

        /// <summary>
        /// Returns the next tile that is able to fit with the current adjacent tiles
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static TileDef GetNextTileReplacement(TileInstance tile)
        {
            return GetNextTileReplacement(tile, null);
        }

        /// <summary>
        /// Returns the next tile that is able to fit with the current adjacent tiles
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static TileDef GetNextTileReplacement(TileInstance tile, TileMatchInfo matchInfo)
        {
            return GetTilesStartingAt(tile).FirstOrDefault(p => CanFitTile(p, tile, matchInfo));
        }

        public static TileDef GetRandomTileReplacement(TileInstance tile, TileMatchInfo matchInfo)
        {
            return GetTilesStartingAt(tile).Where(p => CanFitTile(p, tile, matchInfo)).RandomItem();
        }

        private static IEnumerable<TileDef> GetTilesStartingAt(TileInstance tile)
        {
            foreach(var nextTile in tile.Map.Tileset.GetTiles().SkipWhile(p=>p != tile.TileDef).Skip(1))
                yield return nextTile;

            foreach (var nextTile in tile.Map.Tileset.GetTiles().TakeWhile(p => p != tile.TileDef))
                yield return nextTile;
        }

        private static bool CanFitTile(TileDef tile, TileInstance location, TileMatchInfo matchInfo)
        {
            if (matchInfo.PossibleTiles.Count() > 1 && !matchInfo.PossibleTiles.Contains(tile))
                return false;

            bool matchesAny = false;

            foreach (var groupSide in Util.GetEnumValues<GroupSide>())
            {
                var offset = groupSide.ToOffset();
                var adjacentTile = location.GetAdjacentTile(offset.X, offset.Y);

            //    if(matchInfo.TilesToMatch.NotNullOrEmpty() && !matchInfo.TilesToMatch.Any(p=>p.Equals(adjacentTile)))
             //       continue;

                var theseGroups = tile.Usage.SideGroups.TryGet(groupSide, "").Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
                var otherGroups = adjacentTile.TileDef.Usage.SideGroups.TryGet(groupSide.GetAdjacentSide(), "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //special case, blank tile has "empty" on all sides
                if (adjacentTile.TileDef.IsBlank)
                {
                    otherGroups = new string[] { "empty" };
                   // continue;
                }

                if (adjacentTile.TileDef.IsOutOfBounds)
                    continue;

                if (theseGroups.Length == 0)
                    theseGroups = new string[] { "empty" };

                if (otherGroups.Length == 0)
                    otherGroups = new string[] { "empty" };

                if (DoGroupsMatch(theseGroups, otherGroups))
                {
                    if (!theseGroups.IsNullOrEmpty())
                        matchesAny = true;
                }
                else
                    return false;
                
            }

            return matchesAny;
        }


        private static bool DoGroupsMatch(string[] group1, string[] group2)
        {
            if (group1.IsNullOrEmpty() && group2.IsNullOrEmpty())
                return true;


            foreach(var g1 in group1)
                foreach (var g2 in group2)
                {
                    if (DoGroupsMatch(g1, g2))
                        return true;
                }

            return false;
        }

        private static bool DoGroupsMatch(string group1, string group2)
        {
            if(group1.Equals(group2))
                return true;

            if (group1 == "*")
                return true;

            return false;
                               
        }


        public static IEnumerable<TileInstance> RandomizeTiles(Map map, TileMatchInfo matchInfo)
        {
            var matches = matchInfo.TilesToMatch.Select(p => new TileMatch(p, matchInfo)).ToArray();

            foreach (var m in matches)
            {
                if (m.PossibleMatches.IsNullOrEmpty())
                    continue;

                var tile = m.PossibleMatches.RandomItem();
                var loc = m.Instance.TileLocation;
                map.SetTile(loc.X, loc.Y, tile.TileID);
                yield return map.GetTileAtCoordinates(loc.X, loc.Y);
            }
        }

        /// <summary>
        /// Gets any tiles adjacent to the given ones that are not already in the list
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        private static IEnumerable<TileInstance> GetAdjacentTiles(IEnumerable<TileInstance> tiles)
        {
            var adjacentTiles = tiles.SelectMany(t =>
            {
                return new TileInstance[]{t.GetAdjacentTile(-1,0),t.GetAdjacentTile(1,0),t.GetAdjacentTile(0,-1),t.GetAdjacentTile(0,1)};
            });

            adjacentTiles = adjacentTiles.Distinct(new TileEqualityComparer()).Where(p=> ! tiles.Contains(p));
            return adjacentTiles;
        }


        class TileMatch
        {
            public TileInstance Instance;
            public List<TileDef> PossibleMatches;
            public TileDef Original;

            public TileMatch(TileInstance instance, TileMatchInfo info)
            {
                Original = instance.TileDef;
                this.Instance = instance;
                this.PossibleMatches = TileUsageHelper.GetTilesStartingAt(instance)
                    .Where(p => CanFitTile(p, instance, info)).ToList();
            }
        }

    }
}
