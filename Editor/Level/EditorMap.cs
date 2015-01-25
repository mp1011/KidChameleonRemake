using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Editor
{
    class MapInfo
    {

        public string TilesetName { get; set; }

        public int ScreensWidth { get; set; }

        public int ScreensHeight { get; set; }

        public Map CreateMap(Map original)
        {
            if (ScreensWidth <= 0)
                ScreensWidth = 1;

            if (ScreensHeight <= 0)
                ScreensHeight = 1;

            var newMap = new Map(Program.EditorContext, new GameResource<TileSet>(new GamePath(PathType.Tilesets, this.TilesetName)), ScreensWidth * 20, ScreensHeight * 15);

            if (original != null)
            {
                for (int y = 0; y < original.TileDimensions.Height; y++)
                {
                    for (int x = 0; x < original.TileDimensions.Width; x++)
                    {
                        newMap.SetTile(x, y, original.GetTile(x, y).TileID);
                    }
                }
            }
            return newMap;
        }
    }
}
