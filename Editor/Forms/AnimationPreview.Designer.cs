namespace Editor.Forms
{
    partial class AnimationPreview
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
            this.tbSpeed = new System.Windows.Forms.TrackBar();
            this.imagePanel = new Editor.BitmapPortionPanelUserControl(false);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // tbSpeed
            // 
            this.tbSpeed.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbSpeed.Location = new System.Drawing.Point(0, 363);
            this.tbSpeed.Maximum = 1000;
            this.tbSpeed.Minimum = 1;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(601, 45);
            this.tbSpeed.TabIndex = 0;
            this.tbSpeed.Value = 500;
            this.tbSpeed.Scroll += new System.EventHandler(this.tbSpeed_Scroll);
            // 
            // imagePanel
            // 
            this.imagePanel.CenterOnOrigin = true;
            this.imagePanel.DisplayOrigin = null;
            this.imagePanel.DisplayPortionOnly = true;
            this.imagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagePanel.DrawRectangle = false;
            this.imagePanel.EnableSelector = false;
            this.imagePanel.Image = null;
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(601, 363);
            this.imagePanel.TabIndex = 1;
            this.imagePanel.Resize += new System.EventHandler(this.imagePanel_Resize);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AnimationPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 408);
            this.Controls.Add(this.imagePanel);
            this.Controls.Add(this.tbSpeed);
            this.Name = "AnimationPreview";
            this.Text = "AnimationPreview";
            ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbSpeed;
        private BitmapPortionPanelUserControl imagePanel;
        private System.Windows.Forms.Timer timer1;
    }
}