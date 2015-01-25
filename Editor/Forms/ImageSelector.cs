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
    partial class ImageSelector : Form
    {
        public const int MaxImages = 200;

        private GraphicsExtractor mMainForm;
        private string mFolder;

        public ImageSelector(GraphicsExtractor mainForm)
        {
            InitializeComponent();
            mMainForm = mainForm;
            pnlLoadedImage.ImagePanel.MouseAction += new Editor.ImagePanel.MouseActionEventHandler(this.pnlLoadedImage_ImageClicked);

            pnlLoadedImage.ImageSelectionChanged += new BitmapPortionPanelUserControl.ImageSelectedEventHandler(pnlLoadedImage_ImageSelectionChanged);
            mainForm.ImageChanged += new GraphicsExtractor.ImageChangedEventHandler(mainForm_ImageChanged);

            pnlLoadedImage.Selected = false;
            pnlLoadedImage.SetAllowSelection(true);
        }

        private BitmapPortionPanelUserControl mLastSelected;
        private bool mInSelectionEvent=false;
        void pnlLoadedImage_ImageSelectionChanged(object sender, ImageSelectedEventArgs e)
        {
            if (mInSelectionEvent)
                return;

            try
            {
                mInSelectionEvent = true;

                if (e.Control)
                    return;

                if (!e.Shift)
                {
                    if (!chkKeepSelection.Checked)
                    {
                        if (e.Selected)
                        {
                            mLastSelected = sender as BitmapPortionPanelUserControl;

                            foreach (var panel in mLoadedImagePanels.Where(p => p != mLastSelected))
                                panel.Selected = false;
                        }
                        else
                        {
                            foreach (var panel in mLoadedImagePanels)
                                panel.Selected = false;
                        }
                    }
                    return;
                }
           
           
                var selectedControl = sender as BitmapPortionPanelUserControl;

                if (mLastSelected != null && mLastSelected.Selected)
                {
                    foreach (var panel in mLoadedImagePanels.SkipWhile(p => p != mLastSelected).TakeWhile(p => p != selectedControl))
                        panel.Selected = true;
                }

                if (e.Selected)
                    mLastSelected = sender as BitmapPortionPanelUserControl;

            }
            finally
            {
                mInSelectionEvent = false;
            }
        }

        void mainForm_ImageChanged(object sender, EventArgs e)
        {
            this.RefreshLoadedImages();
            this.CheckForConsecutiveDuplicates();
        }

        #region Constructor

        public static void Open(GraphicsExtractor mainForm, BitmapPortion image, string folder)
        {
            var images = new List<BitmapPortion>();
            while (image != null)
            {
                if(image != null)
                    images.Add(image);

                image = image.LoadNext();               
            }

            var formImages = images.Take(ImageSelector.MaxImages).ToArray();
            HandleExtraImages(images.Skip(ImageSelector.MaxImages), folder);
            var form = new ImageSelector(mainForm, formImages);
            form.mFolder = folder;
            form.Show();
        }

        public ImageSelector(GraphicsExtractor mainForm, SpriteSheet sheet)
            : this(mainForm)
        {
            var firstImage = new BitmapPortion(sheet.Image.Path.FullPath);          
            this.ResetLoadedImages(firstImage);

            foreach (var frame in sheet.Frames)
            {
                var rec = firstImage.Extract(frame.Source);
                AddImagePanel(rec);
                rec.Origin = new RGPointI(frame.Origin.X, frame.Origin.Y);
                rec.CopyHitboxesFrom(frame);
            }
        }



        private ImageSelector(GraphicsExtractor mainForm, BitmapPortion[] images)
            : this(mainForm)
        {            
           // this.ResetLoadedImages(firstImage);
            bgLoadNextImages.RunWorkerAsync(images);
        }
        #endregion

        #region Selection and Current

        public BitmapPortion CurrentImage
        {
            get { return mLoadedImagePanels[CurrentImageIndex].Image; }
            private set { CurrentImageIndex = mLoadedImagePanels.FindIndex(p => p.Image.Equals(value)); }
        }

        private int mCurrentImageIndex = 0;
        private int CurrentImageIndex
        {
            get
            {
                if (mCurrentImageIndex < 0)
                    return 0;
                if (mCurrentImageIndex >= mLoadedImagePanels.Count)
                    return mLoadedImagePanels.Count - 1;

                return mCurrentImageIndex;
            }
            set
            {
                mCurrentImageIndex = value;
                RefreshLoadedImages();

                var curPanel = mLoadedImagePanels[CurrentImageIndex];
                if (curPanel.Bottom > pnlLoadedImages.Height)
                    pnlLoadedImagesScroll.AddScroll(curPanel.Image.Height + 50);
                if (curPanel.Top < 0)
                    pnlLoadedImagesScroll.AddScroll(-(curPanel.Image.Height + 50));

                mMainForm.CurrentImage = this.CurrentImage;
                mMainForm.CurrentImageSelector = this;
            }
        }

        public IEnumerable<BitmapPortion> SelectedImages
        {
            get
            {                
                return mLoadedImagePanels.Where(p => p.Selected || p.Image == CurrentImage).OrderBy(p=>p.SelectionIndex).Select(p => p.Image);
            }
        }

        public void SelectPrevious()
        {
            var selection = this.SelectedImages.ToList();
            var index = selection.IndexOf(this.CurrentImage);
            index--;
            if (index < 0)
                index = selection.Count - 1;

            this.CurrentImage = selection[index];
        }

        public void SelectNext()
        {
            var selection = this.SelectedImages.ToList();
            var index = selection.IndexOf(this.CurrentImage);
            index++;
            if (index == selection.Count)
                index = 0;

            this.CurrentImage = selection[index];
        }


        #endregion

        #region Loaded Images

        private const int LoadedImagePanelSpacing = 8;

        private List<BitmapPortionPanelUserControl> mLoadedImagePanels = new List<BitmapPortionPanelUserControl>();

        private BitmapPortionPanelUserControl AddImagePanel(BitmapPortion image)
        {
            var lastPanel = mLoadedImagePanels.LastOrDefault();

            if (lastPanel == null)
            {
                mLoadedImagePanels.Add(pnlLoadedImage);
                pnlLoadedImage.Image = image;
                return pnlLoadedImage;
            }

            var panel = new BitmapPortionPanelUserControl(true);
            panel.DisplayPortionOnly = true;

            panel.SetBounds(lastPanel.Right + LoadedImagePanelSpacing, lastPanel.Top, lastPanel.Width, lastPanel.Height);
            if (panel.Right > pnlLoadedImages.Width)
                panel.SetBounds(LoadedImagePanelSpacing, lastPanel.Bottom + LoadedImagePanelSpacing, lastPanel.Width, lastPanel.Height);

            pnlLoadedImages.Controls.Add(panel);
            panel.BackColor = lastPanel.BackColor;
            panel.Image = image;
            panel.EnableSelector = true;

            mLoadedImagePanels.Add(panel);

            panel.RefreshImage();
            panel.ImagePanel.MouseAction += new Editor.ImagePanel.MouseActionEventHandler(this.pnlLoadedImage_ImageClicked);
            panel.ImageSelectionChanged += new BitmapPortionPanelUserControl.ImageSelectedEventHandler(pnlLoadedImage_ImageSelectionChanged);
        
            pnlLoadedImagesScroll.LargeChange = (panel.Height + LoadedImagePanelSpacing)* 2;
            pnlLoadedImagesScroll.SmallChange = panel.Height + LoadedImagePanelSpacing;


            pnlLoadedImagesScroll.Maximum = mLoadedImagePanels.Max(p=>p.Bottom)+ pnlLoadedImagesScroll.LargeChange;

            panel.Selected = false;

            return panel;
        }

        private void ResetLoadedImages(BitmapPortion firstImage)
        {
            var panels = mLoadedImagePanels.Skip(1).ToArray();

            foreach (var panel in panels)
                pnlLoadedImages.Controls.Remove(panel);

            mLoadedImagePanels = new List<BitmapPortionPanelUserControl>();
            mLoadedImagePanels.Add(pnlLoadedImage);

            mLoadedImagePanels.First().Image = firstImage;

            pnlLoadedImagesScroll.Maximum = 100;

            this.CurrentImageIndex = 0;
        }

        private void RefreshLoadedImages()
        {
            frmLog.AddLine("RefreshLoadedImages");

            if (mLoadedImagePanels.Count == 0)
                return;

            foreach (var panel in mLoadedImagePanels)
            {
                panel.ClearOverlay();
                if (panel.Image.Equals(this.CurrentImage))
                    panel.AddOverlayItem(new OverlayRectangle { Pen = new Pen(Color.Red, 2f), Area = panel.ImagePanel.EditorRectangleFromImageRec(RGRectangleI.FromXYWH(0, 0, panel.Image.Width, panel.Image.Height)) });
            }

            SetPanelScroll(-pnlLoadedImagesScroll.Value,true);

            pnlLoadedImages.Refresh();
        }

        private void pnlLoadedImages_Resize(object sender, EventArgs e)
        {
            RefreshLoadedImages();
        }

        private void pnlLoadedImage_ImageClicked(object sender, ImageEventArgs e)
        {
            if (e.Action == MouseActionType.Click && e.Buttons == System.Windows.Forms.MouseButtons.Left)
                this.CurrentImage = (sender as BitmapPortionPanel).Image;
        }

        private void pnlLoadedImagesScroll_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                frmLog.AddLine("Scroll: " + e.NewValue);
                SetPanelScroll(-e.NewValue,false);
            }
        }

        private void SetPanelScroll(int scrollValue, bool setMinMax)
        {
            int y = scrollValue;
            int x = LoadedImagePanelSpacing;
            int bottom = y;

            foreach (var pnl in mLoadedImagePanels)
            {
                if (x + pnl.Width > pnlLoadedImages.Width)
                {
                    x = LoadedImagePanelSpacing;
                    y += pnl.Height + LoadedImagePanelSpacing;
                }

                pnl.Left = x;
                pnl.Top = y;

                x += pnl.Width + LoadedImagePanelSpacing;

                bottom = Math.Max(bottom, pnl.Bottom + LoadedImagePanelSpacing);
            }

            bottom -= scrollValue;

            if (setMinMax)
            {
                if (pnlLoadedImagesScroll.Value >= bottom)
                    pnlLoadedImagesScroll.Value = Math.Max(0, bottom);

                pnlLoadedImagesScroll.Minimum = 0;
                pnlLoadedImagesScroll.Maximum = bottom;
            }
        }

        #endregion

        #region Toolbar

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            mInSelectionEvent = true;
            foreach (var panel in mLoadedImagePanels)
                panel.Selected = true;
            mInSelectionEvent = false;
        }

        private void btnInvertSelection_Click(object sender, EventArgs e)
        {
            mInSelectionEvent = true;
            foreach (var panel in mLoadedImagePanels)
                panel.Selected = !panel.Selected;

            mInSelectionEvent = false;
        }

        private void btnSelectBetween_Click(object sender, EventArgs e)
        {
            mInSelectionEvent = true;
            mLoadedImagePanels[mCurrentImageIndex].Selected = true;

            foreach (var panel in mLoadedImagePanels.Skip(mCurrentImageIndex + 1))
            {
                if (panel.Selected)
                {
                    mInSelectionEvent = false;
                    return;
                }

                panel.Selected = true;
            }

            mInSelectionEvent = false;
        }

        private void btnSelectNext_Click(object sender, EventArgs e)
        {
            mInSelectionEvent = true;
            foreach (var panel in mLoadedImagePanels.Skip(mCurrentImageIndex + 1))
                panel.Selected = true;

            mInSelectionEvent = false;
        }


        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            mInSelectionEvent = true;
            foreach (var panel in mLoadedImagePanels)
                panel.Selected = false;

            mInSelectionEvent = false;
        }


        #endregion

        #region Load Next Images
        private void bgLoadNextImages_DoWork(object sender, DoWorkEventArgs e)
        {
            bgLoadNextImages.ReportProgress(0);

            BitmapPortion[] images = e.Argument as BitmapPortion[];
            if (images == null)
                return;

            foreach(var image in images)
            {
                image.ResetRegion();
                bgLoadNextImages.ReportProgress(0, image);
            }
        }

        private void bgLoadNextImages_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pnlLoadingNotify.BackColor = Color.DarkRed;
            pnlSideTop.Refresh();

            var image = e.UserState as BitmapPortion;
            if (image == null)
                return;

            var panel = AddImagePanel(image);

            if (panel == null)
                bgLoadNextImages.CancelAsync();
        }

        private void bgLoadNextImages_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pnlLoadingNotify.BackColor = Color.DarkGreen;
            RefreshLoadedImages();
            CheckForConsecutiveDuplicates();
        }

        #endregion

        #region Remove Duplicates

        private void CheckForConsecutiveDuplicates()
        {
            if (!bgRemoveDuplicates.IsBusy)
                bgRemoveDuplicates.RunWorkerAsync(mLoadedImagePanels.ToArray());
        }

        private void bgRemoveDuplicates_DoWork(object sender, DoWorkEventArgs e)
        {
            bgRemoveDuplicates.ReportProgress(0);

            BitmapPortionPanelUserControl[] panels = e.Argument as BitmapPortionPanelUserControl[];

            List<BitmapPortionPanelUserControl> removedPanels = new List<BitmapPortionPanelUserControl>();

            var lastPanel = panels.FirstOrDefault();
            if (lastPanel == null)
                return;

            foreach (var panel in panels.Skip(1))
            {
                if (panel.Image.Region.Width == panel.Image.Width && panel.Image.Region.Height == panel.Image.Height)
                {
                    lastPanel = null;
                    continue;
                }

                if (lastPanel != null && panel.Image.PixelsEqual(lastPanel.Image))
                    removedPanels.Add(panel);
                else
                    lastPanel = panel;
            }

            e.Result = removedPanels;

        }

        private void bgRemoveDuplicates_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pnlDupNotify.BackColor = Color.DarkGreen;
            if (e.Cancelled)
                return;

            var removedPanels = e.Result as List<BitmapPortionPanelUserControl>;
            if (removedPanels == null || removedPanels.Count == 0)
                return;

            if (removedPanels.Count == mLoadedImagePanels.Count)
                return;

            foreach (var removedPanel in removedPanels)
            {
                removedPanel.Visible = false;
                pnlLoadedImage.Controls.Remove(removedPanel);
            }

            mLoadedImagePanels.RemoveAll(p => !p.Visible);
            RefreshLoadedImages();
        }

        private void bgRemoveDuplicates_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pnlDupNotify.BackColor = Color.DarkRed;
            pnlSideTop.Refresh();
        }

        #endregion 

        private void saveSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fc = new FolderBrowserDialog();
            fc.SelectedPath = Paths.ScreenshotsPath;

            if (!String.IsNullOrEmpty(mFolder))
                fc.SelectedPath = mFolder;

            if (fc.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            if (MessageBox.Show("Replace entire folder with " + SelectedImages.Count() + " image(s)?", "Replace entire folder?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                var files = System.IO.Directory.GetFiles(fc.SelectedPath);
                foreach (var file in files)
                    System.IO.File.Delete(file);
            }

            int fileNum = 0;
            foreach (var panel in SelectedImages)
                panel.SavePortion(System.IO.Path.Combine(fc.SelectedPath, (++fileNum) + ".png"));
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mMainForm.ClipboardImages = this.SelectedImages.ToArray();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var img in mMainForm.ClipboardImages)
                AddImagePanel(img);

            RefreshLoadedImages();
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (MessageBox.Show("Delete files?", "Delete files?", MessageBoxButtons.YesNoCancel))
            {
                case System.Windows.Forms.DialogResult.Yes:
                    DeleteSelected(true);
                    break;

                case System.Windows.Forms.DialogResult.No:
                    DeleteSelected(false);
                    break;
            }
        }

        private void DeleteSelected(bool deleteFiles)
        {
            var removedPanels = mLoadedImagePanels.Where(p => p.Selected).ToArray();
            if (removedPanels == null || removedPanels.Length == 0)
                return;

            foreach (var removedPanel in removedPanels)
            {
                removedPanel.Visible = false;
                pnlLoadedImage.Controls.Remove(removedPanel);
            }

            mLoadedImagePanels.RemoveAll(p => !p.Visible);
            RefreshLoadedImages();

            if (deleteFiles)
            {
                foreach (var panel in removedPanels)
                {
                    panel.Image.DeleteFile();
                }
            }
        }

        private void ImageSelector_Resize(object sender, EventArgs e)
        {
            RefreshLoadedImages();
        }

        private void btnDeleteDuplicates_Click(object sender, EventArgs e)
        {
            var uniqueItems = this.SelectedImages.Distinct(new PixelEqualityComparer()).ToArray();

            var removedPanels = this.mLoadedImagePanels.Where(p => p.Selected && !uniqueItems.Contains(p.Image)).ToArray();
            if (removedPanels == null || removedPanels.Length == 0)
            {
                RefreshLoadedImages();
                return;
            }

            foreach (var removedPanel in removedPanels)
            {
                removedPanel.Visible = false;
                pnlLoadedImage.Controls.Remove(removedPanel);
            }

            mLoadedImagePanels.RemoveAll(p => !p.Visible);
            RefreshLoadedImages();
        }

        private void btnGroupSelected_Click(object sender, EventArgs e)
        {
            var newOrdering = new List<BitmapPortion>();

            var oldOrdering = mLoadedImagePanels.Select(p => p.Image).ToArray();

            newOrdering.AddRange(mLoadedImagePanels.TakeWhile(p => !p.Selected).Select(p => p.Image));
            newOrdering.AddRange(this.SelectedImages);
            newOrdering.AddRange(mLoadedImagePanels.SkipWhile(p => !p.Selected).Where(p => !p.Selected).Select(p => p.Image));

            if (newOrdering.Count == 0)
                return;

            this.ResetLoadedImages(newOrdering.First());

            foreach (var rec in newOrdering.Skip(1))            
                AddImagePanel(rec);

            RefreshLoadedImages();
        }

        private void chkKeepSelection_CheckedChanged(object sender, EventArgs e)
        {

        }


        #region Extra Images

        private static void HandleExtraImages(IEnumerable<BitmapPortion> images, string rootFolder)
        {
            if(images.IsNullOrEmpty())
                return;

            int count = 0;
            var folder = GetNextFreeImageFolder(rootFolder);

            foreach(var image in images)
            {
                image.ResetRegion();
                image.MoveTo(folder);

                if (++count >= ImageSelector.MaxImages)
                {
                    count = 0;
                    folder = GetNextFreeImageFolder(rootFolder);
                }
            }
            
        }

        private static string GetNextFreeImageFolder(string rootFolder)
        {
            int i = 1;
            while (System.IO.Directory.Exists(System.IO.Path.Combine(rootFolder, "Extra" + ++i)));
            return System.IO.Directory.CreateDirectory(System.IO.Path.Combine(rootFolder, "Extra" + i)).FullName;
        }

        #endregion 
    }
}
