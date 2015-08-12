using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Editor
{
    partial class TilePanelUserControl : UserControl
    {

        private SelectionGrid<TileInstance> mSelectionGrid = new SelectionGrid<TileInstance>();

        public SelectionMode SelectionMode { get { return mSelectionGrid.SelectionMode; } set { mSelectionGrid.SelectionMode = value; } }

        public IEnumerable<Map> Maps { get; private set; }

        public Map ActiveMap { get; set; }

        public TileSet Tileset { get { if (Maps.IsNullOrEmpty()) return null; return Maps.First().Tileset; } }

        public SelectionGrid<TileInstance> SelectionGrid { get { return mSelectionGrid; } }

        public TilePanel ImagePanel { get; private set; }

        public bool mIsTileset;

        public DrawRectangleType RectangleType { get { return ImagePanel.DrawRectangle; } set { ImagePanel.DrawRectangle = value; } }

        public TilePanelUserControl() : base()
        {           
            this.InitializeComponent();
            this.ImagePanel = new TilePanel(this);

            this.ImagePanel.MouseAction += new Editor.ImagePanel.MouseActionEventHandler(ImagePanel_ImageClicked);
            this.Resize += TilePanelUserControl_Resize;
            this.ImagePanel.DrawRectangle = DrawRectangleType.ShiftDrag;            
        }

        void TilePanelUserControl_Resize(object sender, EventArgs e)        
        {
            if (this.Maps.IsNullOrEmpty())
                return;
            else if (mIsTileset)
                this.SetFromTileset(this.Tileset);
            else
                this.SetFromMap(this.Maps);
        }

        private RGPointI mLastGridPoint;

        void ImagePanel_ImageClicked(object sender, ImageEventArgs e)
        {
            var gridPoint = (e.Point as EditorGridPoint);
            if (gridPoint != null)
            {
                if (gridPoint.Equals(mLastGridPoint))
                    return;

                mLastGridPoint = gridPoint.GridPoint;
            }
             
            mSelectionGrid.HandleMouseAction(e);
            this.RefreshImage();
        }

        public void SetFromMap(Map m)
        {
            SetFromMap(new Map[] { m });
        }

        public void SetFromMap(IEnumerable<Map> m)
        {
            this.Maps = m.ToArray();
            this.ActiveMap = this.Maps.FirstOrDefault();

            mSelectionGrid.SetGrid(this.ImagePanel, Maps.First().TileDimensions, (x, y) => this.ActiveMap.GetTileAtGridCoordinates(x, y));
            this.AddOverlayItem(mSelectionGrid);

            this.RefreshImage();
         
        }

        public void SetFromTileset(TileSet ts)
        {
            this.mIsTileset = true;
            var tiles = ts.GetTiles().ToArray();
            int tilesPerRow = DetermineTilesPerRow(ts.GetTiles().Count());
            var map = new Map(Program.EditorContext, new InMemoryResource<TileSet>(ts), tilesPerRow, (int)Math.Ceiling((float)(tiles.Length + 1) / tilesPerRow));

            int i = 0;
            for (int y = 0; y < map.TileDimensions.Height; y++)
                for (int x = 0; x < map.TileDimensions.Width; x++)
                {
                    map.SetTile(x, y, i++);
                }

            this.ActiveMap = map;
            this.Maps = new Map[] { map };

            this.ClearOverlay();
            mSelectionGrid.SetGrid(this.ImagePanel,map.TileDimensions, (x, y) => map.GetTileAtGridCoordinates(x, y));
            this.AddOverlayItem(mSelectionGrid);

            this.RefreshImage(true);

         
        }


        private int DetermineTilesPerRow(int tileCount)
        {
            int totalHeight = Int32.MaxValue;
            int tileSize = 64;
            int tilesPerRow=8;

            while (totalHeight > this.Height && tileSize > 16)
            {
                tilesPerRow = this.Width / tileSize;
                int numColumns =(int)Math.Ceiling((float)(tileCount + 1) / tilesPerRow);
                totalHeight = numColumns * tileSize;

                tileSize -= 2;
            }

            return tilesPerRow;
        }

        public IEnumerable<TileInstance> SelectedTiles()
        {
            if (SelectionGrid == null)
                return new TileInstance[] { };

            return SelectionGrid.SelectedPoints().Select(p => ActiveMap.GetTileAtGridCoordinates(p.X, p.Y));
        }

        public TileInstance HighlightedTile
        {
            get
            {
                return SelectionGrid.HighlightedItem;
            }
        }

        #region Common

        public void ClearOverlay()
        {
            ImagePanel.ClearOverlay();
        }

        public void RefreshImage()
        {
            ImagePanel.RefreshImage();
        }

        public void RefreshImage(bool force)
        {
            ImagePanel.RefreshImage(force);
        }


        public void AddOverlayItem(IDrawable item)
        {
            ImagePanel.AddOverlayItem(item);
        }

        #endregion


        public EditorGridPoint GetGridPointFromClientPoint(int clientX, int clientY)
        {
            var pt= EditorGridPoint.FromClientPoint(clientX, clientY, this.ImagePanel);
            pt.SnapToGrid();
            return pt;
        }

    }

    class TilePanel : ImagePanel
    {
        private TilePanelUserControl mControl;
        private IEnumerable<Map> Maps { get { return mControl.Maps.NeverNull(); } }
        public TileSet Tileset { get { return mControl.Tileset; } }

        public Map ActiveMap { get { return mControl.ActiveMap; } }

        public RGSizeI MapSize
        {
            get
            {
                return new RGSizeI(ActiveMap.Size.Width, ActiveMap.Size.Height);
            }
        }

        public TilePanel(TilePanelUserControl control)
            : base(control)
        {
            mControl = control;
            this.SetupEvents();
        }

        public override RGSizeI WorkingImageSize
        {
            get
            {
                if (Maps.IsNullOrEmpty())
                    return new RGSizeI(mControl.Width, mControl.Height);
                else
                    return new RGSizeI(ActiveMap.Size.Width, ActiveMap.Size.Height);
            }
        }

        protected override bool DrawWorkingImage(Graphics gWorking)
        {
            if (mControl.Tileset == null || mControl.Tileset.Texture == null)
            {
                gWorking.Clear(Color.DarkBlue);
                return false;
            }

            var img = mControl.Tileset.Texture.GetImage();

            foreach (var map in Maps)
            {
                for (int y = 0; y < map.TileDimensions.Height; y++)
                    for (int x = 0; x < map.TileDimensions.Width; x++)
                    {
                        var tile = map.GetTile(x, y);
                        if (!tile.IsBlank)
                            gWorking.DrawImage(Tileset.Texture.GetImage(), new Rectangle(x * Tileset.TileSize.Width, y * Tileset.TileSize.Height, Tileset.TileSize.Width, Tileset.TileSize.Height), tile.SourcePosition.ToSystemRec(), GraphicsUnit.Pixel);
                    }
            }

            return true;
        }

        protected override int GetWorkingImageChangeHashCode()
        {
            int hash = 0;
            unchecked 
            {
                foreach(var map in Maps)
                    hash += map.GetHashCode();
            }

            return hash;
                
        }

        protected override EditorPoint CreateEditorPoint(int clientX, int clientY)
        {
            return EditorGridPoint.FromClientPoint(clientX, clientY, this);
        }

        protected override EditorPoint CreateEditorPointFromImagePoint(int imageX, int imageY)
        {
            return EditorGridPoint.FromImagePoint(imageX, imageY, this);
        }
    }
   
}
