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

            this.MapInfo.TilesetName = "woods";
            this.MapInfo.ScreensWidth = 3;
            this.MapInfo.ScreensHeight = 1;

            pnlTileset.SelectionMode = SelectionMode.Single;

            pnlMap.ImagePanel.MouseAction += new ImagePanel.MouseActionEventHandler(Map_MouseAction);
            pnlMap.SelectionGrid.SelectionChanged += new EventHandler(SelectionGrid_SelectionChanged);

            cboObject.DataSource = this.PlaceableObjectTypes;
         
            try
            {
                var lastMap = System.IO.File.ReadAllText("lastmap");
                if(lastMap.NotNullOrEmpty())
                    LoadMap(GameResource<Map>.Load(new GamePath(PathType.Maps,lastMap), Program.EditorContext));
            }
            catch
            {

            }
        }

     

        private MapInfo MapInfo
        {
            get
            {
                if (mapProperties.SelectedObject == null)
                    mapProperties.SelectedObject = new MapInfo();

                return mapProperties.SelectedObject as MapInfo;
            }
        }


    
        private OverlayRectangle mCursor;


        private void mapProperties_Click(object sender, EventArgs e)
        {

        }

        private void btnApplyMapInfo_Click(object sender, EventArgs e)
        {
            var map = this.MapInfo.CreateMap(pnlMap.Map);
            pnlTileset.SetFromTileset(map.Tileset);
            pnlMap.SetFromMap(map);
            mCursor =null;
        }

        #region Loading and Saving

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileDialog.ShowSaveDialog<Map>(PathType.Maps, pnlMap.Map);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var loadedFile= FileDialog.ShowLoad<Map>(PathType.Maps);
            System.IO.File.WriteAllText("lastmap", loadedFile.Name);
            LoadMap(loadedFile.Data);          
        }

        private void LoadMap(Map map)
        {
            if (map != null)
            {
                pnlTileset.SetFromTileset(map.Tileset);
                pnlMap.SetFromMap(map);
                mCursor = null;
                UpdateControls();
            }

        }

        #endregion

        private void tbZoom_Scroll(object sender, EventArgs e)
        {
    
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

     

        #region Actions

        private enum EditorAction
        {
            None = 0,
            Select = 1,
            Draw = 2
        }

        private EditorAction mCurrentAction;
        private EditorAction CurrentAction
        {
            get
            {
                if (chkDraw.Checked)
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
            }

            var gridPoint = (e.Point as EditorGridPoint).SnapToGrid();
            var originalCursorPos = mCursor.Area.TopLeft == null ? RGPointI.Empty : mCursor.Area.TopLeft.ImagePoint;

            mCursor.Area.TopLeft = gridPoint;
            mCursor.Area.BottomRight = gridPoint.OffsetGrid(1, 1);

            switch (CurrentAction)
            {
                case EditorAction.Draw: DrawTile(gridPoint, e); break;
                case EditorAction.Select: SelectTile(gridPoint, e); break;
            }
          
            if (e.Buttons != System.Windows.Forms.MouseButtons.None || !originalCursorPos.Equals(mCursor.Area.TopLeft.ImagePoint))
                pnlMap.RefreshImage();
        }


        private void DrawTile(EditorGridPoint gridPoint, ImageEventArgs e)
        {             
            var selectedTile = pnlTileset.SelectedTiles().FirstOrDefault();

            if (selectedTile != null && e.Buttons == System.Windows.Forms.MouseButtons.Left)
            {
                pnlMap.Map.SetTile(gridPoint.GridPoint.X, gridPoint.GridPoint.Y, selectedTile.TileDef.TileID);
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

        private void SelectTile(EditorGridPoint gridPoint, ImageEventArgs e)
        {

            if (e.Buttons == System.Windows.Forms.MouseButtons.Left)
            {
                pnlMap.SelectionGrid.SetSelection(gridPoint,true);
            }
            else if (e.Buttons == System.Windows.Forms.MouseButtons.Right)
            {
                pnlMap.SelectionGrid.SetSelection(gridPoint, false);
            }


        }


        private void btnRandomize_Click(object sender, EventArgs e)
        {
            var h = new TileUsageHelper();
            foreach (var tileInstance in pnlMap.SelectionGrid.SelectedItems())
            {
                h.RandomizeTile(tileInstance);
            }

            pnlMap.RefreshImage();
        }


        #endregion


        #region Groups

        private void UpdateControls()
        {
            lstGroups.Items.Clear();
            lstGroups.Items.Add("All");
            foreach (var group in pnlMap.Tileset.GetTiles().SelectMany(p => p.Usage.Groups).Distinct().ToArray())
            {
                if (String.IsNullOrEmpty(group))
                    continue;
                lstGroups.Items.Add(group, true);
            }
        }

      
        private void lstGroups_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var ts = pnlMap.Tileset;

            if(e.Index == 0)
            {              
                if(lstGroups.GetItemChecked(0,e))
                    pnlTileset.SetFromTileset(pnlMap.Tileset);

                return;
            }
            else 
            {
                lstGroups.SetItemChecked(0, false);
            }

            var groups = lstGroups.GetCheckedItems<string>(e);
            var filteredTilset = new TileSet(ts.Texture,ts.TileSize, ts.GetTiles().Where(p=> p.Usage.ContainsGroups(groups)));

            pnlTileset.SetFromTileset(filteredTilset);
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
        private ObjectType[]  PlaceableObjectTypes
        {
            get 
            {
                if(mPlaceableObjectTypes != null)
                    return mPlaceableObjectTypes;

                  mPlaceableObjectTypes = ReflectionHelper.GetAssembly(Program.EditorGame).GetTypesByAttribute<EditorVisibleAttribute>().SelectMany(p=>
                      p.GetPropertiesByAttribute<EditorVisibleAttribute>().Select(k=> (ObjectType)k.GetValue(null,null))).ToArray();

                return mPlaceableObjectTypes;
            }
        }

        private Bitmap GetObjectTypeImage(ObjectType t)
        {
            var layer = new FixedLayer(Program.EditorContext);
            var sprite = t.CreateInstance<Sprite>(layer, Program.EditorContext);
            sprite.Location = sprite.CurrentAnimation.CurrentDirectedAnimationFrame.Origin;

            var painter = new BitmapPainter();
            sprite.CurrentAnimation.Draw(painter, RGRectangleI.FromXYWH(0, 0, 1000, 1000));

            return painter.Image;
        }

        class BitmapPainter : Engine.Graphics.Painter
        {
            public Bitmap Image { get; private set; }

            public BitmapPainter() : base(Program.EditorContext) { }

            protected override void PaintToScreen(TextureResource texture, RGRectangleI source, RGRectangleI dest, RenderOptions options)
            {
                var textureImage = texture.GetImage();

                this.Image = new Bitmap(dest.Width, dest.Height);
                var g = Graphics.FromImage(this.Image);
                g.DrawImage(textureImage, dest.ToSystemRec(), source.ToSystemRec(), GraphicsUnit.Pixel);
                g.Dispose();

            }
        }

        private ObjectType SelectedObjectType
        {
            get { return (ObjectType)cboObject.SelectedValue; }
        }
        
        private void cboObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbObjectPreview.Image = GetObjectTypeImage(SelectedObjectType);
            UpdateObjectEntry(SelectedObjectType);
        }

        private ObjectEntry UpdateObjectEntry(ObjectType t)
        {
            var o = new ObjectEntry() { SpriteType = t };
            this.CurrentObjectEntry = o;
            return o;
        }

        private ObjectEntry CurrentObjectEntry
        {
            get
            {
               return (pgObject.SelectedObject as ObjectEntry) ?? UpdateObjectEntry(this.SelectedObjectType);
            }
            set
            {
                pgObject.SelectedObject = value;
            }
        }

        #endregion


    }
}
