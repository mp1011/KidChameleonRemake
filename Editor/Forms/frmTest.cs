using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Editor.Forms
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
            var img = new BitmapPortion(@"D:\Games\Emulation\Genesis\gensKMod_07\Screenshots\test_001.bmp");         
            bitmapPortionPanelUserControl1.Image = img;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bitmapPortionPanelUserControl1.RefreshImage();
        }
    }
}
