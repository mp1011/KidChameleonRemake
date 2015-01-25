using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor.Forms
{
    partial class AnimationPreview : Form
    {
        private BitmapPortion[] mFrames;
        private int mCurrentIndex = 0;

        public AnimationPreview(IEnumerable<BitmapPortion> frames)
        {
            InitializeComponent();
            RefreshOrigin();
            mFrames = frames.ToArray();
            timer1.Interval = tbSpeed.Value;
        }

        private void RefreshOrigin()
        {
            imagePanel.DisplayOrigin = new Engine.RGPointI(imagePanel.Width / 2f, imagePanel.Height - 16);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mCurrentIndex == mFrames.Length - 1)
                mCurrentIndex = 0;
            else
                mCurrentIndex++;

            imagePanel.Image = mFrames[mCurrentIndex];
        }

        private void tbSpeed_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = tbSpeed.Value;
        }

        private void imagePanel_Resize(object sender, EventArgs e)
        {
            RefreshOrigin();
        }
    }
}
