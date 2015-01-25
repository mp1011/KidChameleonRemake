namespace Editor
{
    partial class BitmapPortionPanelUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlImage = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlImage
            // 
            this.pnlImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlImage.BackColor = System.Drawing.Color.White;
            this.pnlImage.Location = new System.Drawing.Point(15, 13);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(715, 577);
            this.pnlImage.TabIndex = 0;
            this.pnlImage.Click += new System.EventHandler(this.pnlImage_Click);
            // 
            // BitmapPortionPanelUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.Controls.Add(this.pnlImage);
            this.DoubleBuffered = true;
            this.Name = "BitmapPortionPanelUserControl";
            this.Size = new System.Drawing.Size(742, 607);
            this.Click += new System.EventHandler(this.pnlImage_Click);
            this.Resize += new System.EventHandler(this.BitmapPortionPanelUserControl_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlImage;

    }
}
