using Engine;
using Engine.FontEditor;
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
    public partial class FontEditor : Form
    {
        #region Instance

        private static FontEditor mInstance;

        public static FontEditor GetOrOpen()
        {
            if (mInstance == null || mInstance.IsDisposed)
            {
                mInstance = new FontEditor();
                mInstance.Show();
            }

            return mInstance;
        }

        public static FontEditor GetInstance()
        {
            if (mInstance == null || mInstance.IsDisposed)
                return null;

            return mInstance;
        }

#if DESIGNMODE
        public TilesetEditor()
        {       
          InitializeComponent();
        }
#endif

        #endregion

        private FontEditorRow[] EditorRows
        {
            get
            {
                return (FontEditorRow[])dgFont.DataSource;
            }
            set
            {
                dgFont.DataSource = value;
            }
        }

        public FontEditor()
        {
            InitializeComponent();            
            LoadImages(@"D:\Games\Emulation\Genesis\gensKMod_07\Screenshots\BigFont");
        }

        private void dgFont_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void LoadImages(string folder)
        {
            this.EditorRows = System.IO.Directory.GetFiles(folder, "*.png").Select(filename => FontEditorRow.Load(filename, ImagePositionChanged)).ToArray();            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var row in EditorRows)
                row.Save();
        }

        private void ImagePositionChanged(object sender, EventArgs args)
        {
            var row = sender as FontEditorRow;
            if (row != null)
                imgFont.Image = new BitmapPortion(row.CreateImage());
        }

        private void dgFont_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("X");
        }


        private void dgFont_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.TextChanged += (sndr, args) => { dgFont.CommitEdit(DataGridViewDataErrorContexts.Commit); };
            //e.Control.KeyPress += (sndr, args) => { dgFont.CommitEdit(DataGridViewDataErrorContexts.Commit); };
        }

        private void dgFont_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void exportToFontTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            FileDialog.ShowSaveDialog<VariableSpaceFont[]>(PathType.Fonts.GetFolder(), "txt", path =>
                {
                    int index=1;

                    var fonts = BuildFonts().ToArray();
                    foreach (var font in fonts)
                    {
                        var fontPath = path.Substring(0,path.LastIndexOf('.'))  + index + ".txt";
                        FileDialog.SaveObject(font, fontPath);
                        var bitmap = font.FontTexture.GetImage();
                        bitmap.Save(font.FontTexture.Path.FullPath);
                        index++;
                    }
                    return fonts;
                });
        }


        private IEnumerable<VariableSpaceFont> BuildFonts()
        {
            var cells = this.EditorRows.SelectMany(p => FontEditorCell.Create(p)).ToArray();
            
            var palettes = new ImagePaletteCollection(cells.Select(p => p.Image));
            var output = new List<ColorKeyedImage>();

            
            foreach(var lettergroup in cells.GroupBy(p=>p.Letter).OrderBy(p=>p.Key))
            {
                output.Add(new ColorKeyedImage(lettergroup.Select(p=>p.Image)));
            }

            int index = 0;
            foreach (var palette in palettes.Palettes)
            {
                index++;
                var ss = BitmapPortion.CreateSpriteSheet(output.Select(p=> p.CreateImage(palette)), cells.Take(12).Sum(p => p.Image.Region.Width), "Font" + index, Color.Transparent);

                var font = new VariableSpaceFont(ss.Image);
                var frames = ss.Frames.Select(p => p.Source).ToArray();
                var letters = cells.Select(p => p.Letter).Distinct().OrderBy(c => c).ToArray();

                for (int rangeIndex = 0; rangeIndex < frames.Count(); rangeIndex++)
                    font.AddLetter(letters[rangeIndex], frames[rangeIndex]);

                yield return font;
            }
        }


    }

    public class FontEditorCell
    {
        public static IEnumerable<FontEditorCell> Create(FontEditorRow row)
        {
            int lastX = row.GetPosition(0);
            for(int index = 0; index < row.PositionCount;index++)
            {
                var xPos = row.GetPosition(index + 1);
                if (xPos > lastX)
                {                
                    var portion = new BitmapPortion(row.Image);
                    portion.Crop(RGRectangleI.FromXYWH(lastX, 0, xPos - lastX, row.Image.Height));
                    yield return new FontEditorCell { Image = portion , Letter = row.Text[index]};
                    lastX = xPos;
                }
            }
        }

        public char Letter { get; private set; }
        internal BitmapPortion Image { get; private set; }
     }

    class ColorKeyedImage
    {        
        private byte[,] mColorKey;
        private ImagePalette[] mPalettes;

        public ColorKeyedImage(IEnumerable<BitmapPortion> sources)
        {
            mPalettes = sources.Select(p => p.GetPallete()).Distinct().ToArray();
            mColorKey = sources.First().ToColorMap();
        }

        public BitmapPortion  CreateImage(ImagePalette palette)
        {
            var image = new Bitmap(mColorKey.GetLength(0), mColorKey.GetLength(1));
            for(int y =0; y < image.Height;y++)
                for (int x = 0; x < image.Width; x++)
                {
                    image.SetPixel(x,y,palette.GetColor(mColorKey[x, y]));
                }

            var portion = new BitmapPortion(image);
            portion.Crop(RGRectangleI.FromXYWH(0, 0, image.Width, image.Height));
            return portion;
        }
    }   
   
}
