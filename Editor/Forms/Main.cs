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
    }
}
