using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Graphics;

namespace Engine
{
    public class WorldID
    {
        public string Name { get; private set; }
        public int Number { get; private set; }

        public WorldID SubWorld { get; private set; }

        public WorldID(string name, int number, WorldID sub)
        {
            Name = name;
            Number = number;
            SubWorld = sub;
        }

        public WorldID(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public WorldID(int number)
        {
            Number = number;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(Name))
            {
                sb.Append(Name);
                sb.Append(" ");
            }
            sb.Append(Number);
            if (SubWorld != null)
                sb.Append(SubWorld.ToString());

            return sb.ToString();
        }
    }

    public partial class World : IDrawable
    {
        private List<Layer> mLayers = new List<Layer>();
        private List<ObjectEntry> mEntries = new List<ObjectEntry>();

        public GameContext Context { get; private set; }

        public Layer ScreenLayer { get; private set; }

        public RGRectangle Area { get; private set; }

        public RGColor BackgroundColor { get; set; }

        public World()
        {
        }

        public World(GameContext context)
        {
            Context = context;
            ScreenLayer = new FixedLayer(context, LayerDepth.Screen);
            AddLayer(ScreenLayer);
        }

        public void SetObjectEntries(IEnumerable<ObjectEntry> entries)
        {
            mEntries = new List<ObjectEntry>(entries);
            //            mEntries.AddRange(entries.ToList());
        }      

        public Layer AddLayer(Layer layer)
        {
            mLayers.Add(layer);
            mLayers = mLayers.OrderBy(p => p.Depth).ToList();

            layer.SetLayerID(this);

            var foregroundLayers = mLayers.Where(p => p.Depth == LayerDepth.Foreground).ToArray();
            if(foregroundLayers.Length > 0)
                Area = RGRectangle.FromTLBR(foregroundLayers.Min(p => p.Location.Top), foregroundLayers.Min(p => p.Location.Left), foregroundLayers.Max(p => p.Location.Bottom), foregroundLayers.Max(p => p.Location.Right));
            else
                Area = RGRectangle.FromTLBR(mLayers.Min(p => p.Location.Top), mLayers.Min(p => p.Location.Left), mLayers.Max(p => p.Location.Bottom), mLayers.Max(p => p.Location.Right));

            return layer;
        }


        public int GetNextFreeLayerID()
        {
            return mLayers.Select(p => p.LayerID).Max() + 1;
        }

        public IEnumerable<Layer> GetLayers(LayerDepth depth)
        {
            return mLayers.Where(p => p.Depth == depth);
        }


        public Layer GetLayerByID(int id)
        {
            return mLayers.Single(p => p.LayerID == id);
        }

        public void Draw(Painter painter, RGRectangleI canvas)
        {
            foreach (var layer in mLayers)
                layer.Draw(painter, canvas);
        }

        public TileInstance GetTopmostTile(RGPointI worldLocation, Predicate<TileInstance> condition)
        {
            foreach (var layer in mLayers.OrderByDescending(p => p.Depth))
            {
                TileLayer tileLayer = layer as TileLayer;
                if (tileLayer == null)
                    continue;

                var tile = tileLayer.Map.GetTileAtLocation(worldLocation);
                if (condition(tile))
                    return tile;
            }

            return null;
        }

    }

}
