using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Engine
{
    public class WorldInfo 
    {
        protected virtual string TilesetName { get { throw new NotImplementedException(); } }

        [Browsable(false)]
        public List<ObjectEntry> Objects { get; set; }

        [Browsable(false)]
        public Map Map { get; set; }

        public string Name { get; set; }
        public int ScreensWidth { get; set; }
        public int ScreensHeight { get; set; }


        public RGColor BackgroundColor { get; set; }

        public System.Drawing.Color SColor { get; set; }

        public virtual World CreateWorld(GameContext context) { throw new NotImplementedException(); }

        public WorldInfo() 
        {
            this.Objects = new List<ObjectEntry>();
        }

        public Map UpdateMap(GameContext context)
        {
            if (ScreensWidth <= 0)
                ScreensWidth = 1;

            if (ScreensHeight <= 0)
                ScreensHeight = 1;

            var newMap = new Map(context, new GameResource<TileSet>(new GamePath(PathType.Tilesets, this.TilesetName)), ScreensWidth * 20, ScreensHeight * 15);

            if (this.Map != null)
            {
                for (int y = 0; y < this.Map.TileDimensions.Height; y++)
                {
                    for (int x = 0; x < this.Map.TileDimensions.Width; x++)
                    {
                        var tileToCopy = this.Map.GetTileAtCoordinates(x, y);
                        newMap.SetTile(x, y, tileToCopy);
                    }
                }
            }

            if (this.Objects == null)
                this.Objects = new List<ObjectEntry>();

            var fakeLayer = new FixedLayer(new World(context, this), LayerDepth.Foreground);
          
            foreach (var o in this.Objects)
            {
                o.CreateObject(fakeLayer);
                if(o.PlacedObject != null)
                    o.PlacedObject.Location = o.Location;
            }

            this.Map = newMap;
            return newMap;
        }

    }
}
