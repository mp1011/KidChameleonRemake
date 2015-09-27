using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Engine;

namespace Editor.Forms
{
    partial class GraphicsExtractor : Form
    {
        public delegate void ImageChangedEventHandler(object sender, EventArgs e);
        public event ImageChangedEventHandler ImageChanged;

        public GraphicsExtractor()
        {
            InitializeComponent();

            pnlImage.ImagePanel.MouseAction += new ImagePanel.MouseActionEventHandler(pnlImage_ImageClicked);
        }

        public BitmapPortion[] ClipboardImages { get; set; }

        public BitmapPortion CurrentImage
        {
            get
            {

                return pnlImage.Image;
            }
            set
            {
                pnlImage.Image = value;
                ResetOverlay();      
            }
        }

        public ImageSelector CurrentImageSelector
        {
            get;
            set;
        }

        private void ResetOverlay()
        {
            this.pnlImage.ClearOverlay();

            if (CurrentAction == EditorAction.SetOrigin)
            {
                var originDot = new OverlayCircle(pnlImage.ImagePanel, p => BitmapPortionPoint.FromRegionPoint(p.Image.Origin, pnlImage.ImagePanel));
                originDot.Pen = new Pen(Color.DarkGray);
                originDot.Brush = new SolidBrush(Color.LightGreen);
                pnlImage.AddOverlayItem(originDot);
            }

            if (CurrentAction == EditorAction.SetHitbox)
            {
                var hitboxRec = new OverlayRectangle(pnlImage.ImagePanel, p => EditorRectangle.FromRegionRec(p.Image.PrimaryHitbox, pnlImage.ImagePanel));
                hitboxRec.Pen = new Pen(Color.Turquoise);
                pnlImage.AddOverlayItem(hitboxRec);
            }

            if (CurrentAction == EditorAction.SetSecondaryHitbox)
            {
                var hitboxRec = new OverlayRectangle(pnlImage.ImagePanel, p => EditorRectangle.FromRegionRec(p.Image.SecondaryHitbox, pnlImage.ImagePanel));
                hitboxRec.Pen = new Pen(Color.Turquoise);
                pnlImage.AddOverlayItem(hitboxRec);
            }

            var cropRec = new OverlayRectangle(pnlImage.ImagePanel, p =>
            {
                var crop = pnlImage.Image.Region;
                crop = RGRectangleI.FromXYWH(0, 0, crop.Width, crop.Height);
                return EditorRectangle.FromRegionRec(crop, pnlImage.ImagePanel);
            });
            cropRec.Pen = new Pen(Color.Orange);
            pnlImage.AddOverlayItem(cropRec);
            
        }
    
        #region Actions

        private enum EditorAction
        {
            None,
            PickTransparentColor,
            PickTile,
            PickSprite,
            Crop,
            SetOrigin,
            SetHitbox,
            SetSecondaryHitbox,
            FindTile
        }

        private Color TransparentColor
        {
            get
            {
                return pnlTransparentColor.BackColor;
            }
            set
            {
                pnlTransparentColor.BackColor = value;
            }
        }

        private int TileSize
        {
            get
            {
                return (int)txtRectangleIncrease.Value;
            }
            set
            {
                txtRectangleIncrease.Value = value;
            }
        }
        private EditorAction CurrentAction
        {
            get
            {
                if (pnlTransparentColor.Tag != null)
                    return EditorAction.PickTransparentColor;

                if (rdoPickSprite.Checked) return EditorAction.PickSprite;
                if (rdoPickTile.Checked) return EditorAction.PickTile;
                if (rdoCrop.Checked) return EditorAction.Crop;
                if (rdoSetOrigin.Checked) return EditorAction.SetOrigin;
                if (rdoHitbox.Checked) return EditorAction.SetHitbox;
                if (rdoHitbox2.Checked) return EditorAction.SetSecondaryHitbox;
                if (rdoFindTiles.Checked) return EditorAction.FindTile;

                return EditorAction.None;
            }
            set
            {
                pnlTransparentColor.Tag = null;
                pnlImage.DrawRectangle = DrawRectangleType.None;
                switch (value)
                {
                    case EditorAction.PickSprite: rdoPickSprite.Checked = true; return;
                    case EditorAction.PickTile: rdoPickTile.Checked = true; return;
                    case EditorAction.PickTransparentColor: pnlTransparentColor.Tag = true; rdoNoAction.Checked = true; return;
                    case EditorAction.Crop: rdoCrop.Checked = true; pnlImage.DrawRectangle = DrawRectangleType.Drag; return;
                    case EditorAction.SetOrigin: rdoSetOrigin.Checked = true; return;
                    case EditorAction.SetHitbox: rdoHitbox.Checked = true; return;
                    case EditorAction.SetSecondaryHitbox: rdoHitbox2.Checked = true; return;
                    case EditorAction.FindTile: rdoFindTiles.Checked = true; pnlImage.DrawRectangle = DrawRectangleType.Drag; return;
                }

                this.ResetOverlay();
            }
        }

        private void pnlTransparentColor_Click(object sender, EventArgs e)
        {
            CurrentAction = EditorAction.PickTransparentColor;
        }

        private IEnumerable<BitmapPortion> ImagesForAction
        {
            get
            {
                if (chkContinueAction.Checked)
                    return CurrentImageSelector.SelectedImages;
                else
                    return new BitmapPortion[] { CurrentImageSelector.CurrentImage };
            }
        }
        private void PerformAction(BitmapPortionPoint point)
        {
            switch (CurrentAction)
            {
                case EditorAction.PickTransparentColor: PickTransparentColor(point.ImagePoint);  break;
                case EditorAction.PickTile: PickTile(point.ImagePoint); break;
                case EditorAction.PickSprite: PickSprite(point.ImagePoint); break;
                case EditorAction.SetOrigin: SetOrigin(point); break;
            }

            if (this.ImageChanged != null)
                this.ImageChanged(this, new EventArgs());
        }

        private void PerformAction(RGRectangleI imageRec)
        {
            switch (CurrentAction)
            {
                case EditorAction.Crop: CropImage(imageRec); break;
                case EditorAction.SetHitbox: SetHitbox(imageRec, HitboxType.Primary); break;
                case EditorAction.SetSecondaryHitbox: SetHitbox(imageRec, HitboxType.Secondary); break;
                case EditorAction.FindTile: FindTiles(imageRec); break;
            }

            if (this.ImageChanged != null)
                this.ImageChanged(this, new EventArgs());
        }

      
        private void SetOrigin(BitmapPortionPoint point)
        {
            foreach(var img in this.ImagesForAction)
                img.Origin = point.RegionPoint;

            pnlImage.RefreshImage();
        }

        private void SetHitbox(RGRectangleI imageRec, HitboxType hitboxType)
        {
            var regionRec = RGRectangleI.Create(imageRec.TopLeft.RelativeTo(CurrentImage.Region.TopLeft), imageRec.Size);

            var offsetX = CurrentImage.Origin.X - regionRec.Left;
            var offsetY = CurrentImage.Origin.Y - regionRec.Top;
            foreach(var img in this.ImagesForAction)            
                img.SetHitbox(hitboxType, RGRectangleI.FromXYWH(img.Origin.X - offsetX, img.Origin.Y - offsetY, regionRec.Width, regionRec.Height));

            pnlImage.DrawRectangle = DrawRectangleType.None;
            pnlImage.DrawRectangle = DrawRectangleType.Drag;
        }

        private void PickTransparentColor(RGPointI pt)
        {
            TransparentColor = CurrentImage.GetPixel(pt.X, pt.Y); 
            CurrentAction = EditorAction.None;

            foreach (var image in ImagesForAction)
                image.MakeTransparent(TransparentColor);           
        }

        private void PickTile(RGPointI pt)
        {
            var img = CurrentImage;
            int x = pt.X;
            int y=  pt.Y;

            var nextPixel = dirNextPixel.SelectedDirection.ToPoint();

            if (nextPixel.Y < 0)
            {
                while (y > 0 && y < img.Height-1 && !img.GetPixel(x, --y).IsTransparent());
                y++;
            }
            else if (nextPixel.Y > 0)
            {
                while (y > 0 && y < img.Height - 1 && !img.GetPixel(x, ++y).IsTransparent()) ;
                //y--;
            }

            if (nextPixel.X < 0)
            {
                while (x > 0 && x < img.Width - 1 && !img.GetPixel(--x, pt.Y).IsTransparent()) ;
                x++;
            }
            else if (nextPixel.X > 0)
            {
                while (x > 0 && x < img.Width - 1 && !img.GetPixel(++x, pt.Y).IsTransparent()) ;
                //x--;
            }

            if (nextPixel.Y == 0)
                y = 0;
            if (nextPixel.X == 0)
                x = 0;

            TilesetEditor.GetOrOpen().AddNewTiles(CurrentImage.ExtractTiles(new RGPointI(x, y), this.TileSize));
        }

        private void PickSprite(RGPointI pt)
        {
            int minResolution = (int)txtFloodFillRes.Value;
            var nextPixelOffset = dirNextPixel.SelectedDirection.ToPoint();

            foreach (var img in this.ImagesForAction)
            {
                if (!img.FloodFillExtract(pt, minResolution))
                {
                    if (nextPixelOffset.IsEmpty)
                        break;

                    while (img.GetPixel(pt.X,pt.Y,true).IsTransparent() && img.Region.Contains(pt))
                        pt = new RGPointI(pt.X + nextPixelOffset.X, pt.Y + nextPixelOffset.Y);

                    if (!img.FloodFillExtract(pt, minResolution))
                        break;
                }

                Crop(img);
            }

           
        }

        private void FindTiles(RGRectangleI searchArea)
        {
            TilesetEditor.GetOrOpen().FindTiles(CurrentImage, searchArea);
        }


        private void CropImage(RGRectangleI imageRec)
        {
            foreach (var img in this.ImagesForAction)
            {
                if (!chkCropToNew.Checked)
                    img.Crop(imageRec);
                else
                {
                    var copy = CurrentImageSelector.CloneImage(img);
                    copy.Crop(imageRec);
                }
            }
        }

        #endregion

        #region Events

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var loadedItem = FileDialog.ShowOpenDialog(Paths.ScreenshotsPath,"bmp;png", filename => new { Path = System.IO.Path.GetDirectoryName(filename), Image = new BitmapPortion(filename) }).Data;
            if (loadedItem == null)
                return;

            ImageSelector.Open(this, loadedItem.Image,loadedItem.Path);
        }

        private void newSpriteSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ImageSelector(this).Show();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //TBD prompt save
            OpenSpriteSheet();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSpriteSheet();
        }

        private void openTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTileset();
        }

        private void saveTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTileset();
        }

        private void btnPreviewAnimation_Click(object sender, EventArgs e)
        {
            var animPreview = new AnimationPreview(CurrentImageSelector.SelectedImages);
            animPreview.ShowDialog();
        }

        private OverlayCross mCross;
        private void pnlImage_ImageClicked(object sender, ImageEventArgs e)
        {
            if (e.Action == MouseActionType.RectangleSelection)
            {
                var rectangleEventArgs = e as DrawRectangleEventArgs;
                if (rectangleEventArgs != null)
                    PerformAction(rectangleEventArgs.ImageRectangle);
                return;
            }

            if (e.Buttons != System.Windows.Forms.MouseButtons.Left || e.Action != MouseActionType.Click)
                return;

            if (mCross == null)
                mCross = new OverlayCross { Pen = new Pen(Color.Red, 2f) };

            mCross.Location = e.Point;
            pnlImage.AddOverlayItem(mCross);

            PerformAction(e.Point as BitmapPortionPoint);
        }

        #endregion

        #region Loading / Saving

        private void OpenSpriteSheet()
        {
            var sheet = FileDialog.ShowLoad<SpriteSheet>(PathType.SpriteSheets).Data;
            if (sheet == null)
                return;
                     
            new ImageSelector(this, sheet).Show();
        }

        private void SaveSpriteSheet()
        {
            if (CurrentImageSelector == null)
                return;

            var images = CurrentImageSelector.SelectedImagesInOriginalOrder;

            if (MessageBox.Show("Save " + images.Count() + " cell(s)?","Save?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            FileDialog.ShowSaveDialog<SpriteSheet>(PathType.SpriteSheets, selectedFile =>
                {
                    var name = Path.GetFileNameWithoutExtension(selectedFile);
                    var ss = BitmapPortion.CreateSpriteSheet(images, 320, name, this.TransparentColor);

                    var bitmap = ss.Image.GetImage();
                    bitmap.Save(ss.Image.Path.FullPath);
                    BackupManager.CreateBackup(ss.Image.Path.FullPath);

                    if (chkCreateFlashTexture.Checked)
                    {
                        var flash = bitmap.CreateFlash();
                        flash.Save(ss.Image.Path.FullPath.Replace(".png", "_flash.png"));
                    }

                    
                    return ss;
                });

        }
      
        private void OpenTileset()
        {
            TilesetEditor.GetOrOpen().LoadTileset();
        }

        private void SaveTileset()
        {
            var ts = TilesetEditor.GetInstance();
            if (ts == null)
                return;

            ts.Save();
        }

        #endregion

        private void rdoWithRec_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCrop.Checked || rdoHitbox.Checked || rdoHitbox2.Checked || rdoFindTiles.Checked)
                pnlImage.DrawRectangle = DrawRectangleType.Drag;
            else
                pnlImage.DrawRectangle = DrawRectangleType.None;
            this.ResetOverlay();
        }

        private void chkShowRegion_CheckedChanged(object sender, EventArgs e)
        {
            pnlImage.DisplayPortionOnly = !chkShowRegion.Checked;
        }

        private void btnRectangleMoveSide_Click(object sender, EventArgs e)
        {
            MoveSides((int)txtMoveRecAmount.Value);
        }

        private void btnRectangleMoveSide2_Click(object sender, EventArgs e)
        {
            MoveSides(-((int)txtMoveRecAmount.Value));
        }

        private void btnMoveToSide_Click(object sender, EventArgs e)
        {
            foreach (var img in ImagesForAction)
            {
                switch (CurrentAction)
                {
                    case EditorAction.SetOrigin: img.Origin = AdjustPoint(img.Origin,img.PrimaryHitbox); break;
                }
            }

            pnlImage.RefreshImage();
        }

        private void MoveSides(int amount)
        {
            pnlImage.ImagePanel.ClearRectangle();

            foreach (var img in ImagesForAction)
            {
                switch (CurrentAction)
                {
                    case EditorAction.SetHitbox: img.PrimaryHitbox = AdjustRec(img.PrimaryHitbox, amount); break;
                    case EditorAction.SetSecondaryHitbox: img.SecondaryHitbox = AdjustRec(img.SecondaryHitbox, amount); break;
                    case EditorAction.Crop: img.Crop(AdjustRec(img.Region,amount)); break;
                    case EditorAction.SetOrigin: AdjustOriginAndHitbox(img,amount); break;
                }
            }


            pnlImage.RefreshImage();
        }

        private RGRectangleI AdjustRec(RGRectangleI rec, int amount)
        {
            var flags = dirNextPixel.Flags;
            var moveAmt = (int)txtMoveRecAmount.Value;

            if (flags.Left)
                rec.Left -= amount;
            if (flags.Right)
                rec.Right += amount;
            if (flags.Up)
                rec.Top -= amount;
            if (flags.Down)
                rec.Bottom += amount;

            return rec;
        }

        private void btnCropToSeam_Click(object sender, EventArgs e)
        {
            foreach (var img in this.ImagesForAction)
                CropToSeam(img);
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            foreach (var img in this.ImagesForAction)
                Crop(img);
        }

        private RGPointI AdjustPoint(RGPointI src, RGRectangleI rec)
        {
            var flags = dirNextPixel.Flags;

            if (flags.Right)
                src.X = rec.Right;
            else if (flags.Left)
                src.X = rec.Left;

            if (flags.Up)
                src.Y = rec.Top;
            else if (flags.Down)
                src.Y = rec.Bottom;

            return src;

       }

        private void AdjustOriginAndHitbox(BitmapPortion img, int amount)
        {
            var flags = dirNextPixel.Flags;
            int offX=0,offY=0;

            if (flags.Right)
                offX = 1 * amount;            
            else if (flags.Left)
                offX = -1 * amount;           
            if (flags.Up)
                offY = -1 * amount;            
            else if (flags.Down)
                offY = 1 * amount;

            img.Origin = img.Origin.Offset(offX, offY);
            img.PrimaryHitbox = img.PrimaryHitbox.Offset(offX, offY);

        }

        private void Crop(BitmapPortion img)
        {
            RGRectangleI newCrop = RGRectangleI.FromTLBR(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
         //  img.Crop(RGRectangleI.FromXYWH(0, 0, img.Width, img.Height));

            for (int y = img.Region.Top; y <= img.Region.Bottom; y++)
            {
                for (int x = img.Region.Left; x <= img.Region.Right; x++)
                {
                    if (img.GetPixel(x, y,true).IsTransparent())
                        continue;

                    if (x < newCrop.Left) newCrop.Left = x;
                    if (x > newCrop.Right) newCrop.Right = x;
                    if (y < newCrop.Top) newCrop.Top = y;
                    if (y > newCrop.Bottom) newCrop.Bottom = y;
                }
            }

            newCrop.Right++;
            newCrop.Bottom++;
            img.Crop(newCrop);
            pnlImage.RefreshImage();
        }

        private void CropToSeam(BitmapPortion img)
        {
            int x = img.Region.Left+1;
            pnlImage.ImagePanel.ClearRectangle();

            while (x < img.Region.Right)
            {
                if (CheckSeam(img, x))
                {
                    var r = img.Region;
                    img.Crop(RGRectangleI.FromTLBR(r.Top, r.Left, r.Bottom, x));
                }
                x++;
            }

            pnlImage.RefreshImage();
        }

        private bool CheckSeam(BitmapPortion img, int position)
        {
            var cursor1 = img.Region.TopLeft;
            var cursor2 = new RGPointI(position, cursor1.Y);

            while (true)
            {
                if(cursor2.X >= img.Region.Right)
                    return true;

                if(!img.GetPixel(cursor1.X,cursor1.Y).Equals(img.GetPixel(cursor2.X,cursor2.Y)))
                    return false;

                cursor1.Y++;
                cursor2.Y++;

                if(cursor2.Y >= img.Region.Bottom)
                {
                    cursor1.X++;
                    cursor1.Y = img.Region.Top;

                    cursor2.X++;
                    cursor2.Y = cursor1.Y;
                }                
            }
        }

        private void GraphicsExtractor_KeyUp(object sender, KeyEventArgs e)
        {          
            if (e.KeyCode == Keys.Right)
                this.CurrentImageSelector.SelectNext();
            else if (e.KeyCode == Keys.Left)
                this.CurrentImageSelector.SelectPrevious();            
        }

        private void chkCenterOrigin_CheckedChanged(object sender, EventArgs e)
        {
            pnlImage.CenterOnOrigin = chkCenterOrigin.Checked;
            pnlImage.Refresh();
        }

        private void rdoSetOrigin_CheckedChanged(object sender, EventArgs e)
        {
            this.ResetOverlay();
        }

        #region Grid Collapse
        private void btnCollapseGrid_Click(object sender, EventArgs e)
        {
            var diff = (int)gridCollapseActual.Value - (int)gridCollapseDesired.Value;

            var tiles = this.CurrentImage.ExtractGrid((int)gridCollapseActual.Value).ToArray();
            foreach(var tile in tiles)
                tile.Crop(RGRectangleI.FromXYWH(tile.Region.X, tile.Region.Y, tile.Region.Width - diff, tile.Region.Height - diff));
            
            TilesetEditor.GetOrOpen().AddNewTiles(tiles);

        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            var tiles = this.CurrentImage.ExtractGrid(16);
            var sel = new ImageSelector(this);
            sel.AddImages(tiles);
            sel.Show();
           
        }

    }
}

