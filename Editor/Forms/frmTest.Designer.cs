namespace Editor.Forms
{
    partial class frmTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.bitmapPortionPanelUserControl1 = new Editor.BitmapPortionPanelUserControl(true);
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(0, 239);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(284, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bitmapPortionPanelUserControl1
            // 
            this.bitmapPortionPanelUserControl1.DisplayOrigin = null;
            this.bitmapPortionPanelUserControl1.DisplayPortionOnly = false;
            this.bitmapPortionPanelUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapPortionPanelUserControl1.DrawRectangle = DrawRectangleType.None;
            this.bitmapPortionPanelUserControl1.EnableSelector = false;
            this.bitmapPortionPanelUserControl1.Image = null;
            this.bitmapPortionPanelUserControl1.Location = new System.Drawing.Point(0, 0);
            this.bitmapPortionPanelUserControl1.Name = "bitmapPortionPanelUserControl1";
            this.bitmapPortionPanelUserControl1.Size = new System.Drawing.Size(284, 239);
            this.bitmapPortionPanelUserControl1.TabIndex = 1;
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.bitmapPortionPanelUserControl1);
            this.Controls.Add(this.button1);
            this.Name = "frmTest";
            this.Text = "frmTest";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private BitmapPortionPanelUserControl bitmapPortionPanelUserControl1;
    }
}