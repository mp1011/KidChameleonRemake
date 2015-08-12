using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class ImageToMap
    {
        class GridPortion
        {
            public TileDef TileDef { get; set; }
            public BitmapPortion Image { get; set; }
        }
        
        public void Test()
        {
            var bmp = new BitmapPortion(@"D:\Games\Emulation\Genesis\gensKMod_07\Screenshots\woods\7.png", true);
            var tileset = new GameResource<TileSet>("woods", PathType.Tilesets);
            var map = CreateMapFromImage(bmp, tileset);

            var worldInfo = Program.EditorGame.WorldInfoCreate();

            worldInfo.Name = "extracttest";
            worldInfo.Maps = new Map[] { map };

            var path = new GamePath(PathType.Maps,worldInfo.Name);
            FileDialog.SaveObject(worldInfo, path.FullPath);

        }

        public static Map CreateMapFromImage(string path, GameResource<TileSet> tilesetResource)
        {
            var bmp = new BitmapPortion(path, true);
            var map = CreateMapFromImage(bmp, tilesetResource);
            return map;
        }

        private static IEnumerable<GridPortion> TilesetToBitmapPortions(TileSet tileset)
        {
            var tilesetImage = new BitmapPortion(tileset.Texture.GetImage());
            foreach (var tile in tileset.GetTiles())
            {
                if (tile.IsBlank)
                    continue;

                var image = tilesetImage.Extract(tile.SourcePosition);
                if (IsMostlyTransparent(image))
                    continue;

                yield return new GridPortion
                {
                    TileDef = tile,
                    Image = image
                };
            }
        }

        private static bool IsMostlyTransparent(BitmapPortion image)
        {
            int numBlank = 0;
            int numFilled = 0;

            for (int x = image.Region.X; x < image.Region.Right; x++)
            {
                for (int y = image.Region.Y; y < image.Region.Bottom; y++)
                {
                    if (image.GetPixel(x, y).IsTransparent())
                        numBlank++;
                    else
                        numFilled++;
                }
            }

            if(numFilled == 0)
                return true;
            return numBlank >= (numFilled + numBlank) * .9f;
        }

        private static Map CreateMapFromImage(BitmapPortion image, GameResource<TileSet> tilesetResource)
        {
            var tileset = tilesetResource.GetObject(Program.EditorContext);
            var gridPortions = TilesetToBitmapPortions(tileset).ToArray();

            var origin = DetermineOriginPoint(image, gridPortions, tileset);
            var grid = image.ExtractGrid(origin, tileset.TileSize.Width).ToArray();

            var mapSize = GetMapSize(grid, origin);
            var map = new Map(Program.EditorContext, tilesetResource, mapSize.Width, mapSize.Height);

            foreach (var tile in grid)
            {
                var gridPt = GetGridCoordinatesFromImage(tile,origin);
                var tileDef = FindTileFromImage(tile, gridPortions, tileset);
                map.SetTile(gridPt.X, gridPt.Y, tileDef.TileID);
            }


            return map;
        }

        private static RGSizeI GetMapSize(IEnumerable<BitmapPortion> tile, RGPointI origin)
        {
            var size = new RGSizeI();
            size.Width = tile.Max(p => GetGridCoordinatesFromImage(p,origin).X)+1;
            size.Height = tile.Max(p => GetGridCoordinatesFromImage(p,origin).Y) + 1;
            return size;
        }

        private static RGPointI GetGridCoordinatesFromImage(BitmapPortion image, RGPointI origin)
        {
            var mapPoint = image.Region.TopLeft.Difference(origin);
            return mapPoint.Scale(1f / image.Region.Width);
        }

        private static TileDef FindTileFromImage(BitmapPortion image, IEnumerable<GridPortion> tilesetImage, TileSet tileset)
        {
            var comparer = new PixelEqualityComparer();
            var tile = tilesetImage.FirstOrDefault(p => comparer.Equals(p.Image, image));
            if (tile == null)
                return TileDef.Blank;
            else
                return tile.TileDef;
        }

        private static RGPointI DetermineOriginPoint(BitmapPortion image, IEnumerable<GridPortion> tiles, TileSet tileset)
        {

            var start = RGPointI.Empty;

            while (start.X < image.Width/2f && image.GetPixel(start.X, start.Y).IsTransparent())
                start.X++;

            while (start.Y < image.Height && image.GetPixel(start.X, start.Y).IsTransparent())
                start.Y++;

            var comparer = new PixelEqualityComparer();
            for(int x = start.X; x < start.X + tileset.TileSize.Width*2;x++)
                for (int y = start.Y; y < start.Y + tileset.TileSize.Height * 2; y++)
                {
                    var possibleTile = image.Extract(RGRectangleI.FromXYWH(x,y,tileset.TileSize.Width,tileset.TileSize.Height));
                    var matchTile = tiles.FirstOrDefault(p => comparer.Equals(p.Image, possibleTile));
                    if(matchTile != null)
                        start = new RGPointI(x, y);
                }


            while (start.X > tileset.TileSize.Width)
                start.X -= tileset.TileSize.Width;

            while (start.Y > tileset.TileSize.Height)
                start.Y -= tileset.TileSize.Height;

            return start;
        }

    }
}
