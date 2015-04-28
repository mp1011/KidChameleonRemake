namespace Editor.Forms
{
    partial class Main
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
            this.btnLevelEditor = new System.Windows.Forms.Button();
            this.btnGraphicExtractor = new System.Windows.Forms.Button();
            this.btnTileEditor = new System.Windows.Forms.Button();
            this.btnFontEditor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLevelEditor
            // 
            this.btnLevelEditor.Location = new System.Drawing.Point(25, 37);
            this.btnLevelEditor.Name = "btnLevelEditor";
            this.btnLevelEditor.Size = new System.Drawing.Size(114, 37);
            this.btnLevelEditor.TabIndex = 0;
            this.btnLevelEditor.Text = "Level Editor";
            this.btnLevelEditor.UseVisualStyleBackColor = true;
            this.btnLevelEditor.Click += new System.EventHandler(this.btnLevelEditor_Click);
            // 
            // btnGraphicExtractor
            // 
            this.btnGraphicExtractor.Location = new System.Drawing.Point(25, 80);
            this.btnGraphicExtractor.Name = "btnGraphicExtractor";
            this.btnGraphicExtractor.Size = new System.Drawing.Size(114, 40);
            this.btnGraphicExtractor.TabIndex = 1;
            this.btnGraphicExtractor.Text = "Graphic Extractor";
            this.btnGraphicExtractor.UseVisualStyleBackColor = true;
            this.btnGraphicExtractor.Click += new System.EventHandler(this.btnGraphicExtractor_Click);
            // 
            // btnTileEditor
            // 
            this.btnTileEditor.Location = new System.Drawing.Point(25, 126);
            this.btnTileEditor.Name = "btnTileEditor";
            this.btnTileEditor.Size = new System.Drawing.Size(114, 37);
            this.btnTileEditor.TabIndex = 2;
            this.btnTileEditor.Text = "Tileset Editor";
            this.btnTileEditor.UseVisualStyleBackColor = true;
            this.btnTileEditor.Click += new System.EventHandler(this.btnTileEditor_Click);
            // 
            // btnFontEditor
            // 
            this.btnFontEditor.Location = new System.Drawing.Point(25, 169);
            this.btnFontEditor.Name = "btnFontEditor";
            this.btnFontEditor.Size = new System.Drawing.Size(114, 37);
            this.btnFontEditor.TabIndex = 3;
            this.btnFontEditor.Text = "Font Editor";
            this.btnFontEditor.UseVisualStyleBackColor = true;
            this.btnFontEditor.Click += new System.EventHandler(this.btnFontEditor_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 262);
            this.Controls.Add(this.btnFontEditor);
            this.Controls.Add(this.btnTileEditor);
            this.Controls.Add(this.btnGraphicExtractor);
            this.Controls.Add(this.btnLevelEditor);
            this.Name = "Main";
            this.Text = "Main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLevelEditor;
        private System.Windows.Forms.Button btnGraphicExtractor;
        private System.Windows.Forms.Button btnTileEditor;
        private System.Windows.Forms.Button btnFontEditor;

    }
}