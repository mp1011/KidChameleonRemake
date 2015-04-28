namespace Editor.Forms
{
    partial class ImageSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlLoadedImages = new System.Windows.Forms.Panel();
            this.pnlLoadedImage = new Editor.BitmapPortionPanelUserControl();
            this.pnlLoadedImagesScroll = new System.Windows.Forms.VScrollBar();
            this.pnlSideTop = new System.Windows.Forms.Panel();
            this.chkKeepSelection = new System.Windows.Forms.CheckBox();
            this.btnGroupSelected = new System.Windows.Forms.Button();
            this.btnDeleteDuplicates = new System.Windows.Forms.Button();
            this.btnClearSelection = new System.Windows.Forms.Button();
            this.btnSelectNext = new System.Windows.Forms.Button();
            this.btnSelectBetween = new System.Windows.Forms.Button();
            this.btnInvertSelection = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.pnlDupNotify = new System.Windows.Forms.Panel();
            this.pnlLoadingNotify = new System.Windows.Forms.Panel();
            this.bgLoadNextImages = new System.ComponentModel.BackgroundWorker();
            this.bgRemoveDuplicates = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClone = new System.Windows.Forms.Button();
            this.pnlLoadedImages.SuspendLayout();
            this.pnlSideTop.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLoadedImages
            // 
            this.pnlLoadedImages.Controls.Add(this.pnlLoadedImage);
            this.pnlLoadedImages.Controls.Add(this.pnlLoadedImagesScroll);
            this.pnlLoadedImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLoadedImages.Location = new System.Drawing.Point(0, 89);
            this.pnlLoadedImages.Name = "pnlLoadedImages";
            this.pnlLoadedImages.Size = new System.Drawing.Size(700, 478);
            this.pnlLoadedImages.TabIndex = 7;
            // 
            // pnlLoadedImage
            // 
            this.pnlLoadedImage.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.pnlLoadedImage.CenterOnOrigin = false;
            this.pnlLoadedImage.DisplayOrigin = null;
            this.pnlLoadedImage.DisplayPortionOnly = true;
            this.pnlLoadedImage.DrawRectangle = Editor.DrawRectangleType.None;
            this.pnlLoadedImage.EnableSelector = true;
            this.pnlLoadedImage.Image = null;
            this.pnlLoadedImage.Location = new System.Drawing.Point(12, 12);
            this.pnlLoadedImage.Name = "pnlLoadedImage";
            this.pnlLoadedImage.Selected = false;
            this.pnlLoadedImage.Size = new System.Drawing.Size(238, 205);
            this.pnlLoadedImage.TabIndex = 0;
            // 
            // pnlLoadedImagesScroll
            // 
            this.pnlLoadedImagesScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlLoadedImagesScroll.Location = new System.Drawing.Point(678, 0);
            this.pnlLoadedImagesScroll.Name = "pnlLoadedImagesScroll";
            this.pnlLoadedImagesScroll.Size = new System.Drawing.Size(22, 478);
            this.pnlLoadedImagesScroll.TabIndex = 1;
            this.pnlLoadedImagesScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.pnlLoadedImagesScroll_Scroll);
            // 
            // pnlSideTop
            // 
            this.pnlSideTop.Controls.Add(this.btnClone);
            this.pnlSideTop.Controls.Add(this.chkKeepSelection);
            this.pnlSideTop.Controls.Add(this.btnGroupSelected);
            this.pnlSideTop.Controls.Add(this.btnDeleteDuplicates);
            this.pnlSideTop.Controls.Add(this.btnClearSelection);
            this.pnlSideTop.Controls.Add(this.btnSelectNext);
            this.pnlSideTop.Controls.Add(this.btnSelectBetween);
            this.pnlSideTop.Controls.Add(this.btnInvertSelection);
            this.pnlSideTop.Controls.Add(this.btnSelectAll);
            this.pnlSideTop.Controls.Add(this.pnlDupNotify);
            this.pnlSideTop.Controls.Add(this.pnlLoadingNotify);
            this.pnlSideTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSideTop.Location = new System.Drawing.Point(0, 24);
            this.pnlSideTop.Name = "pnlSideTop";
            this.pnlSideTop.Size = new System.Drawing.Size(700, 65);
            this.pnlSideTop.TabIndex = 6;
            // 
            // chkKeepSelection
            // 
            this.chkKeepSelection.AutoSize = true;
            this.chkKeepSelection.Location = new System.Drawing.Point(286, 14);
            this.chkKeepSelection.Name = "chkKeepSelection";
            this.chkKeepSelection.Size = new System.Drawing.Size(98, 17);
            this.chkKeepSelection.TabIndex = 9;
            this.chkKeepSelection.Text = "Keep Selection";
            this.chkKeepSelection.UseVisualStyleBackColor = true;
            // 
            // btnGroupSelected
            // 
            this.btnGroupSelected.Location = new System.Drawing.Point(393, 12);
            this.btnGroupSelected.Name = "btnGroupSelected";
            this.btnGroupSelected.Size = new System.Drawing.Size(159, 22);
            this.btnGroupSelected.TabIndex = 8;
            this.btnGroupSelected.Text = "Group Selected Together";
            this.btnGroupSelected.UseVisualStyleBackColor = true;
            this.btnGroupSelected.Click += new System.EventHandler(this.btnGroupSelected_Click);
            // 
            // btnDeleteDuplicates
            // 
            this.btnDeleteDuplicates.Location = new System.Drawing.Point(558, 12);
            this.btnDeleteDuplicates.Name = "btnDeleteDuplicates";
            this.btnDeleteDuplicates.Size = new System.Drawing.Size(130, 22);
            this.btnDeleteDuplicates.TabIndex = 7;
            this.btnDeleteDuplicates.Text = "Delete All Duplicates";
            this.btnDeleteDuplicates.UseVisualStyleBackColor = true;
            this.btnDeleteDuplicates.Click += new System.EventHandler(this.btnDeleteDuplicates_Click);
            // 
            // btnClearSelection
            // 
            this.btnClearSelection.Location = new System.Drawing.Point(106, 10);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(46, 22);
            this.btnClearSelection.TabIndex = 6;
            this.btnClearSelection.Text = "None";
            this.btnClearSelection.UseVisualStyleBackColor = true;
            this.btnClearSelection.Click += new System.EventHandler(this.btnClearSelection_Click);
            // 
            // btnSelectNext
            // 
            this.btnSelectNext.Location = new System.Drawing.Point(238, 10);
            this.btnSelectNext.Name = "btnSelectNext";
            this.btnSelectNext.Size = new System.Drawing.Size(42, 22);
            this.btnSelectNext.TabIndex = 5;
            this.btnSelectNext.Text = "Next";
            this.btnSelectNext.UseVisualStyleBackColor = true;
            this.btnSelectNext.Click += new System.EventHandler(this.btnSelectNext_Click);
            // 
            // btnSelectBetween
            // 
            this.btnSelectBetween.Location = new System.Drawing.Point(198, 10);
            this.btnSelectBetween.Name = "btnSelectBetween";
            this.btnSelectBetween.Size = new System.Drawing.Size(34, 22);
            this.btnSelectBetween.TabIndex = 4;
            this.btnSelectBetween.Text = "Btw";
            this.btnSelectBetween.UseVisualStyleBackColor = true;
            this.btnSelectBetween.Click += new System.EventHandler(this.btnSelectBetween_Click);
            // 
            // btnInvertSelection
            // 
            this.btnInvertSelection.Location = new System.Drawing.Point(158, 10);
            this.btnInvertSelection.Name = "btnInvertSelection";
            this.btnInvertSelection.Size = new System.Drawing.Size(34, 22);
            this.btnInvertSelection.TabIndex = 3;
            this.btnInvertSelection.Text = "Inv";
            this.btnInvertSelection.UseVisualStyleBackColor = true;
            this.btnInvertSelection.Click += new System.EventHandler(this.btnInvertSelection_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(66, 10);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(34, 22);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // pnlDupNotify
            // 
            this.pnlDupNotify.BackColor = System.Drawing.Color.DarkRed;
            this.pnlDupNotify.Location = new System.Drawing.Point(39, 12);
            this.pnlDupNotify.Name = "pnlDupNotify";
            this.pnlDupNotify.Size = new System.Drawing.Size(21, 20);
            this.pnlDupNotify.TabIndex = 1;
            // 
            // pnlLoadingNotify
            // 
            this.pnlLoadingNotify.BackColor = System.Drawing.Color.DarkRed;
            this.pnlLoadingNotify.Location = new System.Drawing.Point(12, 12);
            this.pnlLoadingNotify.Name = "pnlLoadingNotify";
            this.pnlLoadingNotify.Size = new System.Drawing.Size(21, 20);
            this.pnlLoadingNotify.TabIndex = 0;
            // 
            // bgLoadNextImages
            // 
            this.bgLoadNextImages.WorkerReportsProgress = true;
            this.bgLoadNextImages.WorkerSupportsCancellation = true;
            this.bgLoadNextImages.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgLoadNextImages_DoWork);
            this.bgLoadNextImages.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgLoadNextImages_ProgressChanged);
            this.bgLoadNextImages.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgLoadNextImages_RunWorkerCompleted);
            // 
            // bgRemoveDuplicates
            // 
            this.bgRemoveDuplicates.WorkerReportsProgress = true;
            this.bgRemoveDuplicates.WorkerSupportsCancellation = true;
            this.bgRemoveDuplicates.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgRemoveDuplicates_DoWork);
            this.bgRemoveDuplicates.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgRemoveDuplicates_ProgressChanged);
            this.bgRemoveDuplicates.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgRemoveDuplicates_RunWorkerCompleted);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(700, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveSelectedToolStripMenuItem,
            this.toolStripMenuItem2,
            this.deleteSelectedToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(151, 6);
            // 
            // saveSelectedToolStripMenuItem
            // 
            this.saveSelectedToolStripMenuItem.Name = "saveSelectedToolStripMenuItem";
            this.saveSelectedToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.saveSelectedToolStripMenuItem.Text = "Save Selected";
            this.saveSelectedToolStripMenuItem.Click += new System.EventHandler(this.saveSelectedToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(151, 6);
            // 
            // deleteSelectedToolStripMenuItem
            // 
            this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.deleteSelectedToolStripMenuItem.Text = "Delete Selected";
            this.deleteSelectedToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedToolStripMenuItem_Click);
            // 
            // btnClone
            // 
            this.btnClone.Location = new System.Drawing.Point(393, 37);
            this.btnClone.Name = "btnClone";
            this.btnClone.Size = new System.Drawing.Size(76, 22);
            this.btnClone.TabIndex = 10;
            this.btnClone.Text = "Clone";
            this.btnClone.UseVisualStyleBackColor = true;
            this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
            // 
            // ImageSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 567);
            this.Controls.Add(this.pnlLoadedImages);
            this.Controls.Add(this.pnlSideTop);
            this.Controls.Add(this.menuStrip1);
            this.Name = "ImageSelector";
            this.Text = "ImageSelector";
            this.Resize += new System.EventHandler(this.ImageSelector_Resize);
            this.pnlLoadedImages.ResumeLayout(false);
            this.pnlSideTop.ResumeLayout(false);
            this.pnlSideTop.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlLoadedImages;
        private BitmapPortionPanelUserControl pnlLoadedImage;
        private System.Windows.Forms.VScrollBar pnlLoadedImagesScroll;
        private System.Windows.Forms.Panel pnlSideTop;
        private System.Windows.Forms.Button btnSelectBetween;
        private System.Windows.Forms.Button btnInvertSelection;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Panel pnlDupNotify;
        private System.Windows.Forms.Panel pnlLoadingNotify;
        private System.ComponentModel.BackgroundWorker bgLoadNextImages;
        private System.ComponentModel.BackgroundWorker bgRemoveDuplicates;
        private System.Windows.Forms.Button btnClearSelection;
        private System.Windows.Forms.Button btnSelectNext;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedToolStripMenuItem;
        private System.Windows.Forms.Button btnDeleteDuplicates;
        private System.Windows.Forms.Button btnGroupSelected;
        private System.Windows.Forms.CheckBox chkKeepSelection;
        private System.Windows.Forms.Button btnClone;
    }
}