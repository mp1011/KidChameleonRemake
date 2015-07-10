using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Engine;

namespace Editor
{
    class BitmapPortion : IHasHitboxes
    {
        private string mPath;
        private Bitmap mImage;

        public Bitmap Image
        {
            get
            {
                if (mImage == null)
                {
                    var stream = System.IO.File.OpenRead(mPath);
                    var image = new Bitmap(stream);
                    mImage = image.Clone(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    image.Dispose();
                    stream.Dispose();

                    mImageSize = new RGSizeI(mImage.Width, mImage.Height);
                    RecalcBitmapData();
                }
                return mImage;
            }
            private set
            {
                mImage = value;
                mImageSize = new RGSizeI(mImage.Width, mImage.Height);
                RecalcBitmapData();
            }
        }

        private RGRectangleI mRegion;
        public RGRectangleI Region
        {
            get
            {
                return mRegion;
            }
            private set
            {
                mRegion = value.CropWithin(RGRectangleI.FromXYWH(0, 0, this.Width, this.Height));
                RecalcBitmapData();
            }
        }

        private BitmapPortion(BitmapPortion original, RGRectangleI newArea)
        {
            mPath = original.mPath;
            this.Image = original.Image;
            Region = newArea;
        }

        public BitmapPortion(string path, bool calculateImageDataNow)
        {
            mPath = path;

            if (calculateImageDataNow)
                Region = RGRectangleI.FromXYWH(0, 0, Image.Width, Image.Height);
        }

        public BitmapPortion(string path)
            : this(path, true)
        {

        }

        public BitmapPortion(Bitmap image)
        {
            mImage = image;
            mImageSize = new RGSizeI(mImage.Width, mImage.Height);
            RecalcBitmapData();
        }

        #region Bitmap Properties

        private RGSizeI mImageSize;
        private List<Color> mPixelData = new List<Color>();

        public int Width { get { return mImageSize.Width; } }
        public int Height { get { return mImageSize.Height; } }

        public RGSizeI Size { get { return new RGSizeI(Width, Height); } }

        private static ulong totalBytesLoaded = 0;

        private void RecalcBitmapData()
        {
            if (Region.IsEmpty || (Region.Width == this.Width && Region.Height == this.Height))
            {
                mPixelData.Clear();
                return;
            }

            //we must lock the whole image, even if a portion if specified the Stride is still the width of the entire image


            // http://stackoverflow.com/questions/4747428/getting-rgb-array-from-image-in-c-sharp

            System.Drawing.Imaging.BitmapData data = null;
            try
            {
                data = this.Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                mPixelData.Clear();
                totalBytesLoaded += (ulong)(data.Width * data.Height * 4);
                var stride = data.Stride;
                byte[] pixelData = new Byte[stride];
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    if (scanline < mRegion.Top || scanline >= mRegion.Bottom)
                        continue;

                    System.Runtime.InteropServices.Marshal.Copy(data.Scan0 + (scanline * stride), pixelData, 0, stride);

                    for (int i = 0; i < data.Width * 4; i += 4)
                    {
                        if (mRegion.Contains(new RGPointI(i / 4, scanline)))
                            mPixelData.Add(Color.FromArgb(pixelData[i + 3], pixelData[i + 2], pixelData[i + 1], pixelData[i + 0]));
                    }
                }
            }
            finally
            {
                this.Image.UnlockBits(data);
            }
        }

        public Color GetPixel(int imageX, int imageY)
        {
            return GetPixel(imageX, imageY, false);
        }

        public Color GetPixel(int imageX, int imageY, bool ignoreOutsideRegion)
        {
            if (this.Region.Contains(new RGPointI(imageX, imageY)))
            {
                int x = imageX - Region.X;
                int y = imageY - Region.Y;

                int index = (y * Region.Width) + x;
                if (index >= 0 && index < mPixelData.Count)
                    return mPixelData[index];
                else
                    return this.Image.GetPixel(imageX, imageY);
            }
            else if (ignoreOutsideRegion)
                return Color.Transparent;
            else
                return this.Image.GetPixel(imageX, imageY);
        }

        #endregion

        public BitmapPortion LoadNext()
        {
            try
            {

                string fileNumberStr = mPath.Substring(mPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                fileNumberStr = fileNumberStr.Substring(0, fileNumberStr.LastIndexOf('.'));

                string ext = mPath.Substring(mPath.LastIndexOf('.'));

                string newFilename;
                if (fileNumberStr.Contains('_'))
                {
                    fileNumberStr = fileNumberStr.Substring(fileNumberStr.LastIndexOf('_') + 1);
                    var fileNumber = Int32.Parse(fileNumberStr);
                    newFilename = mPath.Substring(0, mPath.LastIndexOf('_')) + "_" + (fileNumber + 1).ToString("000") + ext;
                }
                else
                {
                    var fileNumber = Int32.Parse(fileNumberStr);
                    newFilename = mPath.Substring(0, mPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1) + (fileNumber + 1).ToString() + ext;

                }


                if (System.IO.File.Exists(newFilename))
                    return new BitmapPortion(newFilename, false);
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public void DeleteFile()
        {
            if (!String.IsNullOrEmpty(this.mPath) && System.IO.File.Exists(this.mPath))
            {
                System.IO.File.Delete(mPath);
            }
        }

        public void MoveTo(string newFolder)
        {
            var newFile = System.IO.Path.Combine(newFolder, System.IO.Path.GetFileName(mPath));
            SavePortion(newFile);

            System.IO.File.Delete(mPath);
            mPath = newFile;
        }

        public Bitmap SavePortion()
        {
            return SavePortion(this.mPath);
        }

        public Bitmap SavePortion(string file)
        {
            var bmp = new Bitmap(Region.Width, Region.Height);
            var g = Graphics.FromImage(bmp);

            g.DrawImage(this.Image, new Rectangle(0, 0, Region.Width, Region.Height), Region.ToSystemRec(), GraphicsUnit.Pixel);
            g.Dispose();
            bmp.Save(file);

            return bmp;
        }

        public void MakeTransparent(Color transparentColor)
        {
            for (int y = 0; y < Image.Height; y++)
                for (int x = 0; x < Image.Width; x++)
                {
                    if (GetPixel(x, y).Equals(transparentColor))
                        this.Image.SetPixel(x, y, Color.Transparent);
                }

            this.RecalcBitmapData();
        }

        public BitmapPortion Extract(RGRectangleI newArea)
        {
            return new BitmapPortion(this, newArea);
        }

        public void PasteInto(Graphics g, RGPointI location)
        {
            g.DrawImage(this.Image, RGRectangleI.Create(location, Region.Size).ToSystemRec(), Region.ToSystemRec(), GraphicsUnit.Pixel);
        }

        public bool PixelsEqual(BitmapPortion other)
        {
            if (this.Region.Width != other.Region.Width || this.Region.Height != other.Region.Height)
                return false;

            var otherData = other.mPixelData;
            if (this.mPixelData.Count != other.mPixelData.Count)
                return false;

            for (int i = 0; i < mPixelData.Count; i++)
                if (!mPixelData[i].PixelEquals(otherData[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;

            for (int y = Region.Y; y < Region.Bottom; y++)
                for (int x = Region.X; x < Region.Right; x++)
                {
                    var px = this.GetPixel(x, y);
                    if (px.A == 0)
                        sum += 777;
                    else
                        sum += px.R + px.G + px.B;
                }

            return sum;
        }

        public Point? GetFirstDifference(BitmapPortion other)
        {
            this.RecalcBitmapData();
            other.RecalcBitmapData();

            var a = this.GetHashCode();
            var b = other.GetHashCode();

            int sum1 = 0, sum2 = 0;

            for (int y = 0; y < Region.Height; y++)
                for (int x = 0; x < Region.Width; x++)
                {
                    var px = this.GetPixel(Region.X + x, Region.Y + y);
                    var otherPX = other.GetPixel(other.Region.X + x, other.Region.Y + y);

                    if (px.A == 0)
                        sum1 += 777;
                    else
                        sum1 += px.R + px.G + px.B;

                    if (otherPX.A == 0)
                        sum2 += 777;
                    else
                        sum2 += otherPX.R + otherPX.G + otherPX.B;

                    if (sum1 != sum2)
                        return new Point(Region.X + x, Region.X + y);

                    if (!px.PixelEquals(otherPX))
                        return new Point(Region.X + x, Region.X + y);
                }

            return null;
        }

        public void ResetRegion()
        {
            Region = RGRectangleI.FromXYWH(0, 0, Image.Width, Image.Height);
        }

        #region Origin

        private RGPointI mOrigin;

        public RGPointI Origin
        {
            get
            {
                return mOrigin;
            }
            set
            {
                mOrigin = value;
            }
        }

        public RGRectangleI PrimaryHitbox { get { return this.Hitbox; } set { this.Hitbox = value; } }
        public RGRectangleI SecondaryHitbox { get; set; }

        public RGRectangleI Hitbox { get; set; }

        #endregion

        #region Tiles

        public BitmapPortion[] ExtractTiles(RGPointI origin, int tileSize)
        {
            return ExtractGrid(origin, tileSize).Distinct(new PixelEqualityComparer()).ToArray();
        }

        private IEnumerable<BitmapPortion> ExtractGrid(RGPointI origin, int gridSize)
        {
            int oX = origin.X;
            int oY = origin.Y;
            while (oX >= gridSize) oX -= gridSize;
            while (oY >= gridSize) oY -= gridSize;

            for (int y = oY; y <= Image.Height - gridSize; y += gridSize)
                for (int x = oX; x <= Image.Width - gridSize; x += gridSize)
                    yield return Extract(RGRectangleI.FromXYWH(x, y, gridSize, gridSize));
        }

        #endregion

        #region Flood Fill

        private bool[,] CreateFloodFillMask(int floodFillSize)
        {
            bool[,] mask = new bool[(Region.Width / floodFillSize), (Region.Height / floodFillSize)];

            for (int y = 0; y < mask.Height(); y++)
                for (int x = 0; x < mask.Width(); x++)
                {
                    bool filled = false;

                    for (int yy = y * floodFillSize; yy < (y + 1) * floodFillSize && !filled; yy++)
                        for (int xx = x * floodFillSize; xx < (x + 1) * floodFillSize && !filled; xx++)
                        {
                            if (!this.GetPixel(Region.X + xx, Region.Y + yy).IsTransparent())
                                filled = true;
                        }

                    mask[x, y] = filled;
                }

            return mask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin">Point relative to the image</param>
        /// <param name="floodFillSize"></param>
        /// <param name="transparentColor"></param>
        /// <returns></returns>
        public bool FloodFillExtract(RGPointI origin, int minResolution)
        {
            var newArea = Region;

            if (Region.Width > 128 || Region.Height > 128)
            {
                newArea = FloodFillGetArea(origin.RelativeTo(Region.TopLeft), 8);
                if (newArea.IsEmpty)
                    return false;
                Region = newArea;
            }

            if (Region.Width > 64 || Region.Height > 64)
            {
                newArea = FloodFillGetArea(origin.RelativeTo(Region.TopLeft), 4);
                if (newArea.IsEmpty)
                    return false;
                Region = newArea;
            }

            newArea = FloodFillGetArea(origin.RelativeTo(Region.TopLeft), minResolution);
            if (newArea.IsEmpty)
                return false;

            Region = newArea;
            return true;
        }

        private RGRectangleI FloodFillGetArea(RGPointI origin, int floodFillSize)
        {
            var mask = CreateFloodFillMask(floodFillSize);
            var maskPoints = FloodFillGetPixels(origin.Scale(1f / (float)floodFillSize), mask, floodFillSize);

            if (maskPoints.Count == 0)
                return RGRectangleI.Empty;

            var newArea = RGRectangleI.FromTLBR(maskPoints.Min(p => p.Y) * floodFillSize, maskPoints.Min(p => p.X) * floodFillSize,
                (maskPoints.Max(p => p.Y) * floodFillSize) + 1, (maskPoints.Max(p => p.X) * floodFillSize) + 1);

            newArea.X += Region.X;
            newArea.Y += Region.Y;
            newArea.Width += Region.X;
            newArea.Height += Region.Y;

            if (floodFillSize > 1)
            {
                newArea.Left -= floodFillSize;
                newArea.Top -= floodFillSize;
                newArea.Right += floodFillSize * 2;
                newArea.Bottom += floodFillSize * 2;
            }

            return newArea;
        }

        private List<RGPointI> FloodFillGetPixels(RGPointI origin, bool[,] mask, int floodFillSize)
        {
            try
            {
                if (!mask[origin.X, origin.Y])
                    return new List<RGPointI>();
            }
            catch
            {
                return new List<RGPointI>();
            }

            int w, e;
            List<RGPointI> points = new List<RGPointI>();

            List<RGPointI> queue = new List<RGPointI>();
            int qindex = 0;
            queue.Add(origin);


            while (qindex < queue.Count)
            {
                var pt = queue[qindex];
                if (mask[pt.X, pt.Y])
                {
                    w = pt.X;
                    e = pt.X;
                    while (w > 0 && mask[w - 1, pt.Y]) { w--; }
                    while (e < (mask.Width() - 1) && mask[e + 1, pt.Y]) { e++; }

                    for (int i = w; i <= e; i++)
                    {
                        points.AddDistinct(new RGPointI(i, pt.Y));

                        if (pt.Y > 0 && mask[pt.X, pt.Y - 1])
                            queue.AddDistinct(new RGPointI(pt.X, pt.Y - 1));

                        if (pt.Y < mask.Height() - 1 && mask[pt.X, pt.Y + 1])
                            queue.AddDistinct(new RGPointI(i, pt.Y + 1));
                    }
                }

                qindex++;
            }

            return points;
        }



        #endregion

        public void Crop(RGRectangleI rec)
        {
            Region = rec;
        }

        public BitmapPortion CropToNew(RGRectangleI rec)
        {
            var copy = new BitmapPortion(this.Image);
            copy.Crop(rec);
            return copy;
        }

        public static SpriteSheet CreateSpriteSheet(IEnumerable<BitmapPortion> images, int width, string name, Color transparentColor)
        {

            int x = 0; int y = 0;
            int currentRowHeight = 0;

            var locations = images.Select(p =>
            {
                if (x + p.Region.Width >= width)
                {
                    x = 0;
                    y += currentRowHeight;
                    currentRowHeight = 0;
                }

                var imageWithLocation = new { X = x, Y = y, Image = p };
                x += p.Region.Width;
                currentRowHeight = Math.Max(currentRowHeight, p.Region.Height);
                return imageWithLocation;

            }).ToArray();


            var bmp = new Bitmap(locations.Max(p => p.X + p.Image.Region.Width), locations.Max(p => p.Y + p.Image.Region.Height));
            var g = Graphics.FromImage(bmp);
            foreach (var img in locations)
                img.Image.PasteInto(g, new RGPointI(img.X, img.Y));

            bmp.MakeTransparent(transparentColor);
            g.Dispose();

            var sheet = new SpriteSheet(new EditorTextureResource(bmp, "SpriteSheets_" + name));
            foreach (var loc in locations)
            {
                sheet.AddFrame(RGRectangleI.FromXYWH(loc.X, loc.Y, loc.Image.Region.Width, loc.Image.Region.Height), new RGPointI(loc.Image.Origin.X, loc.Image.Origin.Y))
                    .CopyHitboxesFrom(loc.Image);
            }

            return sheet;
        }

        public BitmapPortion Clone()
        {
            var clone = new BitmapPortion(this, this.mRegion);
            return clone;
        }

        public ImagePalette GetPallete()
        {
            return new ImagePalette(mPixelData);
        }

        public byte[,] ToColorMap()
        {
            var palette = this.GetPallete();
            var output = new byte[mRegion.Width, mRegion.Height];
            for (int x = mRegion.Left; x < mRegion.Right; x++)
            {
                for (int y = mRegion.Top; y < mRegion.Bottom; y++)
                {
                    output[x - mRegion.Left, y - mRegion.Top] = palette.IndexOf(this.GetPixel(x, y));
                }
            }

            return output;
        }

        #region Testing

        public void OutputBitmapFromPixelData()
        {
            var bmp = new Bitmap(Region.Width, Region.Height);

            for (int y = 0; y < Region.Height; y++)
                for (int x = 0; x < Region.Width; x++)
                    bmp.SetPixel(x, y, this.GetPixel(Region.X + x, Region.Y + y));


            bmp.Save(Paths.TestPath + "/test.bmp");
        }

        #endregion

    }

    class PixelEqualityComparer : IEqualityComparer<BitmapPortion>
    {

        public bool Equals(BitmapPortion x, BitmapPortion y)
        {
            return x.PixelsEqual(y);
        }

        public int GetHashCode(BitmapPortion obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class GenericEqualityComparerUtil
    {
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> list, Func<T, T, bool> fnEquals, Func<T, int> fnGetHashCode)
        {
            var comparer = new GenericEqualityComparer<T>(fnEquals, fnGetHashCode);
            return list.Distinct(comparer).ToArray();
        }
    }

    class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> mEquals;
        private Func<T, int> mGetHashCode;

        public GenericEqualityComparer(Func<T, T, bool> fnEquals, Func<T, int> fnGetHashCode)
        {
            mEquals = fnEquals;
            mGetHashCode = fnGetHashCode;
        }

        public bool Equals(T x, T y)
        {
            return mEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return mGetHashCode(obj);
        }
    }
}
