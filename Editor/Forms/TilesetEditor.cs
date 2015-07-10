using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Editor.Forms;
using System.IO;
using Engine.Collision;

namespace Editor.Forms
{
    partial class TilesetEditor : Form
    {
        #region Instance

        private static TilesetEditor mInstance;

        public static TilesetEditor GetOrOpen()
        {
            if (mInstance == null || mInstance.IsDisposed)
            {
                mInstance = new TilesetEditor();
                mInstance.Show();
            }

            return mInstance;
        }

        public static TilesetEditor GetInstance()
        {
            if (mInstance == null || mInstance.IsDisposed)
                return null;
         
            return mInstance;
        }

#if DESIGNMODE
        public TilesetEditor()
        {       
          InitializeComponent();
        }
#endif

        #endregion

        private TileSet mTileset;
              
        private TilesetEditor()
        {
            mTileset = new TileSet();
            InitializeComponent();

            pnlBase.SetFromTileset(TileSet.CreateBase());
            pnlBase.RefreshImage();

            pnlTileset.ImagePanel.MouseAction += new ImagePanel.MouseActionEventHandler(pnlTileset_ImageClicked);

            chkProperties.Items.Add(new PropertyBinding<TileDef> { Name = "Flags", Copy = (src, dest) => dest.SetValues(null,src.Flags,null,null)});
            chkProperties.Items.Add(new PropertyBinding<TileDef> { Name = "Sides", Copy = (src, dest) => dest.SetValues(null, null, src.Sides, null) });
            chkProperties.Items.Add(new PropertyBinding<TileDef>
            {
                Name = "Group",
                Copy = (src, dest) =>
                    {
                        //TODO    dest.Groups = src.Groups;     
                    }
            });
            chkProperties.Items.Add(new PropertyBinding<TileDef> { Name = "Random Usage", Copy = (src, dest) => dest.Usage.RandomUsageWeight = src.Usage.RandomUsageWeight });

            pnlBase.SelectionMode = SelectionMode.Single;
            pnlTileset.SelectionMode = SelectionMode.Multi;

            foreach (var textbox in new TextBox[] { txtGroupTopLeft, txtGroupTopRight, txtGroupRightTop, txtGroupRightBottom, txtGroupBottomRight, txtGroupBottomLeft,
                txtGroupLeftBottom,txtGroupLeftTop})
                textbox.DoubleClick += new EventHandler(txtGroup_DoubleClick);
        }

        private class PropertyBinding<TObject>
        {
            public string Name { get; set; }

            public Action<TObject, TObject> Copy;

            public override string ToString()
            {
                return Name;
            }
        }

        void pnlTileset_ImageClicked(object sender, ImageEventArgs e)
        {                  
            if (e.Action == MouseActionType.Click)
                OnTileSelected();
        }

        private void pnlBase_Load(object sender, EventArgs e)
        {
            pnlBase.RefreshImage();
        }

       
        
        public void LoadTileset()
        {
            var ts = FileDialog.ShowLoad<TileSet>(PathType.Tilesets).Data;
            if (ts == null)
                return;

            mTileset = ts;
            pnlTileset.SetFromTileset(ts);     
        }

        public void LoadTileset(TileSet ts)
        {
            mTileset = ts;
            pnlTileset.SetFromTileset(ts);
        }

        public void Save()
        {
            FileDialog.ShowSaveDialog<TileSet>(PathType.Tilesets, selectedFile =>
            {
                var bitmap = mTileset.Texture.GetImage();

                mTileset.Texture.Path.Name = System.IO.Path.GetFileNameWithoutExtension(selectedFile); 
                bitmap.Save(mTileset.Texture.Path.FullPath);
                BackupManager.CreateBackup(mTileset.Texture.Path.FullPath);
                return mTileset;
            });

        }


        #region Tile Selection

        private TileDef[] mLastSelectedTiles;

        private IEnumerable<TileDef> SelectedTileDefs
        {
            get
            {
                return pnlTileset.SelectedTiles().Select(p => p.TileDef);
            }
        }
        private void OnTileSelected()
        {
            if(!mLastSelectedTiles.ContainsAll(SelectedTileDefs))
                ApplyGroupsToCurrent();

            mLastSelectedTiles = this.SelectedTileDefs.ToArray();

            var selectedTile = pnlTileset.SelectedTiles().FirstOrDefault();
            if (selectedTile == null)
                return;

            var def = selectedTile.TileDef;


            tileProperties.SelectedObject = selectedTile.TileDef;
            tileProperties.Refresh();

            pnlTilePreview.Refresh();

            if(def != null)
                Forms.frmLog.AddLine("TileID=" + def.TileID);
        }

        #endregion


        private void timer1_Tick(object sender, EventArgs e)
        {
            pnlBase.RefreshImage();
            pnlTileset.RefreshImage();

            timer1.Enabled = false;
        }

        private void TilesetEditor_Resize(object sender, EventArgs e)
        {
            pnlTileset.SetFromTileset(pnlTileset.Tileset);
        }

        #region Actions

        private void btnGroup_Click(object sender, EventArgs e)
        {
            var tiles = pnlTileset.SelectedTiles().ToArray();


        }

        private void btnCopyProperties_Click(object sender, EventArgs e)
        {
            var baseTile = pnlBase.SelectedTiles().FirstOrDefault();
            if (baseTile == null)
                return;

            throw new NotImplementedException();
           // var baseEditorTile = new EditorTile(null, baseTile.TileDef);

          //  CopyProperties(baseEditorTile);
        }

        private void btnCopyFromSelected_Click(object sender, EventArgs e)
        {
            var selectedTile = tileProperties.SelectedObject as TileDef;
            if (selectedTile == null)
                return;

            CopyProperties(selectedTile);
        }


        private void CopyProperties(TileDef source)
        {
            foreach (var tile in pnlTileset.SelectedTiles())
            {
                var editorTile = mTileset.GetTiles().FirstOrDefault(p => p.TileID == tile.TileDef.TileID);
                if (editorTile == null)
                    continue;

                foreach (var item in chkProperties.CheckedItems)
                {
                    var prop = item as PropertyBinding<TileDef>;
                    prop.Copy(source, editorTile);
                }
            }
        }




        #endregion

        private void txtGroup_TextChanged(object sender, EventArgs e)
        {
            RefreshFilter();
        }

        private void chkFilterExclusive_CheckedChanged(object sender, EventArgs e)
        {
            RefreshFilter();
        }

        private void RefreshFilter()
        {
            var groups = txtGroup.Text.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);

            throw new NotImplementedException();
            //if(groups.Length == 0)
            //    pnlTileset.SetTiles(mTileset.CreateTileset(this.TransparentColor, "tiles", null), TilesPerRow);
            //else if (chkFilterExclusive.Checked)
            //    pnlTileset.SetTiles(mTileset.CreateTileset(this.TransparentColor, "tiles", t => t.Usage.ContainsOnlyTheseGroups(groups)), TilesPerRow);
            //else
            //    pnlTileset.SetTiles(mTileset.CreateTileset(this.TransparentColor, "tiles", t => t.Usage.ContainsAny(groups)), TilesPerRow);
        }

        private void pnlTilePreview_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (pnlTileset.SelectedTiles().IsNullOrEmpty())
                {
                    e.Graphics.Clear(Color.Black);
                    return;
                }

                var tile = pnlTileset.SelectedTiles().FirstOrDefault().TileDef;
                if (tile == null)
                    return;

                e.Graphics.SetBlockyScaling();
                e.Graphics.DrawImage(pnlTileset.Tileset.Texture.GetImage(), new Rectangle(0, 0, pnlTilePreview.Width, pnlTilePreview.Height), tile.SourcePosition.ToSystemRec(), GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                frmLog.AddLine(ex.Message);
            }
        }

        private void tileProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Groups")
            {
                if(!e.OldValue.Equals(e.ChangedItem.Value))
                    UpdateGroupTextBoxes();
            }
        }

       

        private void tileProperties_SelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateGroupTextBoxes();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LoadTileset();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        #region Tile Groups

        private TextBox[] mGroupBoxes;

        private void AssignTextboxSides()
        {
            txtGroupLeftTop.Tag = GroupSide.LeftTop;
            txtGroupTopLeft.Tag = GroupSide.TopLeft;
            txtGroupTopRight.Tag = GroupSide.TopRight;
            txtGroupRightTop.Tag = GroupSide.RightTop;
            txtGroupRightBottom.Tag = GroupSide.RightBottom;
            txtGroupBottomRight.Tag = GroupSide.BottomRight;
            txtGroupBottomLeft.Tag = GroupSide.BottomLeft;
            txtGroupLeftBottom.Tag = GroupSide.LeftBottom;

            mGroupBoxes = new TextBox[] { txtGroupLeftTop, txtGroupTopLeft, txtGroupTopRight, txtGroupRightTop, txtGroupRightBottom, txtGroupBottomRight, txtGroupBottomLeft, txtGroupLeftBottom};
        }

        private TextBox GetGroupBox(GroupSide side)
        {
            return mGroupBoxes.First(p => p.Tag.Equals(side));
        }

        private void txtDblClickGroup_DoubleClick(object sender, EventArgs e)
        {
            foreach(var box in mGroupBoxes)
                AddDoubleClickGroup(box);         
        }

        private void txtGroup_DoubleClick(object sender, EventArgs e)
        {
            AddDoubleClickGroup(sender as TextBox);
        }

        private void AddDoubleClickGroup(TextBox box)
        {
            var items = box.Text.Split(',');
            if (!items.Contains(txtDblClickGroup.Text))
            {
                if (box.Text.IsNullOrEmpty())
                    box.Text = txtDblClickGroup.Text;
                else
                    box.Text += "," + txtDblClickGroup.Text;
            }
        }

        private void UpdateGroupTextBoxes()
        {
            var selectedTile = pnlTileset.SelectedTiles().FirstOrDefault();
            if (selectedTile == null)
                return;

            var usg = selectedTile.TileDef.Usage;

            foreach (var textbox in mGroupBoxes)
                textbox.Text = usg.SideGroups.TryGet((GroupSide)textbox.Tag, String.Empty);
        }

        private void ApplyGroupsToCurrent()
        {
            foreach (var tileDef in mLastSelectedTiles.NeverNull())
            {             
                foreach (var textbox in mGroupBoxes)
                    tileDef.Usage.SideGroups.AddOrSet((GroupSide)textbox.Tag, textbox.Text);
            }
        }


        #endregion

        private void TilesetEditor_Load(object sender, EventArgs e)
        {
            AssignTextboxSides();
        }

        private void pnlTileset_Load(object sender, EventArgs e)
        {

        }

        #region Add Tiles

        public void FindTiles(BitmapPortion image, RGRectangleI searchArea)
        {
            if (this.SelectedTileDefs.Count() != 1)
            {
                MessageBox.Show("Please select a single tile", "Editor", MessageBoxButtons.OK);
                return;
            }

            var offset = FindTileOffset(image, searchArea);
            if (!offset.HasValue)
                return;

            var tiles = image.ExtractTiles(offset.Value, mTileset.TileSize.Width);
            AddNewTiles(tiles);
        }

        private RGPointI? FindTileOffset(BitmapPortion image, RGRectangleI searchArea)
        {
            
            BitmapPortion selectedTile = GetBitmapPortionFromTileDef(this.SelectedTileDefs.First(), mTileset.Texture.GetImage());
            RGRectangleI tileArea = RGRectangleI.FromXYWH(searchArea.X, searchArea.Y, mTileset.TileSize.Width, mTileset.TileSize.Height);
            BitmapPortion possibleTile = image.CropToNew(tileArea);

            while (true)
            {
                if (possibleTile.PixelsEqual(selectedTile))
                    return tileArea.TopLeft;

                tileArea.Left++;
                tileArea.Right++;
                if (tileArea.Right >= searchArea.Right)
                {
                    tileArea.Left = 0;
                    tileArea.Width = mTileset.TileSize.Width;

                    tileArea.Top++;
                    tileArea.Bottom++;

                    if (tileArea.Bottom >= searchArea.Bottom)
                        return null;
                }

                possibleTile = image.CropToNew(tileArea);
            }
        }

        public void AddNewTiles(IEnumerable<BitmapPortion> newTiles)
        {
            int id = mTileset.GetTiles().MaxOrDefault(t => t.TileID, 0);
            var newEditorTiles = newTiles.Select(p => new { Image = p, TileDef = new TileDef(TileFlags.Passable, ++id, 0, RGPoint.Empty, DirectionFlags.None) });

            var allTiles = newEditorTiles.ToArray(); 

            if (mTileset.Texture != null)
            {
                var currentTileImage = mTileset.Texture.GetImage();
                var currentTiles = mTileset.GetTiles().Select(p => new { Image = GetBitmapPortionFromTileDef(p, currentTileImage), TileDef = p });

                allTiles = currentTiles.Union(newEditorTiles).ToArray();
            }

            allTiles = allTiles.Distinct((a, b) => a.Image.PixelsEqual(b.Image), a => a.Image.GetHashCode()).ToArray();

            var img = BitmapPortion.CreateSpriteSheet(allTiles.Select(p => p.Image), 320, "tiles", Color.Transparent);
            var img2 = new BitmapPortion(img.Image.GetImage().CloneImage());

            id = 0;
            foreach (var tile in allTiles)
                tile.TileDef.SetValues(++id, null, null, img.Frames[id - 1].Source);

            mTileset = new TileSet(img, allTiles.Select(p => p.TileDef));
            pnlTileset.SetFromTileset(mTileset);
            pnlTileset.RefreshImage();

        }

        private BitmapPortion GetBitmapPortionFromTileDef(TileDef t, Bitmap tileImage)
        {
            var bmp= new BitmapPortion(tileImage);
            bmp.Crop(t.SourcePosition);
            return bmp;
        }
      
  


        #endregion

    }
}
