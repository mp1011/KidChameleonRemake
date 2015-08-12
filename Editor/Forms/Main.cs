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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

         //   TileUsageHelper.BuildMatchesFromFolder(@"D:\Games\Emulation\Genesis\gensKMod_07\Screenshots\woods", "woods");
       
        }

        private void btnGraphicExtractor_Click(object sender, EventArgs e)
        {
            new GraphicsExtractor().Show();
        }

        private void btnLevelEditor_Click(object sender, EventArgs e)
        {
            LevelEditor.GetOrOpen();
        }

        private void btnTileEditor_Click(object sender, EventArgs e)
        {
            TilesetEditor.GetOrOpen();
        }

        private void btnFontEditor_Click(object sender, EventArgs e)
        {
            FontEditor.GetOrOpen();
        }

        private void btnShowLog_Click(object sender, EventArgs e)
        {
            var log = new frmLog();
            log.Show();
        }
    }
}
