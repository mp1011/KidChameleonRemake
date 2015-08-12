using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public class Map : ISerializable  
    {

        public string Name { get; set; }

        private GameResource<TileSet> mTilesetResource;

        private int[,] mTiles;
        private List<TileInstance> mSpecialTiles = new List<TileInstance>();

        private GameContext mContext;
        private TileSet _mTileset;
        public TileSet Tileset { get { return _mTileset ?? (_mTileset = mTilesetResource.GetObject(this.mContext)); } }

        private World World { get { return this.mContext.CurrentWorld; } }

        public Map()
        {
            mContext = Core.EngineBase.Current.Context;
        }

        public Map(GameContext context)
        {
            mContext = context;
        }

        public Map(GameContext context, GameResource<TileSet> tileset, int tilesX, int tilesY)
        {
            mContext = context;
            mTilesetResource = tileset;
            mTiles = new int[tilesX, tilesY];
        }

        public TileInstance SetTile(int x, int y, int tile)
        {
            if (x < 0 || y < 0 || x >= this.TileDimensions.Width || y >= this.TileDimensions.Height)
                return GetTileAtGridCoordinates(x, y);

            mSpecialTiles.RemoveAll(p => p.TileLocation.X == x && p.TileLocation.Y == y);         
            mTiles[x, y] = tile;

            if(mContext.CurrentWorld != null)
                mContext.CurrentWorld.CollisionInfo.OnTileChanged(x, y);

            return GetTileAtGridCoordinates(x, y);
        }

        public void SetTile(int x, int y, TileInstance tile)
        {         
            SetTile(x, y, tile.TileDef.TileID);          
            if(tile.IsSpecial)
                UpdateSpecialInstance(tile);
        }


        public TileDef GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.TileDimensions.Width || y >= this.TileDimensions.Height)
                return TileDef.OutOfBounds;

            return Tileset.GetTile(mTiles[x, y]);
        }

        public TileInstance GetTileAtGridCoordinates(int x, int y)
        {
            var specialTile = mSpecialTiles.FirstOrDefault(p => p.TileLocation.X == x && p.TileLocation.Y == y);
            if (specialTile != null)
                return specialTile;
            
            var instance = this.mContext.Game.TileInstanceCreate();
            instance.TileDef = GetTile(x, y);
            instance.TileLocation = new RGPointI(x, y);
            instance.Map = this;
            return instance;
        }

        public void UpdateSpecialInstance(TileInstance t)
        {
            t.Map = this;
            mSpecialTiles.RemoveAll(p => p.TileLocation.Equals(t.TileLocation));

            if (t.IsSpecial)
                mSpecialTiles.Add(t);                            
        }

        public TileInstance GetTileAtLocation(RGPointI location)
        {
            int x = (int)(location.X / this.Tileset.TileSize.Width);
            int y = (int)(location.Y / this.Tileset.TileSize.Height);

            return GetTileAtGridCoordinates(x, y);
        }

        public RGRectangleI GetTileLocation(int x, int y)
        {
            return Tileset.GetDestRec(x, y);
        }

        public RGSizeI Size { get { return new RGSizeI(mTiles.GetLength(0) * Tileset.TileSize.Width, mTiles.GetLength(1) * Tileset.TileSize.Height); } }
        public RGSizeI TileDimensions { get { return new RGSizeI(mTiles.GetLength(0), mTiles.GetLength(1)); } }       

        public void Draw(Graphics.Painter painter, RGRectangleI canvas, RGPointI layerLocation)
        {
            if (layerLocation.X != 0 || layerLocation.Y != 0)
                throw new NotImplementedException(); // need to offset

            for (int y = 0; y < TileDimensions.Height; y++)
            {
                for (int x = 0; x < TileDimensions.Width; x++)
                {
                    var tileDef = Tileset.GetTile(mTiles[x, y]);
                    if ((tileDef.Flags & TileFlags.Invisible) == 0)
                        painter.Paint(canvas, Tileset.Texture, tileDef.SourcePosition, Tileset.GetDestRec(x, y), RenderOptions.Normal);
                }
            }
        }

        public override int GetHashCode()
        {
            int hc = TileDimensions.Width * TileDimensions.Height;

            for (int y = 0; y < TileDimensions.Height; y++)
            {
                for (int x = 0; x < TileDimensions.Width; x++)
                {
                    hc = unchecked(hc * 314159 + mTiles[x, y]);
                }
            }

            return hc;
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }

        #region Saving

        private class MapSaveModel
        {
            public GameResource<TileSet> TileSet;
            public List<TileInstance> SpecialTiles;
            public int[,] Tiles;
            public string Name;
        }

        public object GetSaveModel()
        {
            return new MapSaveModel { TileSet = this.mTilesetResource, SpecialTiles = mSpecialTiles, Tiles = this.mTiles, Name = this.Name };
        }

        public Type GetSaveModelType()
        {
            return typeof(MapSaveModel);
        }

        public void Load(object saveModel)
        {
            var model = saveModel as MapSaveModel;
            this.mTilesetResource = model.TileSet;
            this.mTiles = model.Tiles;
            this.mSpecialTiles = model.SpecialTiles.NotNull();
            this.Name = model.Name;
            foreach (var tile in mSpecialTiles)
                tile.Map = this;
        }

        #endregion
    }

}
