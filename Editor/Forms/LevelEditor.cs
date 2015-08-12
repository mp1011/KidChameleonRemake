using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Editor.Forms
{
    public partial class LevelEditor : Form
    {

        #region Properties
  
        private OverlayRectangle mCursor;
        private BitmapPortion mCursorImage;

        private WorldInfo WorldInfo
        {
            get
            {
                return mapProperties.SelectedObject as WorldInfo;
            }
            set
            {
                mapProperties.SelectedObject = value;
            }
        }

        #endregion

        #region Instance

        private static LevelEditor mInstance;
        public static LevelEditor GetOrOpen()
        {
            if (mInstance == null || mInstance.IsDisposed)
            {
                mInstance = new LevelEditor();
                mInstance.Show();
            }

            return mInstance;
        }

        public static LevelEditor GetInstance()
        {
            if (mInstance == null || mInstance.IsDisposed)
                return null;

            return mInstance;
        }


        public LevelEditor()
        {     
            InitializeComponent();
        }


        #endregion

        #region Events

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PromptSaveChanges();
            this.WorldInfo = Program.EditorGame.WorldInfoCreate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog.ShowSaveDialog<WorldInfo>(PathType.Maps, this.WorldInfo);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PromptSaveChanges();
            var loadedFile = FileDialog.ShowLoad<WorldInfo>(PathType.Maps, this.WorldInfo.GetType());
            System.IO.File.WriteAllText("lastmap", loadedFile.Name);
            LoadMap(loadedFile.Data);
        }


        private void LevelEditor_Load(object sender, EventArgs e)
        {
            SetupActions();

            lstShow.SetItemsChecked<string>(p => true);

            tbZoom.Maximum = (int)(ImagePanel.ZoomMax * 100);
            tbZoom.Minimum = (int)(ImagePanel.ZoomMin * 100);
            tbZoom.Value = 200;

            pnlTileset.SelectionMode = SelectionMode.Single;

            pnlMap.ImagePanel.MouseAction += new ImagePanel.MouseActionEventHandler(Map_MouseAction);
            pnlMap.SelectionGrid.SelectionChanged += new EventHandler(SelectionGrid_SelectionChanged);
            cboObjectType.DataSource = this.PlaceableObjectTypes;

            AddGridSnaps();

            try
            {
                this.WorldInfo = Program.EditorGame.WorldInfoCreate();
                var lastMap = System.IO.File.ReadAllText("lastmap");
                if (lastMap.NotNullOrEmpty())
                {
                    var path = new GamePath(PathType.Maps, lastMap);
                    var json = System.IO.File.ReadAllText(path.FullPath);
                    this.WorldInfo = (WorldInfo)Engine.Serializer.FromJson(json, this.WorldInfo.GetType());
                }
                else
                    this.WorldInfo = Program.EditorGame.WorldInfoCreate();
            }
            catch (Exception ex)
            {
                this.WorldInfo = Program.EditorGame.WorldInfoCreate();
            }

            this.LoadMap(this.WorldInfo);
        }

        private void btnApplyMapInfo_Click(object sender, EventArgs e)
        {
            var maps = this.WorldInfo.UpdateMap(Program.EditorContext);
            pnlTileset.SetFromTileset(maps[0].Tileset);
            pnlMap.SetFromMap(maps);
            ResetGridCursor();
        }

        private void tbZoom_ValueChanged(object sender, EventArgs e)
        {
            pnlMap.ImagePanel.Zoom = tbZoom.Value / 100.0f;
            pnlMap.RefreshImage();
        }

        private void LevelEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {

            }
            else if (e.KeyCode == Keys.W)
            {
                pnlMap.ImagePanel.Pan = pnlMap.ImagePanel.Pan.Offset(8, 0);
                pnlMap.RefreshImage();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                PanMap(Direction.Right);
                return true;
            }
            else if (keyData == Keys.Right)
            {
                PanMap(Direction.Left);
                return true;
            }
            else if (keyData == Keys.Up)
            {
                PanMap(Direction.Down);
                return true;
            }
            else if (keyData == Keys.Down)
            {
                PanMap(Direction.Up);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void chkShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            pnlMap.SelectionGrid.ShowGridLines = chkShowGrid.Checked;
            pnlMap.RefreshImage();
        }

        private void Map_MouseAction(object sender, ImageEventArgs e)
        {
            if (pnlMap.Tileset == null)
                return;

            var gridPoint = (e.Point as EditorGridPoint).SnapToGrid();
            var originalCursorPos = mCursor.Area.TopLeft == null ? RGPointI.Empty : mCursor.Area.TopLeft.ImagePoint;

            UpdateGridCursor(gridPoint);

            foreach (var action in mActions)
                action.PerformIfNeeded(e, tabMain.SelectedTab);

            if (e.Action == MouseActionType.Click)
                mNewlyAddedTiles.Clear();

            if (e.Buttons != System.Windows.Forms.MouseButtons.None || !originalCursorPos.Equals(mCursor.Area.TopLeft.ImagePoint))
                pnlMap.RefreshImage();
        }

        private void chkDraw_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDraw.Checked)
                pnlMap.SelectionMode = SelectionMode.None;
            else
                pnlMap.SelectionMode = SelectionMode.Multi;
        }

    
    

        #endregion
   
        #region Loading and Saving
      
        private void LoadMap(WorldInfo world)
        {
            if (world == null)
                return;

            this.WorldInfo = world;
            this.WorldInfo.UpdateMap(Program.EditorContext);
            pnlTileset.SetFromTileset(world.Maps[0].Tileset);
            pnlMap.SetFromMap(world.Maps);
            world.Maps[0].Name = "Background";
            world.Maps[1].Name = "Special";

            lstObjects.DataSource = WorldInfo.Objects;
            
            var overlay = new ObjectCollectionOverlay(this);
            pnlMap.ImagePanel.MouseAction += overlay.HandleMouse;
            pnlMap.AddOverlayItem(overlay);

            ResetGridCursor();
            AddGridSnaps();
            SetupLayersList();
            tileFilter.LoadData(pnlTileset); 
        }

        private void PromptSaveChanges()
        {
            if (MessageBox.Show("put a save prompt here. Press no to skip saving", "...", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;
        }
    
        #endregion

        #region Pan + Zoom

        private RGPointI mPanStartPos;
        private RGPointI mOriginalPan;
        private bool mIsPanning;

        private void PanMap(Direction d)
        {
            pnlMap.ImagePanel.Pan = pnlMap.ImagePanel.Pan.Offset(d, 64);
            pnlMap.RefreshImage();
        }

        private void OnWheelScroll(ImageEventArgs args)
        {
            var amount = args.MouseWheelScroll;
            tbZoom.Value = Util.LimitNumber(tbZoom.Value + amount, tbZoom.Maximum, tbZoom.Minimum);
        }

        private void HandlePan(ImageEventArgs e)
        {
            if (!mIsPanning)
            {                
                mIsPanning = true;
                mPanStartPos = e.Point.ClientPoint;
                mOriginalPan = pnlMap.ImagePanel.Pan;
            }
            else
            {               
                var offset = e.Point.ClientPoint.Difference(mPanStartPos);
                pnlMap.ImagePanel.Pan = mOriginalPan.Offset(offset);
                pnlMap.RefreshImage();

                if (e.Action == MouseActionType.Click)
                    mIsPanning = false;           
            }
        }

        #endregion
      
        #region GridSnap

        private void AddGridSnaps()
        {
            var originalValue = cboGridSnap.SelectedItem as GridSnapType;

            cboGridSnap.Items.Clear();

            RGSizeI baseSize = new RGSizeI(32, 32);

            if(this.pnlTileset.Tileset != null)
                baseSize = this.pnlTileset.Tileset.TileSize;

            cboGridSnap.Items.Add(new GridSnapType { Size = baseSize, Text = "1 Cell" });
            cboGridSnap.Items.Add(new GridSnapType { Size = baseSize.Scale(.5f), Text = "1/2 Cell" });
            cboGridSnap.Items.Add(new GridSnapType { Size = baseSize.Scale(.25f), Text = "1/4 Cell" });
            cboGridSnap.Items.Add(new GridSnapType { Size = RGSizeI.Empty, Text = "None" });

            cboGridSnap.SelectedItem = originalValue;

            if (cboGridSnap.SelectedItem == null || cboGridSnap.SelectedIndex < 0)
                cboGridSnap.SelectedIndex = 0;              
        }

        private class GridSnapType
        {
            public RGSizeI Size { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public RGSizeI GridSnap
        {
            get
            {
                return (cboGridSnap.SelectedItem as GridSnapType).Size;
            }
        }

        #endregion

        #region Actions

        private class LevelEditorAction
        {
            public MouseButtons? MouseButtons = null;
            public MouseActionType? MouseAction = null;
            public TabPage Tab;
            public Predicate<ImageEventArgs> Condition;
            public Action<ImageEventArgs> Action;

            public void PerformIfNeeded(ImageEventArgs args, TabPage selectedTab)
            {
                if (Tab != null && selectedTab != Tab)
                    return;

                if (MouseButtons.HasValue && args.Buttons != MouseButtons)
                    return;

                if (MouseAction.HasValue && args.Action != MouseAction)
                    return;

                if (Condition != null && !Condition(args))
                    return;

                Action(args);

            }
        }
        private List<LevelEditorAction> mActions = new List<LevelEditorAction>();

        private void SetupActions()
        {
            mActions.Clear();
 
            //zoom
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = MouseButtons.None,
                MouseAction = MouseActionType.Wheel,
                Action = OnWheelScroll
            });

            //pan start
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = MouseButtons.Left,
                MouseAction = MouseActionType.MouseDown,
                Condition = a=> Control.ModifierKeys == Keys.Control && !mIsPanning,
                Action = HandlePan
            });

            //pan stop
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = MouseButtons.Left,
                MouseAction = MouseActionType.MouseDown,
                Condition = a=> Control.ModifierKeys == Keys.Control && mIsPanning,
                Action = HandlePan
            });

            //copy tile
            mActions.Add(new LevelEditorAction 
            {
                MouseButtons = MouseButtons.Right,
                MouseAction = MouseActionType.Click,
                Tab = pgeTiles,
                Action = CopyTile
            });

            //draw tile
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.MouseDown,
                Tab = pgeTiles,
                Condition = args => !chkMatchPrev.Checked && chkDraw.Checked && GetCurrentlySelectedTile() != null,
                Action = DrawTile
            });

            //draw tile 2
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.Move,
                Tab = pgeTiles,
                Condition = args => !chkMatchPrev.Checked && chkDraw.Checked && GetCurrentlySelectedTile() != null,
                Action = DrawTile
            });
          
            //draw tiles in rectangle
            mActions.Add(new LevelEditorAction
            {
                MouseAction = MouseActionType.RectangleSelection,
                Tab = pgeTiles,
                Condition = args => chkDraw.Checked && GetCurrentlySelectedTile() != null,
                Action = DrawTileRange 
            });

            //alter tile
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.Click,
                Tab = pgeTiles,
                Condition = args => Control.ModifierKeys == Keys.Alt && chkDraw.Checked,
                Action = AlterTile 
            });

            //select object
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.Click,
                Tab = pgeObjects,
                Action = SelectObjectUnderCursor
            });

            //remove object
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Right,
                MouseAction = MouseActionType.Click,
                Tab = pgeObjects,
                Action = RemoveObjectUnderCursor
            });

            //place object
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.DoubleClick,
                Tab = pgeObjects,
                Action = PlaceObject
            });

            //auto match
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.Click,
                Tab = pgeTiles,
                Condition = a=> chkEnableTileMatching.Checked && chkDraw.Checked,
                Action = RandomizeAddedTiles 
            });

            //alter tile
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.Click,
                Tab = pgeTiles,
                Condition = a => chkEnableTileMatching.Checked && !chkDraw.Checked,
                Action = AlterTile
            });

            //alter tile against previous
            mActions.Add(new LevelEditorAction
            {
                MouseButtons = System.Windows.Forms.MouseButtons.Left,
                MouseAction = MouseActionType.Click,
                Tab = pgeTiles,
                Condition = a => chkMatchPrev.Checked,
                Action = MatchAgainstPreviousTile
            });

        }
        
        private int mSelectedTileIndex = -1;
        private TileDef GetNextSelectedTile()
        {
            if (mSelectedTileIndex < 0 || mSelectedTileIndex >= pnlTileset.SelectedTiles().Count())
                mSelectedTileIndex = -1;
            
            mSelectedTileIndex++;

            var instance = pnlTileset.SelectedTiles().ElementAtOrDefault(mSelectedTileIndex);
            if (instance == null)
                return TileDef.Blank;
            else
                return instance.TileDef;
        }

        private TileDef GetCurrentlySelectedTile()
        {
            if (mSelectedTileIndex < 0 || mSelectedTileIndex >= pnlTileset.SelectedTiles().Count())
                mSelectedTileIndex = 0;

            var instance = pnlTileset.SelectedTiles().ElementAtOrDefault(mSelectedTileIndex);
            if (instance == null)
                return TileDef.Blank;
            else
                return instance.TileDef;
        }

        private void DrawTile(ImageEventArgs e)
        {
            RGPointI gridPoint = (e.Point as EditorGridPoint).GridPoint;
            DrawTile(gridPoint);
        }

        private void DrawTile(RGPointI gridPoint)
        {
            TileDef selectedTile = GetCurrentlySelectedTile();
            var newTile = pnlMap.ActiveMap.SetTile(gridPoint.X, gridPoint.Y, selectedTile.TileID);
            OnTileAdded(newTile);
        }

        private void DrawTileRange(ImageEventArgs args)
        {
            TileDef selectedTile = GetCurrentlySelectedTile();
            var rectangleEventArgs = args as DrawRectangleEventArgs;
            if (rectangleEventArgs != null)
            {
                var topLeft = (rectangleEventArgs.Point as EditorGridPoint).GridPoint;
                var bottomRight = (rectangleEventArgs.Point2 as EditorGridPoint).GridPoint;

                for (int x = topLeft.X; x <= bottomRight.X; x++)
                    for (int y = topLeft.Y; y <= bottomRight.Y; y++)
                        pnlMap.ActiveMap.SetTile(x, y, selectedTile.TileID);

                return;
            }         
        }

        private void CopyTile(ImageEventArgs args)
        {
            CopyTile((args.Point as EditorGridPoint).GridPoint);
        }

        private void CopyTile(RGPointI gridPoint)
        {
            var selectedMapTile = pnlMap.ActiveMap.GetTile(gridPoint.X, gridPoint.Y);

            if (!selectedMapTile.IsBlank && !pnlTileset.Tileset.GetTiles().Any(p => p.TileID == selectedMapTile.TileID))
            {
                pnlTileset.Tileset.AddTile(selectedMapTile);
                pnlTileset.SetFromTileset(pnlTileset.Tileset);
            }
                
            pnlTileset.SelectionGrid.SelectWhere(t => t.TileDef.TileID == selectedMapTile.TileID, true);
            pnlTileset.RefreshImage();

            pnlMap.SelectionGrid.ClearSelection();
            pnlMap.SelectionGrid.SetSelection(gridPoint.X, gridPoint.Y, true);

            TileUsageHelper.LogMatchInfo(pnlMap.ActiveMap.GetTileAtGridCoordinates(gridPoint.X, gridPoint.Y));
        }

        #endregion

        #region Special Properties

        private void propSpecialTile_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            foreach (var tile in pnlMap.SelectedTiles())
            {
                e.ChangedItem.PropertyDescriptor.SetValue(tile, e.ChangedItem.Value);
                pnlMap.ActiveMap.UpdateSpecialInstance(tile);
            }
        }


        void SelectionGrid_SelectionChanged(object sender, EventArgs e)
        {
            var selectedTile = pnlMap.SelectedTiles().FirstOrDefault();
            if (selectedTile == null)
                return;

            var loc = selectedTile.TileLocation;
            var t = pnlMap.ActiveMap.GetTileAtGridCoordinates(loc.X, loc.Y);

            propSpecialTile.SelectedObject = t;
        }

        #endregion

        #region Objects

        private ObjectType[] mPlaceableObjectTypes;
        private ObjectType[] PlaceableObjectTypes
        {
            get
            {
                if (mPlaceableObjectTypes != null)
                    return mPlaceableObjectTypes;

                mPlaceableObjectTypes = ReflectionHelper.GetAssembly(Program.EditorGame).GetTypesByAttribute<EditorVisibleAttribute>().SelectMany(p =>
                    p.GetPropertiesByAttribute<EditorVisibleAttribute>().Select(k => (ObjectType)k.GetValue(null, null))).ToArray();

                return mPlaceableObjectTypes;
            }
        }

        private Bitmap GetObjectTypeImage(ObjectType t)
        {
            var layer = new FixedLayer(new World());
            var sprite = t.CreateSpriteInstance(layer, Program.EditorContext).Sprite;
            sprite.Location = sprite.CurrentAnimation.CurrentDirectedAnimationFrame.Origin;

            var img = new Bitmap(sprite.CurrentAnimation.DestinationRec.Width, sprite.CurrentAnimation.DestinationRec.Height);
            var g = Graphics.FromImage(img);
            var painter = new GraphicsPainter(g);
            sprite.CurrentAnimation.Draw(painter, RGRectangleI.FromXYWH(0, 0, 1000, 1000));
            g.Dispose();
            return img;
        }

        class GraphicsPainter : Engine.Graphics.Painter
        {
            public Graphics Graphics { get; set; }
            public TilePanel Panel { get; set; }
            
            public GraphicsPainter() : base(Program.EditorContext) { }

            public GraphicsPainter(Graphics graphics) : base(Program.EditorContext) { Graphics = graphics; }

            public RGRectangleI TranslateRectangle(RGRectangleI rec)
            {
                if (Panel == null)
                    return rec;

                return new EditorRectangle()
                {
                    TopLeft = EditorGridPoint.FromImagePoint(rec.Left, rec.Top, Panel),
                    BottomRight = EditorGridPoint.FromImagePoint(rec.Right, rec.Bottom, Panel)
                }.ClientRectangle;
            }

            protected override void PaintToScreen(TextureResource texture, RGRectangleI source, RGRectangleI dest, RenderOptions options, StackedRenderInfo extraRenderInfo)
            {
                var textureImage = texture.GetImage();
                Graphics.DrawImage(textureImage, this.TranslateRectangle(dest).ToSystemRec(), source.ToSystemRec(), GraphicsUnit.Pixel);               
            }
        }



        private ObjectType SelectedObjectType
        {
            get { return (ObjectType)cboObjectType.SelectedValue; }
        }

        private void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbObjectPreview.Image = GetObjectTypeImage(SelectedObjectType);
        }

        private ObjectEntry CurrentObjectEntry
        {
            get
            {
                return (pgObject.SelectedObject as ObjectEntry);
            }
            set
            {
                pgObject.SelectedObject = value;
            }
        }

        private ObjectEntry GetObjectFromPoint(RGPointI point)
        {
            return this.WorldInfo.Objects.FirstOrDefault(p => p.PlacedObject.Area.Contains(point));
        }

        private void RemoveObjectUnderCursor(ImageEventArgs args)
        {
            var objectUnderCursor = GetObjectFromPoint(args.Point.ImagePoint);
            this.WorldInfo.Objects.Remove(objectUnderCursor);
            this.CurrentObjectEntry = null;
        }

        private void PlaceObject(ImageEventArgs args)
        {
            var point = args.Point as EditorGridPoint;
            this.CurrentObjectEntry = new ObjectEntry() { SpriteType = SelectedObjectType, Location=point.ImagePoint}; ;
            this.WorldInfo.Objects.Add(this.CurrentObjectEntry);
            this.CurrentObjectEntry.CreateObject(new FixedLayer(new World(Program.EditorContext, this.WorldInfo)));
            pnlMap.RefreshImage();
        }

        private void SelectObjectUnderCursor(ImageEventArgs args)
        {
            var objectUnderCursor = GetObjectFromPoint(args.Point.ImagePoint);
            this.CurrentObjectEntry = objectUnderCursor;
        }

        private class ObjectCollectionOverlay : IDrawable
        {
            private LevelEditor mForm;
            private ObjectOverlay[] mObjects;
            private GraphicsPainter mPainter;

            private IEnumerable<ObjectOverlay> Objects
            {
                get
                {
                    if (mObjects == null || mObjects.Count() != mForm.WorldInfo.Objects.Count)
                        mObjects = mForm.WorldInfo.Objects.Select(p => new ObjectOverlay(p, mForm, mPainter)).ToArray();
                    return mObjects;
                }
            }

            public ObjectCollectionOverlay(LevelEditor form) 
            { 
                mForm = form;
                mPainter = new GraphicsPainter() { Panel = form.pnlMap.ImagePanel };
            }

            public void DrawToClient(Graphics g)
            {
                mPainter.Graphics = g;

                foreach (var o in Objects)
                    o.DrawToClient(g);
            }

            public IEnumerable<EditorPoint> GetPoints()
            {
                return Objects.SelectMany(p => p.GetPoints());
            }

            public void HandleMouse(object sender, ImageEventArgs mouseArgs)
            {
                var nothingSelected=true;
                var wasSelected = false;

                foreach (var o in Objects)
                {
                    wasSelected = o.Selected;
                    o.Highlighted = o.ClientRectangle.Contains(mouseArgs.Point.ClientPoint);
                    o.Selected = (o.ObjectEntry == mForm.CurrentObjectEntry);

                    if(wasSelected || o.Selected || o.IsDragging)
                        o.HandleMouse(mouseArgs);
                }
                
            }
        }

        private class ObjectOverlay : IDrawable 
        {
            public bool Highlighted { get; set; }
            public bool Selected { get; set; }

            public ObjectEntry ObjectEntry { get; set; }

            private EditorPoint mPoint;
            private GraphicsPainter mPainter;
            private Brush mHighlightBrush;
            private Pen mSelectedPen;
            private DragHandler mDragHandler;

            public bool IsDragging { get { return mDragHandler.IsDragging; } }

            public RGRectangleI ClientRectangle
            {
                get
                {
                    return mPainter.TranslateRectangle(ObjectEntry.PlacedObject.CurrentAnimation.DestinationRec).Inflate(8);              
                }
            }

            public ObjectOverlay(ObjectEntry obj, LevelEditor form, GraphicsPainter painter)
            {
                ObjectEntry = obj;
                mPoint = EditorGridPoint.FromImagePoint(ObjectEntry.Location.X, ObjectEntry.Location.Y, form.pnlMap.ImagePanel);
                mPainter = painter;

                mDragHandler = new DragHandler(obj, () => form.GridSnap);

                mHighlightBrush = new SolidBrush(Color.Orange);
                mSelectedPen = new Pen(Color.LightGreen, 2f);
            }

            public void HandleMouse(ImageEventArgs args)
            {
                mDragHandler.HandleMouse(args);
            }

            public void DrawToClient(Graphics g)
            {
                try
                {
                  
                    if (Highlighted)
                        g.FillRectangle(mHighlightBrush, ClientRectangle.ToSystemRec());

                    mPainter.Paint(RGRectangleI.FromXYWH(0, 0, 9000, 9000), ObjectEntry.PlacedObject);

                    if (Selected)
                        g.DrawRectangle(mSelectedPen, ClientRectangle.ToSystemRec());
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            public IEnumerable<EditorPoint> GetPoints()
            {
                yield return mPoint;
            }
        }


        #endregion

        #region Tile Matching

        private List<TileInstance> mNewlyAddedTiles = new List<TileInstance>();
        private List<TileInstance> mAddedTiles = new List<TileInstance>();
        
        private void MatchAgainstPreviousTile(ImageEventArgs e)
        {
            EditorGridPoint location = e.Point as EditorGridPoint;
            var tile = pnlMap.ActiveMap.GetTileAtGridCoordinates(location.GridPoint.X, location.GridPoint.Y);

            //try selected map tile first
            var matchTile = pnlMap.SelectedTiles().FirstOrDefault();
            if(matchTile == null || matchTile.TileLocation.Equals(tile.TileLocation))
                matchTile = mAddedTiles.Reverse<TileInstance>().FirstOrDefault(p => !p.TileLocation.Equals(tile.TileLocation));

            if (matchTile == null)
                return;

            var xDist = Math.Abs(tile.TileLocation.X - matchTile.TileLocation.X);
            var yDist = Math.Abs(tile.TileLocation.Y - matchTile.TileLocation.Y);
            if ((xDist > 0 && yDist > 0) || xDist > 1 || yDist > 1)
            {
                DrawTile(tile.TileLocation);
                return;
            }

            var newTile = TileUsageHelper.GetNextTileReplacement(tile, pnlTileset.Tileset.GetTiles(), matchTile);
            if (newTile != null)
            {       
                var chosenTile = pnlMap.ActiveMap.SetTile(location.GridPoint.X, location.GridPoint.Y, newTile.TileID);
                OnTileAdded(chosenTile);
            }
        }

        private void AlterTile(ImageEventArgs e)
        {
            EditorGridPoint location = e.Point as EditorGridPoint;
            var tile = pnlMap.ActiveMap.GetTileAtGridCoordinates(location.GridPoint.X, location.GridPoint.Y);
            Forms.frmLog.AddLine("Tile #" + tile.TileDef.TileID);

            var newTile = TileUsageHelper.GetNextTileReplacement(tile, pnlTileset.Tileset.GetTiles());
            if (newTile != null)
            {
                Forms.frmLog.AddLine("Chose Tile #" + newTile.TileID);

                var chosenTile = pnlMap.ActiveMap.SetTile(location.GridPoint.X, location.GridPoint.Y, newTile.TileID);
                OnTileAdded(chosenTile);

                var tile2 = pnlMap.ActiveMap.GetTileAtGridCoordinates(location.GridPoint.X, location.GridPoint.Y);
                Forms.frmLog.AddLine("New Tile #" + tile2.TileDef.TileID);
            }
        }

        private void OnTileAdded(TileInstance tile)
        {
            mNewlyAddedTiles.AddDistinct(tile);

            var last = mAddedTiles.LastOrDefault();
            if (!tile.Equals(last))
                mAddedTiles.Add(tile);

            CopyTile(tile.TileLocation);
            UpdateGridCursor();
        }
   
        private void RandomizeAddedTiles(ImageEventArgs args)
        {
            mNewlyAddedTiles = TileUsageHelper.RandomizeAddedTiles(mNewlyAddedTiles,pnlTileset.Tileset.GetTiles()).ToList();

            if (chkAlterNeighbors.Checked)
                TileUsageHelper.RandomizeNeighbors(mNewlyAddedTiles, pnlTileset.Tileset.GetTiles());
        }

        #endregion

        #region Layers

        private void SetupLayersList()
        {
            lstShow.Items.Clear();
            foreach (var map in this.WorldInfo.Maps)
                lstShow.Items.Add(map, true);
        }

        private void lstShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            var activeMap = lstShow.SelectedItem as Map;
            if (activeMap != null)
                pnlMap.ActiveMap = activeMap;
        }

        private void lstShow_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var visibleMaps = new List<Map>();
            for (int index = 0; index < lstShow.Items.Count; index++)
            {
                if (lstShow.GetItemChecked(index, e))
                    visibleMaps.Add(lstShow.Items[index] as Map);
            }

            pnlMap.SetFromMap(visibleMaps);
        }


        #endregion

        private void ResetGridCursor()
        {
            mCursor = new OverlayRectangle { Area = new EditorRectangle(), Pen = new Pen(Color.Orange), Brush = new SolidBrush(Color.FromArgb(150, Color.Orange)) };
            pnlMap.AddOverlayItem(mCursor);
            mCursorImage = new BitmapPortion(pnlTileset.ImagePanel.Tileset.Texture.GetImage());
        }

        private void UpdateGridCursor(EditorGridPoint gridPoint)
        {
            mCursor.Area.TopLeft = gridPoint;
            mCursor.Area.BottomRight = gridPoint.OffsetGrid(1, 1);

            UpdateGridCursor();
        }

        private void UpdateGridCursor()
        {
            TileDef selectedTile = GetCurrentlySelectedTile();
            if (selectedTile == null || !chkDraw.Checked)
                mCursor.Image = null;
            else
            {
                mCursor.Image = mCursorImage;
                mCursorImage.Crop(selectedTile.SourcePosition);
            }

        }



    }
}
