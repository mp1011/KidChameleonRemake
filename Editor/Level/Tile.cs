using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Engine;
using Editor.Forms;
using System.ComponentModel;

namespace Editor
{
    abstract class EditorTile
    {
        public BitmapPortion Image;

        [Editor(typeof(FlagEnumEditor.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))] 
        public TileFlags Flags { get; set; }

        public int ID { get; private set; }
        public int FrameDuration { get; set; }

        public bool SideUp { get; set; }
        public bool SideDown { get; set; }
        public bool SideLeft { get; set; }
        public bool SideRight { get; set; }

      
        [Category("Groups")]
        public string SideGroups
        {
            get { return Usage.SideGroups.StringJoin(" "); }
            set
            {
                var names = value.Split(' ');
                Usage.SideGroups = names;
            }
        }

        [Category("Groups")]
        public string Groups
        {
            get { return Usage.Groups.Where(p=>!String.IsNullOrEmpty(p)).StringJoin(" "); }
            set
            {
                var names = value.Split(' ');
                Usage.Groups = names.Where(p => !String.IsNullOrEmpty(p)).ToArray();
            }
        }

        [Category("Groups")]
        public string SingleGroup
        {
            get { return Usage.SingleGroup; }
            set
            {
                Usage.SingleGroup = value;
            }
        }

        public int RandomUsageWeight { get { return Usage.RandomUsageWeight; } set { Usage.RandomUsageWeight = value; } }

        [Browsable(false)]
        public TileUsage Usage { get; private set; }

        [Browsable(false)]
        public DirectionFlags Sides
        {
            get { return new DirectionFlags(SideUp, SideDown, SideLeft, SideRight); }
            set
            {
                if (value == null)
                {
                    SideUp = false; SideDown = false; SideLeft = false; SideRight = false;
                }
                else
                {
                    SideUp = value.Up;
                    SideDown = value.Down;
                    SideLeft = value.Left;
                    SideRight = value.Right;
                }
            }
        }

        public virtual TileDef CreateTileDef()
        {
            return new TileDef(Flags, ID, FrameDuration, RGPoint.Empty, Sides, this.Usage, Image.Region);
        }

        public void Initialize(BitmapPortion tileImage, int id)
        {
            Image = tileImage;
            Flags = TileFlags.Passable;
            ID = id;
            FrameDuration = 0;
            this.Usage = new TileUsage();

            InitializeExtra();
        }

        protected virtual void InitializeExtra()
        {

        }

        public void Initialize(BitmapPortion tileSheet, TileDef tile)
        {
            Image = tileSheet.Extract(tile.SourcePosition);           
            Flags = tile.Flags;
            ID = tile.TileID;
            FrameDuration = tile.FrameDuration;
            Sides = tile.Sides;
            Usage = tile.Usage;

            InitializeExtra();
        }

        public void SetID(int id)
        {
            this.ID = id;
        }
    }

    class EditorTileComparer : IEqualityComparer<EditorTile>
    {
        private PixelEqualityComparer mPixelComparer;

        public EditorTileComparer()
        {
            mPixelComparer = new PixelEqualityComparer();
        }



        public bool Equals(EditorTile x, EditorTile y)
        {
            return mPixelComparer.Equals(x.Image, y.Image);
        }

        public int GetHashCode(EditorTile obj)
        {
            return mPixelComparer.GetHashCode(obj.Image);
        }
    }

    interface IEditorTileset
    {
        void Fill(TileSet ts);
        TileSet CreateTileset(Color transparentColor, string name, Predicate<TileDef> filter);

        IEnumerable<EditorTile> Tiles { get; }
        EditorTile CreateTile(BitmapPortion portion, int id);
        EditorTile GetTile(TileInstance t);

        void AddTiles(IEnumerable<EditorTile> tiles);
    }

    class EditorTileSet<TEditorTile> : IEditorTileset where TEditorTile : EditorTile, new()
    {
        private List<TEditorTile> mTiles = new List<TEditorTile>();

        public IEnumerable<EditorTile> Tiles
        {
            get
            {
                return mTiles;
            }
        }

        public EditorTileSet()
        {
        }

        public void Fill(TileSet ts)
        {
            var image = new BitmapPortion(ts.Texture.GetImage());
            this.mTiles = ts.GetTiles().Select(t => { var tile = new TEditorTile(); tile.Initialize(image, t); return tile; }).ToList();
        }

        public void AddTiles(IEnumerable<EditorTile> tiles)
        {
            var typedTiles = tiles.OfType<TEditorTile>();
            mTiles.AddRangeDistinct(typedTiles, new EditorTileComparer());
        }


        public EditorTile CreateTile(BitmapPortion portion, int id)
        {
            var tile = new TEditorTile();
            tile.Initialize(portion, id);
            return tile;
        }

        public EditorTile GetTile(int id)
        {
            return Tiles.FirstOrDefault(p => p.ID == id);
        }

        public EditorTile GetTile(TileDef t)
        {
            return Tiles.FirstOrDefault(p => p.ID == t.TileID);
        }

        public EditorTile GetTile(TileInstance t)
        {
            return Tiles.FirstOrDefault(p => p.ID == t.TileDef.TileID);
        }

        public TileSet CreateTileset(Color transparentColor, string name, Predicate<TileDef> filter)
        {
            var img = BitmapPortion.CreateSpriteSheet(Tiles.Select(p => p.Image), 320, name, transparentColor);
            var img2 = new BitmapPortion(img.Image.GetImage().CloneImage());

            for (int i = 0; i < img.Frames.Count; i++)
            {
                mTiles[i].Image = img2.Extract(img.Frames[i].Source);
                mTiles[i].SetID(i+1);
            }

            var tiles = Tiles.Select(p => p.CreateTileDef());
            if(filter != null)
                tiles = tiles.Where(t=> filter(t));

            return new TileSet(img, tiles);
        }

    }
}
