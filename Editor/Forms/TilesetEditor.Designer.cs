namespace Editor.Forms
{
    partial class TilesetEditor
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMain = new System.Windows.Forms.SplitContainer();
            this.pnlTileset = new Editor.TilePanelUserControl();
            this.pnlBase = new Editor.TilePanelUserControl();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tileProperties = new System.Windows.Forms.PropertyGrid();
            this.pnlProperties = new System.Windows.Forms.Panel();
            this.btnCopyFromSelected = new System.Windows.Forms.Button();
            this.chkProperties = new System.Windows.Forms.CheckedListBox();
            this.btnCopyProperties = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnlMatches = new Editor.TilePanelUserControl();
            this.pnlGroupBox = new System.Windows.Forms.Panel();
            this.btnMakeEqual = new System.Windows.Forms.Button();
            this.btnAutoDetect = new System.Windows.Forms.Button();
            this.pnlMatchPreview = new Editor.TilePanelUserControl();
            this.btnAutoOrganize = new System.Windows.Forms.Button();
            this.matchDirectionPicker = new Editor.DirectionSelector();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.pnlTileGroups = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lstGroups = new System.Windows.Forms.CheckedListBox();
            this.txtNewGroup = new System.Windows.Forms.TextBox();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.pnlTileFilter = new System.Windows.Forms.Panel();
            this.tileFilter = new Editor.GroupFilter();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
            this.pnlMain.Panel1.SuspendLayout();
            this.pnlMain.Panel2.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.pnlProperties.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pnlGroupBox.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            this.pnlTileGroups.SuspendLayout();
            this.pnlTileFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1147, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 24);
            this.pnlMain.Name = "pnlMain";
            // 
            // pnlMain.Panel1
            // 
            this.pnlMain.Panel1.Controls.Add(this.pnlTileset);
            this.pnlMain.Panel1.Controls.Add(this.pnlBase);
            // 
            // pnlMain.Panel2
            // 
            this.pnlMain.Panel2.Controls.Add(this.tabMain);
            this.pnlMain.Panel2.Controls.Add(this.pnlFilter);
            this.pnlMain.Size = new System.Drawing.Size(1147, 620);
            this.pnlMain.SplitterDistance = 484;
            this.pnlMain.TabIndex = 3;
            // 
            // pnlTileset
            // 
            this.pnlTileset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTileset.Location = new System.Drawing.Point(0, 0);
            this.pnlTileset.Name = "pnlTileset";
            this.pnlTileset.RectangleType = Editor.DrawRectangleType.None;
            this.pnlTileset.SelectionMode = Editor.SelectionMode.Multi;
            this.pnlTileset.Size = new System.Drawing.Size(484, 487);
            this.pnlTileset.TabIndex = 0;
            this.pnlTileset.Load += new System.EventHandler(this.pnlTileset_Load);
            // 
            // pnlBase
            // 
            this.pnlBase.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBase.Location = new System.Drawing.Point(0, 487);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.RectangleType = Editor.DrawRectangleType.ShiftDrag;
            this.pnlBase.SelectionMode = Editor.SelectionMode.None;
            this.pnlBase.Size = new System.Drawing.Size(484, 133);
            this.pnlBase.TabIndex = 1;
            this.pnlBase.Load += new System.EventHandler(this.pnlBase_Load);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabPage2);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 267);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(659, 353);
            this.tabMain.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tileProperties);
            this.tabPage1.Controls.Add(this.pnlProperties);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(651, 327);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Properties";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tileProperties
            // 
            this.tileProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileProperties.Location = new System.Drawing.Point(3, 112);
            this.tileProperties.Name = "tileProperties";
            this.tileProperties.Size = new System.Drawing.Size(645, 212);
            this.tileProperties.TabIndex = 0;
            // 
            // pnlProperties
            // 
            this.pnlProperties.Controls.Add(this.btnCopyFromSelected);
            this.pnlProperties.Controls.Add(this.chkProperties);
            this.pnlProperties.Controls.Add(this.btnCopyProperties);
            this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProperties.Location = new System.Drawing.Point(3, 3);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new System.Drawing.Size(645, 109);
            this.pnlProperties.TabIndex = 7;
            // 
            // btnCopyFromSelected
            // 
            this.btnCopyFromSelected.Location = new System.Drawing.Point(249, 13);
            this.btnCopyFromSelected.Name = "btnCopyFromSelected";
            this.btnCopyFromSelected.Size = new System.Drawing.Size(122, 31);
            this.btnCopyFromSelected.TabIndex = 3;
            this.btnCopyFromSelected.Text = "Copy From Selected";
            this.btnCopyFromSelected.UseVisualStyleBackColor = true;
            this.btnCopyFromSelected.Click += new System.EventHandler(this.btnCopyFromSelected_Click);
            // 
            // chkProperties
            // 
            this.chkProperties.FormattingEnabled = true;
            this.chkProperties.Location = new System.Drawing.Point(12, 13);
            this.chkProperties.Name = "chkProperties";
            this.chkProperties.Size = new System.Drawing.Size(231, 79);
            this.chkProperties.TabIndex = 2;
            // 
            // btnCopyProperties
            // 
            this.btnCopyProperties.Location = new System.Drawing.Point(249, 61);
            this.btnCopyProperties.Name = "btnCopyProperties";
            this.btnCopyProperties.Size = new System.Drawing.Size(114, 31);
            this.btnCopyProperties.TabIndex = 1;
            this.btnCopyProperties.Text = "Copy From Base";
            this.btnCopyProperties.UseVisualStyleBackColor = true;
            this.btnCopyProperties.Click += new System.EventHandler(this.btnCopyProperties_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pnlMatches);
            this.tabPage2.Controls.Add(this.pnlGroupBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(651, 327);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Usage";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnlMatches
            // 
            this.pnlMatches.BackColor = System.Drawing.Color.Gray;
            this.pnlMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMatches.Location = new System.Drawing.Point(211, 3);
            this.pnlMatches.Name = "pnlMatches";
            this.pnlMatches.RectangleType = Editor.DrawRectangleType.ShiftDrag;
            this.pnlMatches.SelectionMode = Editor.SelectionMode.None;
            this.pnlMatches.Size = new System.Drawing.Size(437, 321);
            this.pnlMatches.TabIndex = 2;
            // 
            // pnlGroupBox
            // 
            this.pnlGroupBox.Controls.Add(this.btnMakeEqual);
            this.pnlGroupBox.Controls.Add(this.btnAutoDetect);
            this.pnlGroupBox.Controls.Add(this.pnlMatchPreview);
            this.pnlGroupBox.Controls.Add(this.btnAutoOrganize);
            this.pnlGroupBox.Controls.Add(this.matchDirectionPicker);
            this.pnlGroupBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlGroupBox.Location = new System.Drawing.Point(3, 3);
            this.pnlGroupBox.Name = "pnlGroupBox";
            this.pnlGroupBox.Size = new System.Drawing.Size(208, 321);
            this.pnlGroupBox.TabIndex = 10;
            // 
            // btnMakeEqual
            // 
            this.btnMakeEqual.Location = new System.Drawing.Point(14, 267);
            this.btnMakeEqual.Name = "btnMakeEqual";
            this.btnMakeEqual.Size = new System.Drawing.Size(111, 20);
            this.btnMakeEqual.TabIndex = 18;
            this.btnMakeEqual.Text = "Make Equal";
            this.btnMakeEqual.UseVisualStyleBackColor = true;
            this.btnMakeEqual.Click += new System.EventHandler(this.btnMakeEqual_Click);
            // 
            // btnAutoDetect
            // 
            this.btnAutoDetect.Location = new System.Drawing.Point(14, 293);
            this.btnAutoDetect.Name = "btnAutoDetect";
            this.btnAutoDetect.Size = new System.Drawing.Size(111, 23);
            this.btnAutoDetect.TabIndex = 17;
            this.btnAutoDetect.Text = "Auto Detect All";
            this.btnAutoDetect.UseVisualStyleBackColor = true;
            this.btnAutoDetect.Click += new System.EventHandler(this.btnAutoDetect_Click);
            // 
            // pnlMatchPreview
            // 
            this.pnlMatchPreview.Location = new System.Drawing.Point(14, 103);
            this.pnlMatchPreview.Name = "pnlMatchPreview";
            this.pnlMatchPreview.RectangleType = Editor.DrawRectangleType.ShiftDrag;
            this.pnlMatchPreview.SelectionMode = Editor.SelectionMode.None;
            this.pnlMatchPreview.Size = new System.Drawing.Size(181, 161);
            this.pnlMatchPreview.TabIndex = 16;
            // 
            // btnAutoOrganize
            // 
            this.btnAutoOrganize.Location = new System.Drawing.Point(21, 464);
            this.btnAutoOrganize.Name = "btnAutoOrganize";
            this.btnAutoOrganize.Size = new System.Drawing.Size(160, 31);
            this.btnAutoOrganize.TabIndex = 13;
            this.btnAutoOrganize.Text = "Auto Organize";
            this.btnAutoOrganize.UseVisualStyleBackColor = true;
            this.btnAutoOrganize.Click += new System.EventHandler(this.btnAutoOrganize_Click);
            // 
            // matchDirectionPicker
            // 
            this.matchDirectionPicker.AllowDiagonals = false;
            this.matchDirectionPicker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.matchDirectionPicker.Location = new System.Drawing.Point(26, 13);
            this.matchDirectionPicker.Name = "matchDirectionPicker";
            this.matchDirectionPicker.Size = new System.Drawing.Size(136, 84);
            this.matchDirectionPicker.TabIndex = 1;
            this.matchDirectionPicker.DirectionChanged += new Editor.DirectionSelector.DirectionChangedEventHandler(this.matchDirectionPicker_DirectionChanged);
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.pnlTileGroups);
            this.pnlFilter.Controls.Add(this.pnlTileFilter);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(659, 267);
            this.pnlFilter.TabIndex = 6;
            // 
            // pnlTileGroups
            // 
            this.pnlTileGroups.Controls.Add(this.label1);
            this.pnlTileGroups.Controls.Add(this.lstGroups);
            this.pnlTileGroups.Controls.Add(this.txtNewGroup);
            this.pnlTileGroups.Controls.Add(this.btnAddGroup);
            this.pnlTileGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTileGroups.Location = new System.Drawing.Point(244, 0);
            this.pnlTileGroups.Name = "pnlTileGroups";
            this.pnlTileGroups.Size = new System.Drawing.Size(415, 267);
            this.pnlTileGroups.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Tile Groups";
            // 
            // lstGroups
            // 
            this.lstGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstGroups.FormattingEnabled = true;
            this.lstGroups.Location = new System.Drawing.Point(15, 35);
            this.lstGroups.Name = "lstGroups";
            this.lstGroups.Size = new System.Drawing.Size(388, 184);
            this.lstGroups.TabIndex = 11;
            this.lstGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstGroups_ItemCheck);
            // 
            // txtNewGroup
            // 
            this.txtNewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewGroup.Location = new System.Drawing.Point(15, 228);
            this.txtNewGroup.Name = "txtNewGroup";
            this.txtNewGroup.Size = new System.Drawing.Size(351, 20);
            this.txtNewGroup.TabIndex = 12;
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddGroup.Location = new System.Drawing.Point(372, 225);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(31, 23);
            this.btnAddGroup.TabIndex = 13;
            this.btnAddGroup.Text = "+";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // pnlTileFilter
            // 
            this.pnlTileFilter.Controls.Add(this.tileFilter);
            this.pnlTileFilter.Controls.Add(this.label2);
            this.pnlTileFilter.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlTileFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlTileFilter.Name = "pnlTileFilter";
            this.pnlTileFilter.Size = new System.Drawing.Size(244, 267);
            this.pnlTileFilter.TabIndex = 17;
            // 
            // tileFilter
            // 
            this.tileFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileFilter.Location = new System.Drawing.Point(0, 13);
            this.tileFilter.Name = "tileFilter";
            this.tileFilter.Size = new System.Drawing.Size(244, 254);
            this.tileFilter.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Filter";
            // 
            // TilesetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 644);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TilesetEditor";
            this.Text = "Tileset";
            this.Resize += new System.EventHandler(this.TilesetEditor_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlMain.Panel1.ResumeLayout(false);
            this.pnlMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.pnlProperties.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.pnlGroupBox.ResumeLayout(false);
            this.pnlFilter.ResumeLayout(false);
            this.pnlTileGroups.ResumeLayout(false);
            this.pnlTileGroups.PerformLayout();
            this.pnlTileFilter.ResumeLayout(false);
            this.pnlTileFilter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TilePanelUserControl pnlTileset;
        private TilePanelUserControl pnlBase;
        private System.Windows.Forms.Button btnCopyProperties;
        private System.Windows.Forms.PropertyGrid tileProperties;
        private System.Windows.Forms.CheckedListBox chkProperties;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnCopyFromSelected;
        private System.Windows.Forms.Panel pnlProperties;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.SplitContainer pnlMain;
        private System.Windows.Forms.Panel pnlGroupBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnAutoOrganize;
        private TilePanelUserControl pnlMatches;
        private DirectionSelector matchDirectionPicker;
        private TilePanelUserControl pnlMatchPreview;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.TextBox txtNewGroup;
        private System.Windows.Forms.CheckedListBox lstGroups;
        private GroupFilter tileFilter;
        private System.Windows.Forms.Panel pnlTileGroups;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlTileFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAutoDetect;
        private System.Windows.Forms.Button btnMakeEqual;



    }
}