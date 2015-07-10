using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class TileSet : ISerializable
    {
        private TextureResource mTileTexture;
        public RGSizeI TileSize { get; private set; }
        private TileDef[] mTiles;
        public TextureResource Texture { get { return mTileTexture; } }

        public TileSet() 
        {
            mTiles = new TileDef[] { };
        }

        public TileSet(TextureResource tileTexture, RGSizeI tileSize, IEnumerable<TileDef> tiles)
        {
            mTileTexture = tileTexture;
            TileSize = tileSize;
            mTiles = tiles.ToArray();
        }

        public TileSet(SpriteSheet sheet, IEnumerable<TileDef> tiles)
        {
            this.mTileTexture = sheet.Image;
            this.TileSize = sheet.Frames.First().Source.Size;

            int id = 1;
            //mTiles = sheet.Frames.Select(p => new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty,new DirectionFlags(), p.Source)).ToArray();
            mTiles = tiles.ToArray();
        }

        public IEnumerable<TileDef> GetTiles() { return mTiles.AsEnumerable(); }

        public void AddTiles(IEnumerable<TileDef> t)
        {
            var curTiles = mTiles.ToList();
            foreach (var tile in t)
                curTiles.Add(tile);

            mTiles = curTiles.ToArray();
        }

        public void AddTile(TileDef tile)
        {
            var t = mTiles.ToList();
            t.Add(tile);
            mTiles = t.ToArray();
        }
        public TileDef GetTile(int tileID)
        {
            if (tileID == 0)
                return TileDef.Blank;
            else if (tileID == -1)
                return TileDef.BlankSolid;
            else if (tileID >= mTiles.Length)
            {
                return mTiles.FirstOrDefault(p => p.TileID == tileID) ?? TileDef.Blank;
            }

            return mTiles[tileID - 1];
        }



        public RGRectangleI GetDestRec(int tileX, int tileY)
        {
            return RGRectangleI.FromXYWH(tileX * TileSize.Width, tileY * TileSize.Height, this.TileSize.Width, this.TileSize.Height);
        }

        public void UpdateAnimation(GameContext ctx)
        {
            foreach (var tile in mTiles)
                tile.UpdateAnimation(ctx);
        }


        public static TileSet CreateBase()
        {
            var ts = new TileSet();

            var sheet = new SpriteSheet(new TextureResource("blocks"));
            ts.mTileTexture = sheet.Image;

            var size = 16;

            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 8; x++)
                    sheet.AddFrame(RGRectangleI.FromXYWH(x * size, y * size, size, size), new RGPointI(x * size, y * size));

            ts.TileSize = new RGSizeI(size, size);

            var tiles = new List<TileDef>();
            var id = 1;
            var index = 2;

            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.UpLeft), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Up), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.UpRight), sheet.Frames[index++].Source));

            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.UpLeft), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Up), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.UpRight), sheet.Frames[index++].Source));

            tiles.Add(new TileDef(TileFlags.Sloped, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.UpRight), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Sloped, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.UpLeft), sheet.Frames[index++].Source));

            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Left), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Right), sheet.Frames[index++].Source));

            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Left), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Right), sheet.Frames[index++].Source));

            tiles.Add(new TileDef(TileFlags.Sloped, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.DownRight), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Sloped, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.DownLeft), sheet.Frames[index++].Source));

            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.DownLeft), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Down), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Solid, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.DownRight), sheet.Frames[index++].Source));

            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.DownLeft), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.Down), sheet.Frames[index++].Source));
            tiles.Add(new TileDef(TileFlags.Passable, id++, 0, RGPoint.Empty, new DirectionFlags(Direction.DownRight), sheet.Frames[index++].Source));

            ts.mTiles = tiles.ToArray();

            return ts;
        }

        #region Saving
        private class TilesetSaveModel
        {
            public TextureResource Texture;
            public TileDef[] Tiles;
            public RGSizeI TileSize;
        }

        public object GetSaveModel()
        {
            return new TilesetSaveModel { Texture = this.Texture, Tiles = this.mTiles, TileSize = this.TileSize };
        }

        public Type GetSaveModelType()
        {
            return typeof(TilesetSaveModel);
        }

        public void Load(object saveModel)
        {
            var model = saveModel as TilesetSaveModel;
            this.mTileTexture = model.Texture;
            this.mTiles = model.Tiles;
            this.TileSize = model.TileSize;
        }
        #endregion

    }
}
