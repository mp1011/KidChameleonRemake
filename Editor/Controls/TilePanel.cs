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

        public Map Map { get; private set; }

        public TileSet Tileset { get { if(Map == null) return null; return Map.Tileset; } }

        public SelectionGrid<TileInstance> SelectionGrid { get { return mSelectionGrid; } }

        public TilePanel ImagePanel { get; private set; }

        public bool mIsTileset;

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
            if (this.Map == null)
                return;
            else if (mIsTileset)
                this.SetFromTileset(this.Tileset);
            else
                this.SetFromMap(this.Map);
        }


        void ImagePanel_ImageClicked(object sender, ImageEventArgs e)
        {
            mSelectionGrid.HandleMouseAction(e);
            this.RefreshImage();
        }

        public void SetFromMap(Map m)
        {
            this.Map = m;
            this.ClearOverlay();
            mSelectionGrid.SetGrid(this.ImagePanel, Map.TileDimensions, (x, y) => this.Map.GetTileAtCoordinates(x, y));
            this.AddOverlayItem(mSelectionGrid);

            this.RefreshImage();
        }

        public void SetFromTileset(TileSet ts)
        {
            this.mIsTileset = true;
            var tiles = ts.GetTiles().ToArray();
            int tilesPerRow = DetermineTilesPerRow(ts.GetTiles().Count());
            Map = new Map(Program.EditorContext, new InMemoryResource<TileSet>(ts), tilesPerRow, (int)Math.Ceiling((float)(tiles.Length + 1) / tilesPerRow));

            int i = 0;
            for (int y = 0; y < Map.TileDimensions.Height; y++)
                for (int x = 0; x < Map.TileDimensions.Width; x++)
                {
                    Map.SetTile(x, y, i++);
                }

            this.ClearOverlay();
            mSelectionGrid.SetGrid(this.ImagePanel, Map.TileDimensions, (x, y) => this.Map.GetTileAtCoordinates(x, y));
            this.AddOverlayItem(mSelectionGrid);

            this.RefreshImage();
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

            return SelectionGrid.SelectedPoints().Select(p => Map.GetTileAtCoordinates(p.X, p.Y));
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
        private Map Map { get { return mControl.Map; } }
        public TileSet Tileset { get { return mControl.Tileset; } }

        public RGSizeI MapSize
        {
            get
            {
                return new RGSizeI(Map.Size.Width, Map.Size.Height);
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
                if (Map == null)
                    return new RGSizeI(mControl.Width, mControl.Height);
                else
                    return new RGSizeI(Map.Size.Width, Map.Size.Height);
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

            for (int y = 0; y < Map.TileDimensions.Height; y++)
                for (int x = 0; x < Map.TileDimensions.Width; x++)
                {
                    var tile = Map.GetTile(x, y);
                    if (!tile.IsBlank)
                        gWorking.DrawImage(Tileset.Texture.GetImage(), new Rectangle(x * Tileset.TileSize.Width, y * Tileset.TileSize.Height, Tileset.TileSize.Width, Tileset.TileSize.Height), tile.SourcePosition.ToSystemRec(), GraphicsUnit.Pixel);
                }

            return true;
        }

        protected override int GetWorkingImageChangeHashCode()
        {
            return Map.GetHashCode();
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
