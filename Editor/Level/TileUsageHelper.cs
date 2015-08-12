using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Drawing;
using Engine.Collision;
using System.IO;
using Editor.Forms;

namespace Editor
{
    class TileUsageHelper
    {

        public static void LogMatchInfo(TileInstance tile)
        {

            frmLog.AddLine("Tile #" + tile.TileDef.TileID);
            frmLog.AddLine(tile.TileDef.Usage.Groups.StringJoin(","));
          
            foreach (var side in Util.GetEnumValues<Side>())
            {
                if (side == Side.None)
                    continue;

                 var adjacent = tile.GetAdjacentTile(side.ToOffset());

                var matches = tile.TileDef.Usage.GetMatches(side).Contains(adjacent.TileDef, new TileDefEqualityComparer());
                var matches2 = adjacent.TileDef.Usage.GetMatches(side.Opposite()).Contains(tile.TileDef,new TileDefEqualityComparer());

                if (matches && matches2)
                    frmLog.AddLine("Matches both ways with tile on " + side.ToString());
                else if(matches)
                    frmLog.AddLine("Matches with tile on " + side.ToString());
                else if(matches2)
                    frmLog.AddLine("Tile on " + side.ToString() + " matches this");
                else
                    frmLog.AddLine("Does not match on " + side.ToString());


                frmLog.AddLine("---------------");
            }

        }

        #region Auto Detect
        public static void AutoDetectMatches(TileSet tileset)
        {
            var image = tileset.Texture.GetImage();

            foreach (var tile in tileset.GetTiles())
            {
                foreach (Side side in Util.GetEnumValues<Side>())
                    tile.Usage.AddMatches(side, GetSuggestedMatches(tile, tileset, side, image));
            }
        }

        public static IEnumerable<TileDef> GetSuggestedMatches(TileDef tile, TileSet tileset, Side matchSide, Bitmap tilesetImage)
        {
            var tiles = tileset.GetTiles();

            //remove any tile that does not share at least one group
            tiles = tiles.Where(p => p.Equals(tile) || p.Usage.Groups.ContainsAny(tile.Usage.Groups));

            tiles = tiles.Where(p => p.Equals(tile) || CheckPixelMatch(tile, p, matchSide, tilesetImage));

            return tiles;
        }

        private static bool CheckPixelMatch(TileDef tile, TileDef other, Side matchSide, Bitmap tilesetImage)
        {
            var thisTilePixels = GetEdgeColors(matchSide, tile.SourcePosition, tilesetImage);
            var otherTilePixels = GetEdgeColors(matchSide.Opposite(), other.SourcePosition, tilesetImage);

            int matchCount = 0;
            for (int i = 0; i < thisTilePixels.Length; i++)
            {
                var thisColor = thisTilePixels[i];
                var otherColor = otherTilePixels[i];

                if (thisColor.IsTransparent || otherColor.IsTransparent)
                {
                    if (thisColor.IsTransparent && otherColor.IsTransparent)
                        matchCount++;
                }
                else
                {
                    if (otherTilePixels.Contains(thisColor))
                        matchCount++;
                }
            }

            return matchCount > thisTilePixels.Length * .75;
        }

        private static RGColor[] GetEdgeColors(Side side, RGRectangleI area, Bitmap tileImage)
        {
            List<RGColor> colors = new List<RGColor>();
            if (area.IsEmpty)
                return colors.ToArray();

            int xMin = 0, xMax = 0, yMin = 0, yMax = 0;

            switch (side)
            {
                case Side.Top:
                    xMin = area.Left; xMax = area.Right - 1;
                    yMin = area.Top; yMax = area.Top;
                    break;
                case Side.Bottom:
                    xMin = area.Left; xMax = area.Right - 1;
                    yMin = area.Bottom - 1; yMax = area.Bottom - 1;
                    break;
                case Side.Left:
                    xMin = area.Left; xMax = area.Left;
                    yMin = area.Top; yMax = area.Bottom - 1;
                    break;
                case Side.Right:
                    xMin = area.Right - 1; xMax = area.Right - 1;
                    yMin = area.Top; yMax = area.Bottom - 1;
                    break;
            }

            for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    colors.Add(tileImage.GetPixel(x, y).ToRGColor());

            return colors.ToArray();

        }
        #endregion

        public static TileDef GetNextTileReplacement(TileInstance tile, IEnumerable<TileDef> possibleTiles)
        {
            possibleTiles = GetPossibleTiles(tile, new TileInstance[] { }, new TileInstance[] { },possibleTiles);
            possibleTiles = possibleTiles.OrderBy(p => p.TileID).ToArray();

            return possibleTiles.FirstOrDefault(p => p.TileID > tile.TileDef.TileID) ?? possibleTiles.FirstOrDefault();
        }

        public static TileDef GetNextTileReplacement(TileInstance tile, IEnumerable<TileDef> possibleTiles, TileInstance matchTile)
        {
            possibleTiles = GetPossibleTiles(tile, new TileInstance[] { }, new TileInstance[] { matchTile }, possibleTiles);
            possibleTiles = possibleTiles.OrderBy(p => p.TileID).ToArray();

            return possibleTiles.FirstOrDefault(p => p.TileID > tile.TileDef.TileID) ?? possibleTiles.FirstOrDefault();
        }

        public static IEnumerable<TileInstance> RandomizeAddedTiles(IEnumerable<TileInstance> addedTiles, IEnumerable<TileDef> possibleTiles)
        {
            if (addedTiles.IsNullOrEmpty())
                return addedTiles.NeverNull();

            var tilesRemaining = new LinkedList<TileInstance>(addedTiles);
            var output = new List<TileInstance>();

            output.Add(addedTiles.First());
            tilesRemaining.RemoveFirst();

            var unmatched = new List<TileInstance>();
            while (tilesRemaining.First != null)
            {
                var tile = tilesRemaining.First();
                tilesRemaining.RemoveFirst();
                var newTile = RandomizeTile(tile, tilesRemaining, addedTiles, possibleTiles,true);
                
                if (newTile != null)
                    output.Add(newTile);
                else
                    unmatched.Add(tile);
            }

            return output;
        }

        private static TileInstance RandomizeTile(TileInstance tile, IEnumerable<TileInstance> tilesToIgnore, IEnumerable<TileInstance> tilesToConsider, IEnumerable<TileDef> possibleTiles, bool mustMatchGroups)
        {
            possibleTiles = GetPossibleTiles(tile, tilesToIgnore,tilesToConsider, possibleTiles);

            if(mustMatchGroups)
                possibleTiles = possibleTiles.Where(p => {
                    
                  var g1 = p.Usage.Groups;
                    var g2 = tile.TileDef.Usage.Groups;
                    return g1.ContainsAll(g2) && g2.ContainsAll(g1);
                }).ToArray();

            if(!possibleTiles.IsNullOrEmpty())
            {
                var chosen = possibleTiles.RandomItem();
                tile.Map.SetTile(tile.TileLocation.X, tile.TileLocation.Y, chosen.TileID);
                return tile.Map.GetTileAtGridCoordinates(tile.TileLocation.X, tile.TileLocation.Y);     
            }

            return null;

       
        }

        private static IEnumerable<TileDef> GetPossibleTiles(TileInstance tile, 
            IEnumerable<TileInstance> tilesToIgnore,
            IEnumerable<TileInstance> tilesToConsider,            
            IEnumerable<TileDef> possibleTiles)
        {
            possibleTiles = possibleTiles.Where(p => !p.IsBlank);
            foreach (var side in Util.GetEnumValues<Side>())
            {
                if (side == Side.None)
                    continue;

                var adjacentTile = tile.GetAdjacentTile(side.ToOffset());
                if (tilesToIgnore.Contains(adjacentTile))
                    continue;

                if (!tilesToConsider.IsNullOrEmpty() && !tilesToConsider.Contains(adjacentTile))
                    continue;

                var validTilesForSide = adjacentTile.TileDef.Usage.GetMatches(side.Opposite());
                if (!validTilesForSide.IsNullOrEmpty())
                    possibleTiles = possibleTiles.Intersect(validTilesForSide, new TileDefEqualityComparer()).ToArray();

                //now check the tiles own side matches
                possibleTiles = possibleTiles.Where(p =>
                {
                    var sideMatches = p.Usage.GetMatches(side);
                    return sideMatches.IsNullOrEmpty() ||
                        sideMatches.Contains(adjacentTile.TileDef, new TileDefEqualityComparer());
                }).ToArray();
            }

            var output = possibleTiles.ToArray();

            frmLog.AddLine("Found " + output.Length + " possible matches");
            return output;
        }

        public static void AddTileMatches(TileSet tileset, IEnumerable<TileInstance> tiles)
        {
            foreach (var tile in tiles)
            {
                foreach (var side in Util.GetEnumValues<Side>())
                {
                    if (side == Side.None)
                        continue;

                    var adjacentTile = tile.GetAdjacentTile(side.ToOffset());
                    if (tiles.Contains(adjacentTile))
                    {
                        tile.TileDef.Usage.AddMatch(side, adjacentTile.TileDef);
                        adjacentTile.TileDef.Usage.AddMatch(side.Opposite(), tile.TileDef);
                    }
                }
            }
        }

        public static void RemoveTileMatches(TileSet tileset, IEnumerable<TileInstance> tiles)
        {
            foreach (var tile in tiles)
            {
                foreach (var side in Util.GetEnumValues<Side>())
                {
                    if (side == Side.None)
                        continue;

                    var adjacentTile = tile.GetAdjacentTile(side.ToOffset());
                    if (tiles.Contains(adjacentTile))
                    {
                        tile.TileDef.Usage.RemoveMatch(side, adjacentTile.TileDef);
                        adjacentTile.TileDef.Usage.RemoveMatch(side.Opposite(), tile.TileDef);
                    }
                }
            }
        }

        #region Extract from Images

        public static void BuildMatchesFromFolder(string folder, string tilesetName)
        {

            var tilesetResource = new GameResource<TileSet>(tilesetName, PathType.Tilesets);
            var tileset = tilesetResource.GetObject(Program.EditorContext);

            foreach (var tile in tileset.GetTiles())
                tile.Usage.ClearMatches();


            foreach (var mapPath in Directory.GetFiles(folder, "*.png"))
            {
                var map = ImageToMap.CreateMapFromImage(mapPath, tilesetResource);
                AddMatchesFromMap(tileset, map);
            }

            FileDialog.SaveObject(tileset, tilesetResource.Path.FullPath);
        }

        private static void AddMatchesFromMap(TileSet tileset, Map map)
        {
            for(int x =0; x < map.TileDimensions.Width; x++)
                for (int y = 0; y < map.TileDimensions.Height; y++)
                {
                    var tile = map.GetTileAtGridCoordinates(x,y);
                    if (tile.IsSpecial)
                        continue;

                    foreach(var side in Util.GetEnumValues<Side>())
                    {
                        if(side == Side.None)
                            continue;

                        var adjacentTile = tile.GetAdjacentTile(side.ToOffset());
                        if(!adjacentTile.IsSpecial)
                            tile.TileDef.Usage.AddMatch(side, adjacentTile.TileDef);
                    }
                }
        }

        #endregion 

        public static void RandomizeNeighbors(IEnumerable<TileInstance> changedTiles, IEnumerable<TileDef> possibleTiles)
        {
            var neighbors = GetMismatchedEdgeTiles(changedTiles);
            if (neighbors.IsNullOrEmpty())
                return;

            foreach (var tile in neighbors)            
                RandomizeTile(tile, new List<TileInstance>(), new List<TileInstance>(), possibleTiles,false);

        }

        /// <summary>
        /// Returns all tiles that neighbor the given list of tiles that are not included in that list and which 
        /// do not match with their neighbor
        /// </summary>
        /// <param name="changedTiles"></param>
        /// <returns></returns>
        private static IEnumerable<TileInstance> GetMismatchedEdgeTiles(IEnumerable<TileInstance> changedTiles)
        {
            if(changedTiles.IsNullOrEmpty())
                return changedTiles;


            var adjacent = new List<TileInstance>();
            foreach (var tile in changedTiles)
                adjacent.AddRangeDistinct(tile.GetAdjacentTiles(),new TileEqualityComparer());

            adjacent = adjacent.Except(changedTiles, new TileEqualityComparer()).ToList();
            adjacent = adjacent.Where(t => !TileUsageHelper.DoesTileMatch(t)).ToList();

            return adjacent;
        }


        private static bool DoesTileMatch(TileInstance tile)
        {
            foreach (var side in Util.GetEnumValues<Side>())
            {
                if (side == Side.None)
                    continue;

                var adjacent = tile.GetAdjacentTile(side.ToOffset());

                var matches = tile.TileDef.Usage.GetMatches(side).Contains(adjacent.TileDef, new TileDefEqualityComparer());
                if(!matches)
                    return false;
            }

            return true;
        }


        public static void MakeEqual(IEnumerable<TileDef> tiles)
        {
            if (tiles.IsNullOrEmpty())
                return;

            var first = tiles.FirstOrDefault();
            foreach (var tile in tiles.Skip(1))
            {
                foreach (var side in Util.GetEnumValues<Side>())
                {
                    if (side == Side.None)
                        continue;

                    first.Usage.AddMatches(side, tile.Usage.GetMatches(side));
                }
            }

            foreach (var tile in tiles)
            {
                foreach (var side in Util.GetEnumValues<Side>())
                {
                    if (side == Side.None)
                        continue;

                    tile.Usage.AddMatches(side, first.Usage.GetMatches(side));
                    foreach (var match in tile.Usage.GetMatches(side))
                        match.Usage.AddMatch(side.Opposite(), tile);
                }
            }
        }

    }
}
