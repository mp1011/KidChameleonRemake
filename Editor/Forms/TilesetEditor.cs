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
           // chkProperties.Items.Add(new PropertyBinding<TileDef> { Name = "Random Usage", Copy = (src, dest) => dest.Usage.RandomUsageWeight = src.Usage.RandomUsageWeight });
            chkProperties.Items.Add(new PropertyBinding<TileDef>
                {
                    Name = "Automatch Group",
                    Copy = (src, dest) => dest._AutomatchGroupForEditor = src._AutomatchGroupForEditor
                });


            pnlBase.SelectionMode = SelectionMode.Single;
            pnlTileset.SelectionMode = SelectionMode.Multi;

               
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
            {
                if (e.Buttons == System.Windows.Forms.MouseButtons.Left)
                    OnTileSelected();
                else if (e.Buttons == System.Windows.Forms.MouseButtons.Right)
                {
                    var pt = (e.Point as EditorGridPoint).GridPoint;

                    AddTileMatch(pnlTileset.ActiveMap.GetTileAtGridCoordinates(pt.X, pt.Y).TileDef);
                }
            }
            else if (e.Action == MouseActionType.Move)
            {
                var tile = pnlTileset.HighlightedTile;
                if (tile != null)
                    DisplayMatchPreview(pnlTileset.HighlightedTile.TileDef);
                else
                    DisplayMatchPreview(null);
            }
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

            LoadTileset(ts);
        }

        public void LoadTileset(TileSet ts)
        {
            mTileset = ts;
            pnlTileset.SetFromTileset(ts);
            InitGroupsTab();
            tileFilter.LoadData(pnlTileset);
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
            mLastSelectedTiles = this.SelectedTileDefs.ToArray();

            var selectedTile = pnlTileset.SelectedTiles().FirstOrDefault();
            if (selectedTile == null)
                return;

            var def = selectedTile.TileDef;


            tileProperties.SelectedObject = selectedTile.TileDef;
            tileProperties.Refresh();

            RefreshGroups(def);
            RefreshMatches(def);

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
            throw new NotImplementedException();
            //if(groups.Length == 0)
            //    pnlTileset.SetTiles(mTileset.CreateTileset(this.TransparentColor, "tiles", null), TilesPerRow);
            //else if (chkFilterExclusive.Checked)
            //    pnlTileset.SetTiles(mTileset.CreateTileset(this.TransparentColor, "tiles", t => t.Usage.ContainsOnlyTheseGroups(groups)), TilesPerRow);
            //else
            //    pnlTileset.SetTiles(mTileset.CreateTileset(this.TransparentColor, "tiles", t => t.Usage.ContainsAny(groups)), TilesPerRow);
        }
      
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LoadTileset();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Save();
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

        #region Matches

        private Map mMatchPreviewMap;
        private Map mMatchesMap;

        private Side CurrentMatchSide
        {
            get
            {
                return matchDirectionPicker.SelectedSide;
            }
        }

        private void InitGroupsTab()
        {
            mMatchPreviewMap = new Map(Program.EditorContext, new InMemoryResource<TileSet>(mTileset), 2, 2);
            mMatchesMap = new Map(Program.EditorContext, new InMemoryResource<TileSet>(mTileset), 5, 5);


            mMatchesMap.SetTile(0, 0, 1);
            
            pnlMatchPreview.SetFromMap(mMatchPreviewMap);
            pnlMatches.SetFromMap(mMatchesMap);
            DisplayMatchPreview(null);


            string[] allGroups = mTileset.GetTiles().SelectMany(p => p.Usage.Groups).Distinct().ToArray();
            lstGroups.Items.AddRange(allGroups);

            pnlMatches.ImagePanel.MouseAction += ImagePanel_MouseAction;
        }

        void ImagePanel_MouseAction(object sender, ImageEventArgs e)
        {
            var pt = (e.Point as EditorGridPoint).GridPoint;
            var tile = pnlMatches.ActiveMap.GetTileAtGridCoordinates(pt.X, pt.Y);

            if (e.Action == MouseActionType.Click && e.Buttons == System.Windows.Forms.MouseButtons.Right)
            {              
                RemoveTileMatch(tile.TileDef);
                RefreshMatches(SelectedTileDefs.FirstOrDefault());
            }
            else if (e.Action == MouseActionType.Move)
            {
                DisplayMatchPreview(tile.TileDef);
            }
        }

        private void DisplayMatchPreview(TileDef other)
        {
            if (mMatchPreviewMap == null)
                return;

            mMatchPreviewMap.SetTile(0, 0, TileDef.Blank.TileID);
            mMatchPreviewMap.SetTile(0, 1, TileDef.Blank.TileID);
            mMatchPreviewMap.SetTile(1, 0, TileDef.Blank.TileID);
            mMatchPreviewMap.SetTile(1, 1, TileDef.Blank.TileID);

            var current = this.SelectedTileDefs.FirstOrDefault();
            if(current == null)
                return;

            RGPointI thisTile = RGPointI.Empty;
            RGPointI otherTile = RGPointI.Empty;

            switch (CurrentMatchSide)
            {
                case Side.Left:
                    thisTile = new RGPointI(1, 0);
                    otherTile = new RGPointI(0, 0);
                    break;
                case Side.Right:
                    thisTile = new RGPointI(0, 0);
                    otherTile = new RGPointI(1, 0);
                    break;
                case Side.Top:
                    thisTile = new RGPointI(0, 1);
                    otherTile = new RGPointI(0, 0);
                    break;
                case Side.Bottom:
                    thisTile = new RGPointI(0, 0);
                    otherTile = new RGPointI(0, 1);
                    break;

            }

            mMatchPreviewMap.SetTile(thisTile.X, thisTile.Y, current.TileID);
            if(other !=null)
                mMatchPreviewMap.SetTile(otherTile.X, otherTile.Y, other.TileID);

            pnlMatchPreview.RefreshImage();
            
        }

        private void matchDirectionPicker_DirectionChanged(DirectionSelector sender, EventArgs args)
        {
            DisplayMatchPreview(null);
            RefreshMatches(this.SelectedTileDefs.FirstOrDefault());
        }

        private void RefreshMatches(TileDef tile)
        {

         
            //do this better later
            RGPointI cursor = RGPointI.Empty;

            if (tile != null)
            {
                foreach (var match in tile.Usage.GetMatches(CurrentMatchSide))
                {
                    mMatchesMap.SetTile(cursor.X, cursor.Y, match.TileID);
                    cursor.X++;
                    if (cursor.X >= mMatchesMap.TileDimensions.Width)
                    {
                        cursor.X = 0;
                        cursor.Y++;
                    }
                }
            }

            while (cursor.Y < mMatchesMap.TileDimensions.Height)
            {
                mMatchesMap.SetTile(cursor.X, cursor.Y, TileDef.Blank.TileID);
                cursor.X++;
                if (cursor.X >= mMatchesMap.TileDimensions.Width)
                {
                    cursor.X = 0;
                    cursor.Y++;
                }
            }

            pnlMatches.SelectionGrid.ShowGridLines = true;
            pnlMatches.RefreshImage();
        }

        private void AddTileMatch(TileDef match)
        {
            var tile = this.SelectedTileDefs.FirstOrDefault();
            if (tile == null)
                return;

            tile.Usage.AddMatch(this.CurrentMatchSide, match);
            match.Usage.AddMatch(this.CurrentMatchSide.Opposite(), tile);
            
            RefreshMatches(tile);
        }

        private void RemoveTileMatch(TileDef match)
        {
            var tile = this.SelectedTileDefs.FirstOrDefault();
            if (tile == null)
                return;

            tile.Usage.RemoveMatch(this.CurrentMatchSide,match);
            match.Usage.RemoveMatch(this.CurrentMatchSide.Opposite(), tile);
            RefreshMatches(tile);
        }

        #endregion

        #region Groups

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            if (txtNewGroup.Text.IsNullOrEmpty())
                return;

            lstGroups.Items.Add(txtNewGroup.Text, true);
            txtNewGroup.Text = String.Empty;
        }

        private void RefreshGroups(TileDef tile)
        {
            lstGroups.SetItemsChecked<string>(s=> tile.Usage.Groups.Contains(s));
        }

        private void lstGroups_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool isChecked = lstGroups.GetItemChecked(e.Index,e);
            string group = lstGroups.Items[e.Index].ToString();

            foreach (var tile in this.SelectedTileDefs)
            {
                if (isChecked)
                    tile.Usage.Groups.Add(group);
                else
                    tile.Usage.Groups.Remove(group);
            }

        }

      

      

        #endregion

        private void btnAutoOrganize_Click(object sender, EventArgs e)
        {
           // var newSet =  TileUsageHelper.AutoOrganize(mTileset);
          //  LoadTileset(newSet);
        }

        private void btnGroupTogether_Click(object sender, EventArgs e)
        {
            List<TileDef> oldOrder = new List<TileDef>(mTileset.GetTiles());
            List<TileDef> newOrder = new List<TileDef>();

            oldOrder.Transfer(newOrder, oldOrder.TakeWhile(p => !SelectedTileDefs.Contains(p)));
            oldOrder.Transfer(newOrder, SelectedTileDefs);
            oldOrder.Transfer(newOrder, oldOrder);

            var ts = new TileSet(mTileset.Texture, mTileset.TileSize, newOrder);
            LoadTileset(ts);
        }

        private void btnAddSelected_Click(object sender, EventArgs e)
        {

        }

        private void btnAutoDetect_Click(object sender, EventArgs e)
        {
            TileUsageHelper.AutoDetectMatches(mTileset);
        }

        private void btnMakeEqual_Click(object sender, EventArgs e)
        {
            TileUsageHelper.MakeEqual(this.SelectedTileDefs);
        }

    }
}
