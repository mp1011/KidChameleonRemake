using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Engine.FontEditor
{
    public class FontEditorRow
    {
        public delegate void PositionChangedEventHandler(object sender, EventArgs e);
        public event PositionChangedEventHandler PositionChanged;

        public static FontEditorRow Load(string ImagePath, PositionChangedEventHandler eventHandler)
        {
            var infoFile = System.IO.Path.ChangeExtension(ImagePath, ".txt");
            FontEditorRow row = null;

            if (System.IO.File.Exists(infoFile))
                row = Serializer.FromJson<FontEditorRow>(System.IO.File.ReadAllText(infoFile));
            else
            {
                row = new FontEditorRow();
                row.ImagePath = ImagePath;
            }

            row.Image = new Bitmap(row.ImagePath);

            row.PositionChanged += eventHandler;
            return row;
        }

        public FontEditorRow()
        {
            mPositions = new int[20];
        }

       
        [JsonIgnore]
        public Bitmap Image { get; private set; }

        [Browsable(false)]
        public string ImagePath { get; set; }

        public string Text { get; set; }

        private int[] mPositions;
        public int Pos1 { get { return mPositions[0]; } set { SetPosition(0, value); } }
        public int Pos2 { get { return mPositions[1]; } set { SetPosition(1, value); } }
        public int Pos3 { get { return mPositions[2]; } set { SetPosition(2, value); } }
        public int Pos4 { get { return mPositions[3]; } set { SetPosition(3, value); } }
        public int Pos5 { get { return mPositions[4]; } set { SetPosition(4, value); } }
        public int Pos6 { get { return mPositions[5]; } set { SetPosition(5, value); } }
        public int Pos7 { get { return mPositions[6]; } set { SetPosition(6, value); } }
        public int Pos8 { get { return mPositions[7]; } set { SetPosition(7, value); } }
        public int Pos9 { get { return mPositions[8]; } set { SetPosition(8, value); } }
        public int Pos10{ get { return mPositions[9]; } set { SetPosition(9, value); } }
        public int Pos11{ get { return mPositions[10]; } set { SetPosition(10, value); } }
        public int Pos12{ get { return mPositions[11]; } set { SetPosition(11, value); } }
        public int Pos13{ get { return mPositions[12]; } set { SetPosition(12, value); } }
        public int Pos14{ get { return mPositions[13]; } set { SetPosition(13, value); } }
        public int Pos15{ get { return mPositions[14]; } set { SetPosition(14, value); } }
        public int Pos16{ get { return mPositions[15]; } set { SetPosition(15, value); } }
        public int Pos17{ get { return mPositions[16]; } set { SetPosition(16, value); } }
        public int Pos18{ get { return mPositions[17]; } set { SetPosition(17, value); } }
        public int Pos19{ get { return mPositions[18]; } set { SetPosition(18, value); } }
        public int Pos20{ get { return mPositions[19]; } set { SetPosition(19, value); } }

        [Browsable(false)]
        public int PositionCount
        {
            get
            {
                return mPositions.Skip(1).TakeWhile(i => i > 0).Count();
            }
        }

        public int GetPosition(int index)
        {
            return mPositions[index];
        }

        private void SetPosition(int index, int position)
        {
            mPositions[index] = position;
            if (PositionChanged != null)
                PositionChanged(this,new EventArgs());
        }

        public void Save()
        {
            var destPath = System.IO.Path.ChangeExtension(ImagePath, ".txt");
            System.IO.File.WriteAllText(destPath, Serializer.ToJSON(this));
        }

        public Bitmap CreateImage()
        {
            int lastPos = mPositions[0];
            int spacing = 8;
            var currentDest = new Rectangle(0, 0, mPositions[0], Image.Height);

            var imgLocations = mPositions.Skip(1).Select(xPos =>
            {
                if (xPos <= lastPos)
                    return new { Source = new Rectangle(), Dest = new Rectangle(), Padding = new Rectangle(), Valid = false };

                currentDest.Width = (xPos - lastPos);
                var loc = new
                {
                    Source = new Rectangle(lastPos, 0, (xPos - lastPos), Image.Height),
                    Dest = currentDest,
                    Padding = new Rectangle(currentDest.Right,0,spacing,Image.Height),
                    Valid=true,
                };
                lastPos = xPos;
                currentDest.X += currentDest.Width + spacing;
                return loc;
            }).Where(p=>p.Valid).ToArray();

            try
            {
                var image = new Bitmap(imgLocations.Max(p => p.Dest.Right), imgLocations.Max(p => p.Dest.Bottom));
                var gfx = System.Drawing.Graphics.FromImage(image);
                foreach (var imgLoc in imgLocations)
                {
                    gfx.DrawImage(this.Image, imgLoc.Dest, imgLoc.Source, GraphicsUnit.Pixel);
                    gfx.FillRectangle(new SolidBrush(Color.LightBlue), imgLoc.Padding);
                }
                gfx.Dispose();

                return image;
            }
            catch
            {
                return new Bitmap(8, 8);
            }
        }
     }


}
