namespace Editor.Forms
{
    partial class LevelEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlLeftRight = new System.Windows.Forms.SplitContainer();
            this.pnlMapContainer = new System.Windows.Forms.Panel();
            this.pnlMap = new Editor.TilePanelUserControl();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnRandomize = new System.Windows.Forms.Button();
            this.chkSelect = new System.Windows.Forms.CheckBox();
            this.chkDraw = new System.Windows.Forms.CheckBox();
            this.chkShowGrid = new System.Windows.Forms.CheckBox();
            this.tbZoom = new System.Windows.Forms.TrackBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.mapProperties = new System.Windows.Forms.PropertyGrid();
            this.pnlMapInfo = new System.Windows.Forms.Panel();
            this.btnApplyMapInfo = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pnlTileset = new Editor.TilePanelUserControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propSpecialTile = new System.Windows.Forms.PropertyGrid();
            this.lstGroups = new System.Windows.Forms.CheckedListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlLeftRight)).BeginInit();
            this.pnlLeftRight.Panel1.SuspendLayout();
            this.pnlLeftRight.Panel2.SuspendLayout();
            this.pnlLeftRight.SuspendLayout();
            this.pnlMapContainer.SuspendLayout();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pnlMapInfo.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // pnlLeftRight
            // 
            resources.ApplyResources(this.pnlLeftRight, "pnlLeftRight");
            this.pnlLeftRight.Name = "pnlLeftRight";
            // 
            // pnlLeftRight.Panel1
            // 
            this.pnlLeftRight.Panel1.Controls.Add(this.pnlMapContainer);
            // 
            // pnlLeftRight.Panel2
            // 
            this.pnlLeftRight.Panel2.Controls.Add(this.tabControl1);
            // 
            // pnlMapContainer
            // 
            this.pnlMapContainer.Controls.Add(this.pnlMap);
            this.pnlMapContainer.Controls.Add(this.pnlTop);
            resources.ApplyResources(this.pnlMapContainer, "pnlMapContainer");
            this.pnlMapContainer.Name = "pnlMapContainer";
            // 
            // pnlMap
            // 
            resources.ApplyResources(this.pnlMap, "pnlMap");
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.SelectionMode = Editor.SelectionMode.None;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnRandomize);
            this.pnlTop.Controls.Add(this.chkSelect);
            this.pnlTop.Controls.Add(this.chkDraw);
            this.pnlTop.Controls.Add(this.chkShowGrid);
            this.pnlTop.Controls.Add(this.tbZoom);
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // btnRandomize
            // 
            resources.ApplyResources(this.btnRandomize, "btnRandomize");
            this.btnRandomize.Name = "btnRandomize";
            this.btnRandomize.UseVisualStyleBackColor = true;
            this.btnRandomize.Click += new System.EventHandler(this.btnRandomize_Click);
            // 
            // chkSelect
            // 
            resources.ApplyResources(this.chkSelect, "chkSelect");
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.UseVisualStyleBackColor = true;
            this.chkSelect.CheckedChanged += new System.EventHandler(this.chkSelect_CheckedChanged);
            // 
            // chkDraw
            // 
            resources.ApplyResources(this.chkDraw, "chkDraw");
            this.chkDraw.Checked = true;
            this.chkDraw.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDraw.Name = "chkDraw";
            this.chkDraw.UseVisualStyleBackColor = true;
            this.chkDraw.CheckedChanged += new System.EventHandler(this.chkDraw_CheckedChanged);
            // 
            // chkShowGrid
            // 
            resources.ApplyResources(this.chkShowGrid, "chkShowGrid");
            this.chkShowGrid.Checked = true;
            this.chkShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowGrid.Name = "chkShowGrid";
            this.chkShowGrid.UseVisualStyleBackColor = true;
            this.chkShowGrid.CheckedChanged += new System.EventHandler(this.chkShowGrid_CheckedChanged);
            // 
            // tbZoom
            // 
            resources.ApplyResources(this.tbZoom, "tbZoom");
            this.tbZoom.Minimum = 1;
            this.tbZoom.Name = "tbZoom";
            this.tbZoom.Value = 1;
            this.tbZoom.Scroll += new System.EventHandler(this.tbZoom_Scroll);
            this.tbZoom.ValueChanged += new System.EventHandler(this.tbZoom_ValueChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.mapProperties);
            this.tabPage2.Controls.Add(this.pnlMapInfo);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // mapProperties
            // 
            resources.ApplyResources(this.mapProperties, "mapProperties");
            this.mapProperties.Name = "mapProperties";
            this.mapProperties.Click += new System.EventHandler(this.mapProperties_Click);
            // 
            // pnlMapInfo
            // 
            this.pnlMapInfo.Controls.Add(this.btnApplyMapInfo);
            resources.ApplyResources(this.pnlMapInfo, "pnlMapInfo");
            this.pnlMapInfo.Name = "pnlMapInfo";
            // 
            // btnApplyMapInfo
            // 
            resources.ApplyResources(this.btnApplyMapInfo, "btnApplyMapInfo");
            this.btnApplyMapInfo.Name = "btnApplyMapInfo";
            this.btnApplyMapInfo.UseVisualStyleBackColor = true;
            this.btnApplyMapInfo.Click += new System.EventHandler(this.btnApplyMapInfo_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pnlTileset);
            this.tabPage1.Controls.Add(this.panel1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pnlTileset
            // 
            resources.ApplyResources(this.pnlTileset, "pnlTileset");
            this.pnlTileset.Name = "pnlTileset";
            this.pnlTileset.SelectionMode = Editor.SelectionMode.None;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.propSpecialTile);
            this.panel1.Controls.Add(this.lstGroups);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // propSpecialTile
            // 
            resources.ApplyResources(this.propSpecialTile, "propSpecialTile");
            this.propSpecialTile.Name = "propSpecialTile";
            this.propSpecialTile.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propSpecialTile_PropertyValueChanged);
            // 
            // lstGroups
            // 
            this.lstGroups.CheckOnClick = true;
            resources.ApplyResources(this.lstGroups, "lstGroups");
            this.lstGroups.FormattingEnabled = true;
            this.lstGroups.Name = "lstGroups";
            this.lstGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstGroups_ItemCheck);
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // LevelEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlLeftRight);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LevelEditor";
            this.Load += new System.EventHandler(this.LevelEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LevelEditor_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlLeftRight.Panel1.ResumeLayout(false);
            this.pnlLeftRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlLeftRight)).EndInit();
            this.pnlLeftRight.ResumeLayout(false);
            this.pnlMapContainer.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.pnlMapInfo.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMapContainer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private TilePanelUserControl pnlTileset;
        private System.Windows.Forms.PropertyGrid mapProperties;
        private System.Windows.Forms.Panel pnlMapInfo;
        private System.Windows.Forms.Button btnApplyMapInfo;
        private TilePanelUserControl pnlMap;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.TrackBar tbZoom;
        private System.Windows.Forms.CheckBox chkShowGrid;
        private System.Windows.Forms.CheckBox chkSelect;
        private System.Windows.Forms.CheckBox chkDraw;
        private System.Windows.Forms.Button btnRandomize;
        private System.Windows.Forms.SplitContainer pnlLeftRight;
        private System.Windows.Forms.CheckedListBox lstGroups;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PropertyGrid propSpecialTile;
    }
}