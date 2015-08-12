using Newtonsoft.Json;
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

        //[Browsable(false)]
        //[JsonIgnore]
        //public Map Map
        //{
        //    get
        //    {
        //        return Maps.NeverNull().FirstOrDefault();
        //    }
        //    set
        //    {
        //        if(Maps == null)
        //            Maps = new Map[] { value };
        //    }
        //}

        [Browsable(false)]
        public Map[] Maps { get; set; }

        public string Name { get; set; }
        public int ScreensWidth { get; set; }
        public int ScreensHeight { get; set; }


        public RGColor BackgroundColor { get; set; }

        public System.Drawing.Color SColor { get; set; }

        public virtual World CreateWorld(GameContext context) { throw new NotImplementedException(); }

        public GameResource<TileSet> TilesetResource
        {
            get
            {
                return new GameResource<TileSet>(new GamePath(PathType.Tilesets, this.TilesetName));
            }
        }

        public WorldInfo() 
        {
            this.Objects = new List<ObjectEntry>();
        }

        public Map[] UpdateMap(GameContext context)
        {
            if (ScreensWidth <= 0)
                ScreensWidth = 1;

            if (ScreensHeight <= 0)
                ScreensHeight = 1;

            List<Map> newMaps = new List<Map>();

            foreach (var map in Maps.NeverNull())
            {
                var newMap = new Map(context, this.TilesetResource, ScreensWidth * 20, ScreensHeight * 15);

                for (int y = 0; y < map.TileDimensions.Height; y++)
                {
                    for (int x = 0; x < map.TileDimensions.Width; x++)
                    {
                        var tileToCopy = map.GetTileAtGridCoordinates(x, y);
                        newMap.SetTile(x, y, tileToCopy);
                    }
                }

                newMaps.Add(newMap);
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

            this.Maps = newMaps.ToArray();
            return this.Maps;
        }

    }
}
