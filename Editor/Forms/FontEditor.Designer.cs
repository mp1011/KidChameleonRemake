namespace Editor.Forms
{
    partial class FontEditor
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
            this.dgFont = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.imgFont = new Editor.BitmapPortionPanelUserControl();
            this.exportToFontTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgFont)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgFont
            // 
            this.dgFont.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFont.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgFont.Location = new System.Drawing.Point(0, 24);
            this.dgFont.Name = "dgFont";
            this.dgFont.Size = new System.Drawing.Size(782, 276);
            this.dgFont.TabIndex = 0;
            this.dgFont.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgFont_CellContentClick);
            this.dgFont.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgFont_DataError);
            this.dgFont.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgFont_EditingControlShowing);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(782, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exportToFontTextureToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(187, 6);
            // 
            // imgFont
            // 
            this.imgFont.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.imgFont.CenterOnOrigin = false;
            this.imgFont.DisplayOrigin = null;
            this.imgFont.DisplayPortionOnly = false;
            this.imgFont.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.imgFont.DrawRectangle = Editor.DrawRectangleType.None;
            this.imgFont.EnableSelector = false;
            this.imgFont.Image = null;
            this.imgFont.Location = new System.Drawing.Point(0, 300);
            this.imgFont.Name = "imgFont";
            this.imgFont.Selected = false;
            this.imgFont.Size = new System.Drawing.Size(782, 200);
            this.imgFont.TabIndex = 2;
            // 
            // exportToFontTextureToolStripMenuItem
            // 
            this.exportToFontTextureToolStripMenuItem.Name = "exportToFontTextureToolStripMenuItem";
            this.exportToFontTextureToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.exportToFontTextureToolStripMenuItem.Text = "Export to Font Texture";
            this.exportToFontTextureToolStripMenuItem.Click += new System.EventHandler(this.exportToFontTextureToolStripMenuItem_Click);
            // 
            // FontEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 500);
            this.Controls.Add(this.dgFont);
            this.Controls.Add(this.imgFont);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FontEditor";
            this.Text = "FontEditor";
            ((System.ComponentModel.ISupportInitialize)(this.dgFont)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgFont;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private BitmapPortionPanelUserControl imgFont;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportToFontTextureToolStripMenuItem;
    }
}