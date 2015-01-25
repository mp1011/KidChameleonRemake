namespace Editor
{
    partial class DirectionSelector
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
            this.chkLeft = new System.Windows.Forms.CheckBox();
            this.chkRight = new System.Windows.Forms.CheckBox();
            this.chkDown = new System.Windows.Forms.CheckBox();
            this.chkUp = new System.Windows.Forms.CheckBox();
            this.txtDegrees = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chkLeft
            // 
            this.chkLeft.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkLeft.Location = new System.Drawing.Point(5, 32);
            this.chkLeft.Name = "chkLeft";
            this.chkLeft.Size = new System.Drawing.Size(35, 23);
            this.chkLeft.TabIndex = 13;
            this.chkLeft.Text = "Left";
            this.chkLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkLeft.UseVisualStyleBackColor = true;
            this.chkLeft.CheckedChanged += new System.EventHandler(this.chkLeft_CheckedChanged);
            // 
            // chkRight
            // 
            this.chkRight.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkRight.Checked = true;
            this.chkRight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRight.Location = new System.Drawing.Point(91, 32);
            this.chkRight.Name = "chkRight";
            this.chkRight.Size = new System.Drawing.Size(42, 23);
            this.chkRight.TabIndex = 12;
            this.chkRight.Text = "Right";
            this.chkRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkRight.UseVisualStyleBackColor = true;
            this.chkRight.CheckedChanged += new System.EventHandler(this.chkRight_CheckedChanged);
            // 
            // chkDown
            // 
            this.chkDown.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkDown.Location = new System.Drawing.Point(44, 58);
            this.chkDown.Name = "chkDown";
            this.chkDown.Size = new System.Drawing.Size(45, 23);
            this.chkDown.TabIndex = 11;
            this.chkDown.Text = "Down";
            this.chkDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkDown.UseVisualStyleBackColor = true;
            this.chkDown.CheckedChanged += new System.EventHandler(this.chkDown_CheckedChanged);
            // 
            // chkUp
            // 
            this.chkUp.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkUp.Location = new System.Drawing.Point(46, 3);
            this.chkUp.Name = "chkUp";
            this.chkUp.Size = new System.Drawing.Size(40, 23);
            this.chkUp.TabIndex = 10;
            this.chkUp.Text = "Up";
            this.chkUp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkUp.UseVisualStyleBackColor = true;
            this.chkUp.CheckedChanged += new System.EventHandler(this.chkUp_CheckedChanged);
            // 
            // txtDegrees
            // 
            this.txtDegrees.Location = new System.Drawing.Point(46, 32);
            this.txtDegrees.Name = "txtDegrees";
            this.txtDegrees.Size = new System.Drawing.Size(40, 20);
            this.txtDegrees.TabIndex = 14;
            // 
            // DirectionSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txtDegrees);
            this.Controls.Add(this.chkLeft);
            this.Controls.Add(this.chkRight);
            this.Controls.Add(this.chkDown);
            this.Controls.Add(this.chkUp);
            this.Name = "DirectionSelector";
            this.Size = new System.Drawing.Size(136, 84);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkLeft;
        private System.Windows.Forms.CheckBox chkRight;
        private System.Windows.Forms.CheckBox chkDown;
        private System.Windows.Forms.CheckBox chkUp;
        private System.Windows.Forms.TextBox txtDegrees;
    }
}
