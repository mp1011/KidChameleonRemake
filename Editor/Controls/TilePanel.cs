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

        public SelectionMode SelectionMode { get; set; }

        public Map Map { get; private set; }

        public TileSet Tileset { get { if(Map == null) return null; return Map.Tileset; } }

        public SelectionGrid<TileInstance> SelectionGrid { get { return mSelectionGrid; } }

        public TilePanel ImagePanel { get; private set; }
       
        public TilePanelUserControl() : base()
        {           
            this.InitializeComponent();
            this.ImagePanel = new TilePanel(this);

            this.ImagePanel.MouseAction += new Editor.ImagePanel.MouseActionEventHandler(ImagePanel_ImageClicked);
        }

        void ImagePanel_ImageClicked(object sender, ImageEventArgs e)
        {
            if (SelectionMode == Editor.SelectionMode.None)
                return;

            if (e.Buttons == MouseButtons.Left && e.Action == MouseActionType.Click) 
            {
                var gridPoint = e.Point as EditorGridPoint;

                if (SelectionMode == Editor.SelectionMode.Single || Control.ModifierKeys != Keys.Shift)
                    SelectionGrid.ClearSelection();

                SelectionGrid.ToggleSelection(gridPoint);
                RefreshImage();
            }
        }

        public void SetMap(Map m)
        {
            this.Map = m;
            this.ClearOverlay();
            mSelectionGrid.SetGrid(this.ImagePanel, Map.TileDimensions, (x, y) => this.Map.GetTileAtCoordinates(x, y));
            this.AddOverlayItem(mSelectionGrid);

            this.RefreshImage();
        }

        public void SetTiles(TileSet ts, int tilesPerRow)
        {
            var tiles = ts.GetTiles().ToArray();
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
