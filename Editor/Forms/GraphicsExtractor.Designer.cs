namespace Editor.Forms
{
    partial class GraphicsExtractor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.newSpriteSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openTilesetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTilesetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTools = new System.Windows.Forms.Panel();
            this.chkCreateFlashTexture = new System.Windows.Forms.CheckBox();
            this.chkCenterOrigin = new System.Windows.Forms.CheckBox();
            this.btnCrop = new System.Windows.Forms.Button();
            this.btnMoveToSide = new System.Windows.Forms.Button();
            this.btnCropToSeam = new System.Windows.Forms.Button();
            this.btnRectangleMoveSide2 = new System.Windows.Forms.Button();
            this.btnRectangleMoveSide = new System.Windows.Forms.Button();
            this.txtMoveRecAmount = new System.Windows.Forms.NumericUpDown();
            this.btnPreviewAnimation = new System.Windows.Forms.Button();
            this.chkShowRegion = new System.Windows.Forms.CheckBox();
            this.dirNextPixel = new Editor.DirectionSelector();
            this.pnlClickAction = new System.Windows.Forms.Panel();
            this.rdoHitbox2 = new System.Windows.Forms.RadioButton();
            this.txtFloodFillRes = new System.Windows.Forms.NumericUpDown();
            this.rdoHitbox = new System.Windows.Forms.RadioButton();
            this.rdoSetOrigin = new System.Windows.Forms.RadioButton();
            this.chkContinueAction = new System.Windows.Forms.CheckBox();
            this.rdoCrop = new System.Windows.Forms.RadioButton();
            this.rdoNoAction = new System.Windows.Forms.RadioButton();
            this.rdoPickSprite = new System.Windows.Forms.RadioButton();
            this.rdoPickTile = new System.Windows.Forms.RadioButton();
            this.txtRectangleIncrease = new System.Windows.Forms.NumericUpDown();
            this.pnlTransparentColor = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlImage = new Editor.BitmapPortionPanelUserControl();
            this.chkCropToNew = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.pnlTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMoveRecAmount)).BeginInit();
            this.pnlClickAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFloodFillRes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRectangleIncrease)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(819, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.newSpriteSheetToolStripMenuItem,
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openTilesetToolStripMenuItem,
            this.saveTilesetToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openToolStripMenuItem.Text = "Load Screenshots";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // newSpriteSheetToolStripMenuItem
            // 
            this.newSpriteSheetToolStripMenuItem.Name = "newSpriteSheetToolStripMenuItem";
            this.newSpriteSheetToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.newSpriteSheetToolStripMenuItem.Text = "New Sprite Sheet";
            this.newSpriteSheetToolStripMenuItem.Click += new System.EventHandler(this.newSpriteSheetToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
            this.openToolStripMenuItem1.Text = "Open Sprite Sheet";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveToolStripMenuItem.Text = "Save Sprite Sheet";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 6);
            // 
            // openTilesetToolStripMenuItem
            // 
            this.openTilesetToolStripMenuItem.Name = "openTilesetToolStripMenuItem";
            this.openTilesetToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openTilesetToolStripMenuItem.Text = "Open Tileset";
            this.openTilesetToolStripMenuItem.Click += new System.EventHandler(this.openTilesetToolStripMenuItem_Click);
            // 
            // saveTilesetToolStripMenuItem
            // 
            this.saveTilesetToolStripMenuItem.Name = "saveTilesetToolStripMenuItem";
            this.saveTilesetToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveTilesetToolStripMenuItem.Text = "Save Tileset";
            this.saveTilesetToolStripMenuItem.Click += new System.EventHandler(this.saveTilesetToolStripMenuItem_Click);
            // 
            // pnlTools
            // 
            this.pnlTools.Controls.Add(this.chkCreateFlashTexture);
            this.pnlTools.Controls.Add(this.chkCenterOrigin);
            this.pnlTools.Controls.Add(this.btnCrop);
            this.pnlTools.Controls.Add(this.btnMoveToSide);
            this.pnlTools.Controls.Add(this.btnCropToSeam);
            this.pnlTools.Controls.Add(this.btnRectangleMoveSide2);
            this.pnlTools.Controls.Add(this.btnRectangleMoveSide);
            this.pnlTools.Controls.Add(this.txtMoveRecAmount);
            this.pnlTools.Controls.Add(this.btnPreviewAnimation);
            this.pnlTools.Controls.Add(this.chkShowRegion);
            this.pnlTools.Controls.Add(this.dirNextPixel);
            this.pnlTools.Controls.Add(this.pnlClickAction);
            this.pnlTools.Controls.Add(this.pnlTransparentColor);
            this.pnlTools.Controls.Add(this.label1);
            this.pnlTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTools.Location = new System.Drawing.Point(0, 24);
            this.pnlTools.Name = "pnlTools";
            this.pnlTools.Size = new System.Drawing.Size(819, 125);
            this.pnlTools.TabIndex = 3;
            // 
            // chkCreateFlashTexture
            // 
            this.chkCreateFlashTexture.AutoSize = true;
            this.chkCreateFlashTexture.Location = new System.Drawing.Point(154, 102);
            this.chkCreateFlashTexture.Name = "chkCreateFlashTexture";
            this.chkCreateFlashTexture.Size = new System.Drawing.Size(124, 17);
            this.chkCreateFlashTexture.TabIndex = 16;
            this.chkCreateFlashTexture.Text = "Create Flash Texture";
            this.chkCreateFlashTexture.UseVisualStyleBackColor = true;
            // 
            // chkCenterOrigin
            // 
            this.chkCenterOrigin.AutoSize = true;
            this.chkCenterOrigin.Location = new System.Drawing.Point(9, 102);
            this.chkCenterOrigin.Name = "chkCenterOrigin";
            this.chkCenterOrigin.Size = new System.Drawing.Size(102, 17);
            this.chkCenterOrigin.TabIndex = 15;
            this.chkCenterOrigin.Text = "Center on Origin";
            this.chkCenterOrigin.UseVisualStyleBackColor = true;
            this.chkCenterOrigin.CheckedChanged += new System.EventHandler(this.chkCenterOrigin_CheckedChanged);
            // 
            // btnCrop
            // 
            this.btnCrop.Location = new System.Drawing.Point(400, 35);
            this.btnCrop.Name = "btnCrop";
            this.btnCrop.Size = new System.Drawing.Size(83, 25);
            this.btnCrop.TabIndex = 14;
            this.btnCrop.Text = "Crop";
            this.btnCrop.UseVisualStyleBackColor = true;
            this.btnCrop.Click += new System.EventHandler(this.btnCrop_Click);
            // 
            // btnMoveToSide
            // 
            this.btnMoveToSide.Location = new System.Drawing.Point(296, 66);
            this.btnMoveToSide.Name = "btnMoveToSide";
            this.btnMoveToSide.Size = new System.Drawing.Size(98, 25);
            this.btnMoveToSide.TabIndex = 13;
            this.btnMoveToSide.Text = "Move To Side";
            this.btnMoveToSide.UseVisualStyleBackColor = true;
            this.btnMoveToSide.Click += new System.EventHandler(this.btnMoveToSide_Click);
            // 
            // btnCropToSeam
            // 
            this.btnCropToSeam.Location = new System.Drawing.Point(400, 66);
            this.btnCropToSeam.Name = "btnCropToSeam";
            this.btnCropToSeam.Size = new System.Drawing.Size(83, 25);
            this.btnCropToSeam.TabIndex = 12;
            this.btnCropToSeam.Text = "Crop to Seam";
            this.btnCropToSeam.UseVisualStyleBackColor = true;
            this.btnCropToSeam.Click += new System.EventHandler(this.btnCropToSeam_Click);
            // 
            // btnRectangleMoveSide2
            // 
            this.btnRectangleMoveSide2.Location = new System.Drawing.Point(296, 35);
            this.btnRectangleMoveSide2.Name = "btnRectangleMoveSide2";
            this.btnRectangleMoveSide2.Size = new System.Drawing.Size(98, 25);
            this.btnRectangleMoveSide2.TabIndex = 11;
            this.btnRectangleMoveSide2.Text = "Move Side(s) -";
            this.btnRectangleMoveSide2.UseVisualStyleBackColor = true;
            this.btnRectangleMoveSide2.Click += new System.EventHandler(this.btnRectangleMoveSide2_Click);
            // 
            // btnRectangleMoveSide
            // 
            this.btnRectangleMoveSide.Location = new System.Drawing.Point(296, 7);
            this.btnRectangleMoveSide.Name = "btnRectangleMoveSide";
            this.btnRectangleMoveSide.Size = new System.Drawing.Size(98, 25);
            this.btnRectangleMoveSide.TabIndex = 10;
            this.btnRectangleMoveSide.Text = "Move Side(s) +";
            this.btnRectangleMoveSide.UseVisualStyleBackColor = true;
            this.btnRectangleMoveSide.Click += new System.EventHandler(this.btnRectangleMoveSide_Click);
            // 
            // txtMoveRecAmount
            // 
            this.txtMoveRecAmount.Location = new System.Drawing.Point(400, 9);
            this.txtMoveRecAmount.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.txtMoveRecAmount.Minimum = new decimal(new int[] {
            64,
            0,
            0,
            -2147483648});
            this.txtMoveRecAmount.Name = "txtMoveRecAmount";
            this.txtMoveRecAmount.Size = new System.Drawing.Size(40, 20);
            this.txtMoveRecAmount.TabIndex = 9;
            this.txtMoveRecAmount.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // btnPreviewAnimation
            // 
            this.btnPreviewAnimation.Location = new System.Drawing.Point(9, 76);
            this.btnPreviewAnimation.Name = "btnPreviewAnimation";
            this.btnPreviewAnimation.Size = new System.Drawing.Size(65, 22);
            this.btnPreviewAnimation.TabIndex = 8;
            this.btnPreviewAnimation.Text = "Animate";
            this.btnPreviewAnimation.UseVisualStyleBackColor = true;
            this.btnPreviewAnimation.Click += new System.EventHandler(this.btnPreviewAnimation_Click);
            // 
            // chkShowRegion
            // 
            this.chkShowRegion.AutoSize = true;
            this.chkShowRegion.Checked = true;
            this.chkShowRegion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowRegion.Location = new System.Drawing.Point(9, 60);
            this.chkShowRegion.Name = "chkShowRegion";
            this.chkShowRegion.Size = new System.Drawing.Size(115, 17);
            this.chkShowRegion.TabIndex = 7;
            this.chkShowRegion.Text = "Show Entire Image";
            this.chkShowRegion.UseVisualStyleBackColor = true;
            this.chkShowRegion.CheckedChanged += new System.EventHandler(this.chkShowRegion_CheckedChanged);
            // 
            // dirNextPixel
            // 
            this.dirNextPixel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dirNextPixel.Location = new System.Drawing.Point(154, 3);
            this.dirNextPixel.Name = "dirNextPixel";
            this.dirNextPixel.Size = new System.Drawing.Size(136, 95);
            this.dirNextPixel.TabIndex = 6;
            // 
            // pnlClickAction
            // 
            this.pnlClickAction.Controls.Add(this.chkCropToNew);
            this.pnlClickAction.Controls.Add(this.rdoHitbox2);
            this.pnlClickAction.Controls.Add(this.txtFloodFillRes);
            this.pnlClickAction.Controls.Add(this.rdoHitbox);
            this.pnlClickAction.Controls.Add(this.rdoSetOrigin);
            this.pnlClickAction.Controls.Add(this.chkContinueAction);
            this.pnlClickAction.Controls.Add(this.rdoCrop);
            this.pnlClickAction.Controls.Add(this.rdoNoAction);
            this.pnlClickAction.Controls.Add(this.rdoPickSprite);
            this.pnlClickAction.Controls.Add(this.rdoPickTile);
            this.pnlClickAction.Controls.Add(this.txtRectangleIncrease);
            this.pnlClickAction.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlClickAction.Location = new System.Drawing.Point(492, 0);
            this.pnlClickAction.Name = "pnlClickAction";
            this.pnlClickAction.Size = new System.Drawing.Size(327, 125);
            this.pnlClickAction.TabIndex = 5;
            // 
            // rdoHitbox2
            // 
            this.rdoHitbox2.AutoSize = true;
            this.rdoHitbox2.Location = new System.Drawing.Point(12, 76);
            this.rdoHitbox2.Name = "rdoHitbox2";
            this.rdoHitbox2.Size = new System.Drawing.Size(128, 17);
            this.rdoHitbox2.TabIndex = 11;
            this.rdoHitbox2.Text = "Set Secondary Hitbox";
            this.rdoHitbox2.UseVisualStyleBackColor = true;
            this.rdoHitbox2.CheckedChanged += new System.EventHandler(this.rdoHitbox2_CheckedChanged);
            // 
            // txtFloodFillRes
            // 
            this.txtFloodFillRes.Location = new System.Drawing.Point(84, 32);
            this.txtFloodFillRes.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.txtFloodFillRes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtFloodFillRes.Name = "txtFloodFillRes";
            this.txtFloodFillRes.Size = new System.Drawing.Size(40, 20);
            this.txtFloodFillRes.TabIndex = 10;
            this.txtFloodFillRes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // rdoHitbox
            // 
            this.rdoHitbox.AutoSize = true;
            this.rdoHitbox.Location = new System.Drawing.Point(12, 55);
            this.rdoHitbox.Name = "rdoHitbox";
            this.rdoHitbox.Size = new System.Drawing.Size(74, 17);
            this.rdoHitbox.TabIndex = 9;
            this.rdoHitbox.Text = "Set Hitbox";
            this.rdoHitbox.UseVisualStyleBackColor = true;
            this.rdoHitbox.CheckedChanged += new System.EventHandler(this.rdoHitbox_CheckedChanged);
            // 
            // rdoSetOrigin
            // 
            this.rdoSetOrigin.AutoSize = true;
            this.rdoSetOrigin.Location = new System.Drawing.Point(165, 9);
            this.rdoSetOrigin.Name = "rdoSetOrigin";
            this.rdoSetOrigin.Size = new System.Drawing.Size(71, 17);
            this.rdoSetOrigin.TabIndex = 8;
            this.rdoSetOrigin.Text = "Set Origin";
            this.rdoSetOrigin.UseVisualStyleBackColor = true;
            this.rdoSetOrigin.CheckedChanged += new System.EventHandler(this.rdoSetOrigin_CheckedChanged);
            // 
            // chkContinueAction
            // 
            this.chkContinueAction.AutoSize = true;
            this.chkContinueAction.Checked = true;
            this.chkContinueAction.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkContinueAction.Location = new System.Drawing.Point(12, 102);
            this.chkContinueAction.Name = "chkContinueAction";
            this.chkContinueAction.Size = new System.Drawing.Size(167, 17);
            this.chkContinueAction.TabIndex = 6;
            this.chkContinueAction.Text = "Repeat Action for all Selected";
            this.chkContinueAction.UseVisualStyleBackColor = true;
            // 
            // rdoCrop
            // 
            this.rdoCrop.AutoSize = true;
            this.rdoCrop.Location = new System.Drawing.Point(165, 55);
            this.rdoCrop.Name = "rdoCrop";
            this.rdoCrop.Size = new System.Drawing.Size(47, 17);
            this.rdoCrop.TabIndex = 7;
            this.rdoCrop.Text = "Crop";
            this.rdoCrop.UseVisualStyleBackColor = true;
            this.rdoCrop.CheckedChanged += new System.EventHandler(this.rdoCrop_CheckedChanged);
            // 
            // rdoNoAction
            // 
            this.rdoNoAction.AutoSize = true;
            this.rdoNoAction.Checked = true;
            this.rdoNoAction.Location = new System.Drawing.Point(165, 32);
            this.rdoNoAction.Name = "rdoNoAction";
            this.rdoNoAction.Size = new System.Drawing.Size(72, 17);
            this.rdoNoAction.TabIndex = 6;
            this.rdoNoAction.TabStop = true;
            this.rdoNoAction.Text = "No Action";
            this.rdoNoAction.UseVisualStyleBackColor = true;
            // 
            // rdoPickSprite
            // 
            this.rdoPickSprite.AutoSize = true;
            this.rdoPickSprite.Location = new System.Drawing.Point(12, 32);
            this.rdoPickSprite.Name = "rdoPickSprite";
            this.rdoPickSprite.Size = new System.Drawing.Size(76, 17);
            this.rdoPickSprite.TabIndex = 5;
            this.rdoPickSprite.Text = "Pick Sprite";
            this.rdoPickSprite.UseVisualStyleBackColor = true;
            // 
            // rdoPickTile
            // 
            this.rdoPickTile.AutoSize = true;
            this.rdoPickTile.Location = new System.Drawing.Point(12, 9);
            this.rdoPickTile.Name = "rdoPickTile";
            this.rdoPickTile.Size = new System.Drawing.Size(66, 17);
            this.rdoPickTile.TabIndex = 0;
            this.rdoPickTile.Text = "Pick Tile";
            this.rdoPickTile.UseVisualStyleBackColor = true;
            // 
            // txtRectangleIncrease
            // 
            this.txtRectangleIncrease.Location = new System.Drawing.Point(84, 7);
            this.txtRectangleIncrease.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.txtRectangleIncrease.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtRectangleIncrease.Name = "txtRectangleIncrease";
            this.txtRectangleIncrease.Size = new System.Drawing.Size(40, 20);
            this.txtRectangleIncrease.TabIndex = 4;
            this.txtRectangleIncrease.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // pnlTransparentColor
            // 
            this.pnlTransparentColor.BackColor = System.Drawing.Color.White;
            this.pnlTransparentColor.Location = new System.Drawing.Point(9, 34);
            this.pnlTransparentColor.Name = "pnlTransparentColor";
            this.pnlTransparentColor.Size = new System.Drawing.Size(50, 20);
            this.pnlTransparentColor.TabIndex = 1;
            this.pnlTransparentColor.Click += new System.EventHandler(this.pnlTransparentColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Transparent Color";
            // 
            // pnlImage
            // 
            this.pnlImage.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.pnlImage.CenterOnOrigin = false;
            this.pnlImage.DisplayOrigin = null;
            this.pnlImage.DisplayPortionOnly = false;
            this.pnlImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImage.DrawRectangle = Editor.DrawRectangleType.None;
            this.pnlImage.EnableSelector = false;
            this.pnlImage.Image = null;
            this.pnlImage.Location = new System.Drawing.Point(0, 149);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Selected = false;
            this.pnlImage.Size = new System.Drawing.Size(819, 367);
            this.pnlImage.TabIndex = 4;
            // 
            // chkCropToNew
            // 
            this.chkCropToNew.AutoSize = true;
            this.chkCropToNew.Location = new System.Drawing.Point(219, 55);
            this.chkCropToNew.Name = "chkCropToNew";
            this.chkCropToNew.Size = new System.Drawing.Size(96, 17);
            this.chkCropToNew.TabIndex = 12;
            this.chkCropToNew.Text = "To New Image";
            this.chkCropToNew.UseVisualStyleBackColor = true;
            // 
            // GraphicsExtractor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 516);
            this.Controls.Add(this.pnlImage);
            this.Controls.Add(this.pnlTools);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GraphicsExtractor";
            this.Text = "SpriteExtractor";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GraphicsExtractor_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlTools.ResumeLayout(false);
            this.pnlTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMoveRecAmount)).EndInit();
            this.pnlClickAction.ResumeLayout(false);
            this.pnlClickAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFloodFillRes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRectangleIncrease)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Panel pnlTools;
        private BitmapPortionPanelUserControl pnlImage;
        private System.Windows.Forms.Panel pnlTransparentColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtRectangleIncrease;
        private System.Windows.Forms.Panel pnlClickAction;
        private System.Windows.Forms.RadioButton rdoNoAction;
        private System.Windows.Forms.RadioButton rdoPickSprite;
        private System.Windows.Forms.RadioButton rdoPickTile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.RadioButton rdoCrop;
        private System.Windows.Forms.CheckBox chkContinueAction;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openTilesetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTilesetToolStripMenuItem;
        private DirectionSelector dirNextPixel;
        private System.Windows.Forms.RadioButton rdoSetOrigin;
        private System.Windows.Forms.CheckBox chkShowRegion;
        private System.Windows.Forms.Button btnPreviewAnimation;
        private System.Windows.Forms.ToolStripMenuItem newSpriteSheetToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown txtMoveRecAmount;
        private System.Windows.Forms.Button btnRectangleMoveSide;
        private System.Windows.Forms.RadioButton rdoHitbox;
        private System.Windows.Forms.Button btnRectangleMoveSide2;
        private System.Windows.Forms.Button btnCropToSeam;
        private System.Windows.Forms.Button btnMoveToSide;
        private System.Windows.Forms.Button btnCrop;
        private System.Windows.Forms.NumericUpDown txtFloodFillRes;
        private System.Windows.Forms.CheckBox chkCenterOrigin;
        private System.Windows.Forms.RadioButton rdoHitbox2;
        private System.Windows.Forms.CheckBox chkCreateFlashTexture;
        private System.Windows.Forms.CheckBox chkCropToNew;
    }
}