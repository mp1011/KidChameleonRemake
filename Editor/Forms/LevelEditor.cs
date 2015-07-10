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

        private void LevelEditor_Load(object sender, EventArgs e)
        {
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
            catch(Exception ex)
            {
                this.WorldInfo = Program.EditorGame.WorldInfoCreate();
            }

            this.LoadMap(this.WorldInfo);
        }
  
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

        private OverlayRectangle mCursor;
        private BitmapPortion mCursorImage;

        private void mapProperties_Click(object sender, EventArgs e)
        {

        }

        private void btnApplyMapInfo_Click(object sender, EventArgs e)
        {
            var map = this.WorldInfo.UpdateMap(Program.EditorContext);
            pnlTileset.SetFromTileset(map.Tileset);
            pnlMap.SetFromMap(map);
            mCursor =null;
            mCursorImage = null;
        }

        #region Loading and Saving

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
            var loadedFile= FileDialog.ShowLoad<WorldInfo>(PathType.Maps, this.WorldInfo.GetType());
            System.IO.File.WriteAllText("lastmap", loadedFile.Name);
            LoadMap(loadedFile.Data);          
        }

        private void LoadMap(WorldInfo world)
        {
            if (world == null)
                return;

            this.WorldInfo = world;
            this.WorldInfo.UpdateMap(Program.EditorContext);
            pnlTileset.SetFromTileset(world.Map.Tileset);
            pnlMap.SetFromMap(world.Map);

            lstObjects.DataSource = WorldInfo.Objects;
            
            var overlay = new ObjectCollectionOverlay(this);
            pnlMap.ImagePanel.MouseAction += overlay.HandleMouse;
            pnlMap.AddOverlayItem(overlay);

            mCursor = null;
            mCursorImage = null;
            AddGridSnaps();
            UpdateControls();
        }

        #endregion

        private void PromptSaveChanges()
        {
            if (MessageBox.Show("put a save prompt here. Press no to skip saving", "...", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;
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

        private void PanMap(Direction d)
        {
            pnlMap.ImagePanel.Pan = pnlMap.ImagePanel.Pan.Offset(d, 64);
            pnlMap.RefreshImage();
        }

        private void chkShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            pnlMap.SelectionGrid.ShowGridLines = chkShowGrid.Checked;
            pnlMap.RefreshImage();
        }


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

        private enum EditorAction
        {
            None = 0,
            Select = 1,
            Draw = 2,
            PlaceObject = 3
        }

        private EditorAction mCurrentAction;
        private EditorAction CurrentAction
        {
            get
            {
                if (tabMain.SelectedTab == pgeObjects)
                    return EditorAction.PlaceObject;
                else if (chkDraw.Checked)
                    return EditorAction.Draw;
                else if (chkSelect.Checked)
                    return EditorAction.Select;
                else
                    return EditorAction.None;
            }
            set
            {
                if (value == mCurrentAction)
                    return;

                mCurrentAction = value;
                chkDraw.Checked = (value == EditorAction.Draw);
                chkSelect.Checked = (value == EditorAction.Select);

                if (value != EditorAction.Select)
                    pnlMap.SelectionGrid.ClearSelection();
            }
        }

        private void chkDraw_CheckedChanged(object sender, EventArgs e)
        {
            if(chkDraw.Checked)
                CurrentAction = EditorAction.Draw;
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelect.Checked)
            {
                CurrentAction = EditorAction.Select;
                pnlMap.SelectionMode = SelectionMode.Multi;
            }
            else
                pnlMap.SelectionMode = SelectionMode.None;
        }

        void Map_MouseAction(object sender, ImageEventArgs e)
        {
            if (pnlMap.Tileset == null)
                return;

            if (mCursor == null)
            {
                mCursor = new OverlayRectangle { Area = new EditorRectangle(), Pen = new Pen(Color.Orange), Brush = new SolidBrush(Color.FromArgb(150, Color.Orange)) };
                pnlMap.AddOverlayItem(mCursor);
                mCursorImage = new BitmapPortion(pnlTileset.ImagePanel.Tileset.Texture.GetImage());
            }

            var gridPoint = (e.Point as EditorGridPoint).SnapToGrid();
            var originalCursorPos = mCursor.Area.TopLeft == null ? RGPointI.Empty : mCursor.Area.TopLeft.ImagePoint;

            mCursor.Area.TopLeft = gridPoint;
            mCursor.Area.BottomRight = gridPoint.OffsetGrid(1, 1);
            mCursor.Image = null;

            if (e.Action == MouseActionType.Wheel)
                HandleWheelScroll(e.MouseWheelScroll);
            else if(HandlePan(e))
                return;
            else if (Control.ModifierKeys == Keys.Alt)
            {
                if(e.Action == MouseActionType.Click)   
                    HandleTileAlter(gridPoint, e);
                return;
            }

            switch (CurrentAction)
            {
                case EditorAction.Draw: DrawTile(gridPoint, e); break;
                case EditorAction.PlaceObject: HandleObjectMouseClick(gridPoint, e); break;
            }


            if (chkAutoMode.Checked && CurrentAction == EditorAction.Draw)
            {
                if (e.Action == MouseActionType.Click)
                    RandomizeAddedTiles();
                else if (e.Action == MouseActionType.MouseDown)
                    mMatchTiles.Clear();
            }


            if (e.Buttons != System.Windows.Forms.MouseButtons.None || !originalCursorPos.Equals(mCursor.Area.TopLeft.ImagePoint))
                pnlMap.RefreshImage();


        }

        private void HandleWheelScroll(int amount)
        {
            tbZoom.Value = Util.LimitNumber(tbZoom.Value + amount, tbZoom.Maximum, tbZoom.Minimum);
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

        private void DrawTile(EditorGridPoint gridPoint, ImageEventArgs e)
        {
            TileDef selectedTile = GetCurrentlySelectedTile();

            if (selectedTile == null)
                mCursor.Image=null;
            else
            {
                mCursor.Image = mCursorImage;
                mCursorImage.Crop(selectedTile.SourcePosition);
            }

            if (selectedTile != null && e.Action == MouseActionType.RectangleSelection)
            {
                var rectangleEventArgs = e as DrawRectangleEventArgs;
                if (rectangleEventArgs != null)
                {
                    var topLeft = (rectangleEventArgs.Point as EditorGridPoint).GridPoint;
                    var bottomRight = (rectangleEventArgs.Point2 as EditorGridPoint).GridPoint;

                    for (int x = topLeft.X; x <= bottomRight.X; x++)
                        for (int y = topLeft.Y; y <= bottomRight.Y; y++)
                            pnlMap.Map.SetTile(x, y, selectedTile.TileID);

                    return;
                }
            }


            if (selectedTile != null && e.Buttons == System.Windows.Forms.MouseButtons.Left)
            {
                pnlMap.Map.SetTile(gridPoint.GridPoint.X, gridPoint.GridPoint.Y, selectedTile.TileID);
                AddTileToMatchGroupIfNeeded(gridPoint.GridPoint);
                if (e.Buttons == System.Windows.Forms.MouseButtons.Left && e.Action == MouseActionType.Click)
                    selectedTile = GetNextSelectedTile();
            }
            else if (e.Buttons == System.Windows.Forms.MouseButtons.Right)
            {     
                var selectedMapTile = pnlMap.Map.GetTile(gridPoint.GridPoint.X, gridPoint.GridPoint.Y);

                if (!pnlTileset.Tileset.GetTiles().Any(p => p.TileID == selectedMapTile.TileID))
                {
                    pnlTileset.Tileset.AddTile(selectedMapTile);
                    pnlTileset.RefreshImage();

                    pnlTileset.SetFromTileset(pnlTileset.Tileset);
                }
                
                pnlTileset.SelectionGrid.SelectWhere(t => t.TileDef.TileID == selectedMapTile.TileID, true);
                pnlTileset.RefreshImage();
            }
        }

       

        #endregion

        #region Groups

        private void UpdateControls()
        {
            lstGroups.Items.Clear();
            lstGroups.Items.Add("All");
             
            foreach (var group in pnlMap.Tileset.GetTiles().SelectMany(p => p.Usage.DistinctGroupNames).Distinct().ToArray())
            {
                if (String.IsNullOrEmpty(group))
                    continue;
                lstGroups.Items.Add(group, true);
            }
        }

        private bool mInItemCheck=false;
        private void lstGroups_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (mInItemCheck)
                return;

            try
            {
                mInItemCheck = true;
                var ts = pnlMap.Tileset;

                if (e.Index == 0)
                {
                    if (lstGroups.GetItemChecked(0, e))
                    {
                        pnlTileset.SetFromTileset(pnlMap.Tileset);
                        for (int i = 1; i < lstGroups.Items.Count; i++)
                            lstGroups.SetItemChecked(i, true);
                    }
                    else
                    {
                        for (int i = 1; i < lstGroups.Items.Count; i++)
                            lstGroups.SetItemChecked(i, false);
                    }

                    return;
                }
                else
                {
                    lstGroups.SetItemChecked(0, false);
                }

                var groups = lstGroups.GetCheckedItems<string>(e);
                var filteredTilset = new TileSet(ts.Texture, ts.TileSize, ts.GetTiles().Where(p => p.Usage.ContainsGroups(groups)));

                pnlTileset.SetFromTileset(filteredTilset);
            }
            finally
            {
                mInItemCheck = false;
            }
        }


        private void btnCreateGroup_Click(object sender, EventArgs e)
        {
            //var groupName = cboGroup.Text;

            //foreach (var tileInstance in pnlMap.SelectedTiles())
            //{
            //  //  tileInstance.TileDef.Usage.MainGroup = groupName;
            //}

            //foreach (var tileInstance in pnlMap.SelectedTiles())
            //{
            //    var def = tileInstance.TileDef;

            //    var tileAbove = tileInstance.GetAdjacentTile(0,-1).TileDef.Usage;
            //    var tileBelow = tileInstance.GetAdjacentTile(0, 1).TileDef.Usage;
            //    var tileLeft = tileInstance.GetAdjacentTile(-1,0).TileDef.Usage;
            //    var tileRight = tileInstance.GetAdjacentTile(1,0).TileDef.Usage;

            //    tileAbove.BottomLeftGroup = tileAbove.BottomLeftGroup.AppendCSV(groupName);
            //    tileAbove.BottomRightGroup = tileAbove.BottomRightGroup.AppendCSV(groupName);

            //    tileBelow.TopLeftGroup = tileAbove.TopLeftGroup.AppendCSV(groupName);
            //    tileBelow.TopRightGroup = tileAbove.TopRightGroup.AppendCSV(groupName);

            //    tileLeft.RightTopGroup = tileAbove.RightTopGroup.AppendCSV(groupName);
            //    tileLeft.RightBottomGroup = tileAbove.RightBottomGroup.AppendCSV(groupName);

            //    tileRight.LeftTopGroup = tileAbove.LeftTopGroup.AppendCSV(groupName);
            //    tileRight.LeftBottomGroup = tileAbove.LeftBottomGroup.AppendCSV(groupName);

            //    TileDef.Blank.Usage.SingleGroup = "empty";

            //    def.Usage.TopLeftGroup = def.Usage.TopLeftGroup.AppendCSV(tileAbove.BottomLeftGroup);
            //    def.Usage.TopRightGroup = def.Usage.TopRightGroup.AppendCSV(tileAbove.BottomRightGroup);

            //    def.Usage.RightTopGroup = def.Usage.RightTopGroup.AppendCSV(tileRight.LeftTopGroup);
            //    def.Usage.RightBottomGroup = def.Usage.RightBottomGroup.AppendCSV(tileRight.LeftBottomGroup);

            //    def.Usage.BottomRightGroup = def.Usage.BottomRightGroup.AppendCSV(tileBelow.TopRightGroup);
            //    def.Usage.BottomLeftGroup = def.Usage.BottomLeftGroup.AppendCSV(tileBelow.TopLeftGroup);

            //    def.Usage.LeftBottomGroup = def.Usage.LeftBottomGroup.AppendCSV(tileLeft.RightBottomGroup);
            //    def.Usage.LeftTopGroup = def.Usage.LeftTopGroup.AppendCSV(tileLeft.RightTopGroup);

          
            //    //if (tileAbove.MainGroup == groupName)
            //    //{
            //    //    def.Usage.TopLeftGroup = def.Usage.TopLeftGroup.RemoveCSV("empty");
            //    //    def.Usage.TopRightGroup = def.Usage.TopRightGroup.RemoveCSV("empty");
            //    //}

            //    //if (tileBelow.MainGroup == groupName)
            //    //{ 
            //    //    def.Usage.BottomLeftGroup = def.Usage.BottomLeftGroup.RemoveCSV("empty");
            //    //    def.Usage.BottomRightGroup = def.Usage.BottomRightGroup.RemoveCSV("empty");
            //    //}

            //    //if (tileRight.MainGroup == groupName)
            //    //{
            //    //    def.Usage.RightTopGroup = def.Usage.RightTopGroup.RemoveCSV("empty");
            //    //    def.Usage.RightBottomGroup = def.Usage.RightBottomGroup.RemoveCSV("empty");
            //    //}

            //    //if (tileLeft.MainGroup == groupName)
            //    //{
            //    //    def.Usage.LeftTopGroup = def.Usage.LeftTopGroup.RemoveCSV("empty");
            //    //    def.Usage.LeftBottomGroup = def.Usage.LeftBottomGroup.RemoveCSV("empty");
            //    //}


                
            //}

          
            //TilesetEditor.GetOrOpen().LoadTileset(pnlMap.Tileset);
        }

        #endregion 

        #region Special Properties

        private void propSpecialTile_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            foreach (var tile in pnlMap.SelectedTiles())
            {
                e.ChangedItem.PropertyDescriptor.SetValue(tile, e.ChangedItem.Value);
                pnlMap.Map.UpdateSpecialInstance(tile);
            }
        }


        void SelectionGrid_SelectionChanged(object sender, EventArgs e)
        {
            var selectedTile = pnlMap.SelectedTiles().FirstOrDefault();
            if (selectedTile == null)
                return;

            var loc = selectedTile.TileLocation;
            var t = pnlMap.Map.GetTileAtCoordinates(loc.X, loc.Y);

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

        private void HandleObjectMouseClick(EditorGridPoint pt, ImageEventArgs args)
        {
            var objectUnderCursor = GetObjectFromPoint(args.Point.ImagePoint);

            if (args.Buttons == System.Windows.Forms.MouseButtons.Right)
                RemoveObject(objectUnderCursor);
            else if (args.Buttons == System.Windows.Forms.MouseButtons.Left)
            {
                if (objectUnderCursor == null && args.Action == MouseActionType.DoubleClick)                                    
                    PlaceObject(pt.ImagePoint);                
                else
                    this.CurrentObjectEntry = objectUnderCursor;
            }

        }

        private ObjectEntry GetObjectFromPoint(RGPointI point)
        {
            return this.WorldInfo.Objects.FirstOrDefault(p => p.PlacedObject.Area.Contains(point));
        }

        private void RemoveObject(ObjectEntry obj)
        {
            this.WorldInfo.Objects.Remove(obj);
            this.CurrentObjectEntry = null;
        }

        private void PlaceObject(RGPointI point)
        {
            this.CurrentObjectEntry = new ObjectEntry() { SpriteType = SelectedObjectType, Location=point }; ;
            this.WorldInfo.Objects.Add(this.CurrentObjectEntry);
            this.CurrentObjectEntry.CreateObject(new FixedLayer(new World(Program.EditorContext, this.WorldInfo)));
            pnlMap.RefreshImage();
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

        #region Panning

        private RGPointI mPanStartPos;
        private RGPointI mOriginalPan;
        private bool mIsPanning;

        private bool HandlePan(ImageEventArgs e)
        {

            if (!mIsPanning)
            {
                if (Control.ModifierKeys == Keys.Control && e.Action == MouseActionType.MouseDown && !mIsPanning)
                {
                    mIsPanning = true;
                    mPanStartPos = e.Point.ClientPoint;
                    mOriginalPan = pnlMap.ImagePanel.Pan;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if(Control.ModifierKeys == Keys.Control && e.Buttons == System.Windows.Forms.MouseButtons.Left)
                {
                    var offset = e.Point.ClientPoint.Difference(mPanStartPos);
                    pnlMap.ImagePanel.Pan = mOriginalPan.Offset(offset);
                    pnlMap.RefreshImage();

                    return true;
                }
                else
                {
                    mIsPanning=false;
                    return false ;
                }

            }
 
        }

        #endregion 

        private void lstGroups_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #region Tile Matching

        private List<TileInstance> mMatchTiles = new List<TileInstance>();

        private void HandleTileAlter(EditorGridPoint location, ImageEventArgs e)
        {
            var tile = pnlMap.Map.GetTileAtCoordinates(location.GridPoint.X, location.GridPoint.Y);
            Forms.frmLog.AddLine("Tile #" + tile.TileDef.TileID);

            var newTile = TileUsageHelper.GetNextTileReplacement(tile,new TileMatchInfo(pnlTileset.SelectedTiles().Select(p=>p.TileDef), mMatchTiles));
            if (newTile != null)
            {
                Forms.frmLog.AddLine("Chose Tile #" + newTile.TileID);

                pnlMap.Map.SetTile(location.GridPoint.X, location.GridPoint.Y, newTile.TileID);
                AddTileToMatchGroupIfNeeded(location.GridPoint);

                var tile2 = pnlMap.Map.GetTileAtCoordinates(location.GridPoint.X, location.GridPoint.Y);
                Forms.frmLog.AddLine("New Tile #" + tile2.TileDef.TileID);

            }
        }

        private void AddTileToMatchGroupIfNeeded(RGPointI tile)
        {
            if (!chkAutoMode.Checked)
                return;

            var instance = pnlMap.Map.GetTileAtCoordinates(tile.X, tile.Y);
            if(!mMatchTiles.Any(p=> p.Equals(instance)))
                mMatchTiles.Add(instance);
        }

        private void chkMatchGroup_CheckedChanged(object sender, EventArgs e)
        {
            mMatchTiles.Clear();
        }

        private void btnRandomize_Click(object sender, EventArgs e)
        {
            RandomizeAddedTiles();
        }

        private void RandomizeAddedTiles()
        {
            var i = new TileMatchInfo(pnlTileset.SelectedTiles().Select(p => p.TileDef), mMatchTiles);
            mMatchTiles = TileUsageHelper.RandomizeTiles(pnlMap.Map, i).ToList();
        }

        #endregion

     

    }
}
