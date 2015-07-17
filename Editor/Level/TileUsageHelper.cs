using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Drawing;

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

                var theseGroups = tile.Usage.SideGroups.TryGet(groupSide, new string[] { });
                var otherGroups = adjacentTile.TileDef.Usage.SideGroups.TryGet(groupSide.GetAdjacentSide(), new string[] { });

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

        public static void AutoDetectSides(TileDef keyTile, IEnumerable<TileDef> tileDefs, Bitmap tileImage)
        {
            var colorMap = new TileGroupAutodetectInfo(keyTile, tileImage);

            foreach (var tile in tileDefs)
                colorMap.TryFill(tile, tileImage);            
        }

        class TileGroupAutodetectInfo
        {
            public string Group { get; private set; }
            public RGColor[] Colors { get; private set; }

            public TileGroupAutodetectInfo(TileDef input, Bitmap tileImage)
            {
                this.Group = input.Usage.DistinctGroupNames.FirstOrDefault().NotNull();
                List<RGColor> edgeColors = new List<RGColor>();
                
                foreach(GroupSide side in Enum.GetValues(typeof(GroupSide)))
                    edgeColors.AddRange(GetEdgeColors(side, input.SourcePosition, tileImage));


                Colors = edgeColors.Distinct().ToArray();
            }


            private RGColor[] GetEdgeColors(GroupSide side, RGRectangleI area, Bitmap tileImage)
            {
                List<RGColor> colors= new List<RGColor>();
                if (area.IsEmpty)
                    return colors.ToArray();

                int xMin = 0, xMax = 0, yMin = 0, yMax = 0;

                switch (side)
                {
                    case GroupSide.TopLeft:
                        xMin = area.Left; xMax = area.Center.X;
                        yMin = area.Top; yMax=area.Top;
                        break;
                    case GroupSide.TopRight:
                        xMin = area.Center.X; xMax = area.Right-1;
                        yMin = area.Top; yMax = area.Top;
                        break;
                    case GroupSide.BottomLeft:
                        xMin = area.Left; xMax = area.Center.X;
                        yMin = area.Bottom-1; yMax = area.Bottom-1;
                        break;
                    case GroupSide.BottomRight:
                        xMin = area.Center.X; xMax = area.Right-1;
                        yMin = area.Bottom-1; yMax = area.Bottom-1;
                        break;
                    case GroupSide.LeftTop:
                        xMin = area.Left;xMax=area.Left;
                        yMin = area.Top; yMax = area.Center.Y;
                        break;
                    case GroupSide.LeftBottom:
                        xMin = area.Left; xMax = area.Left;
                        yMin = area.Center.Y; yMax = area.Bottom-1;
                        break;
                    case GroupSide.RightTop:
                        xMin = area.Right-1; xMax = area.Right-1;
                        yMin = area.Top; yMax = area.Center.Y;
                        break;
                    case GroupSide.RightBottom:
                        xMin = area.Right-1; xMax = area.Right-1;
                        yMin = area.Center.Y; yMax = area.Bottom-1;
                        break;
                }

                for (int x = xMin; x <= xMax; x++)
                    for (int y = yMin; y <= yMax; y++)
                        colors.AddDistinct(tileImage.GetPixel(x, y).ToRGColor());

                return colors.OrderBy(p=>p.GetHashCode()).ToArray();

            }


            public void TryFill(TileDef other, Bitmap tileImage)
            {
                foreach (GroupSide side in Enum.GetValues(typeof(GroupSide)))
                {
                    var sideColors = GetEdgeColors(side, other.SourcePosition, tileImage);

                    int matchCount = sideColors.Where(p => this.Colors.Contains(p)).Count();
                    if (matchCount >= 2)
                    {
                        var currentSides = other.Usage.SideGroups.TryGet(side, new string[]{}).ToList();
                        currentSides.AddDistinct(this.Group);
                        other.Usage.SideGroups.AddOrSet(side, currentSides.ToArray());
                    }
                }
            }
        
        }


        public static TileSet AutoOrganize(TileSet set)
        {
            var specialTiles = set.GetTiles().Where(p => p.IsSpecial).ToArray();
            var allGroupNames = set.GetTiles().SelectMany(p => p.Usage.DistinctGroupNames).Distinct().ToArray();
            var oldOrder = set.GetTiles().ToList();
            var newOrder = new List<TileDef>();

            oldOrder.Transfer(newOrder, specialTiles);

            string[] groups = oldOrder.Select(p => p.Usage.DistinctGroupNames.StringJoin(",")).Distinct().ToArray();

            foreach (var group in groups)
            {
                oldOrder.Transfer(newOrder, oldOrder.Where(p => p.Usage.DistinctGroupNames.StringJoin(",").Equals(group)));
            }

            oldOrder.Transfer(newOrder, oldOrder);
            return  new TileSet(set.Texture, set.TileSize, newOrder);

        }


    }
}
