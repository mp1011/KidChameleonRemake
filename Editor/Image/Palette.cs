using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Editor
{
    class ImagePaletteCollection 
    {
        private ImagePalette[] mPalettes;
      
        public IEnumerable<ImagePalette> Palettes { get{ return mPalettes.AsEnumerable();}}

        public ImagePaletteCollection(IEnumerable<BitmapPortion> images)
        {
            mPalettes = images.Select(p => p.GetPallete()).Distinct().ToArray();
            var numColors = mPalettes.Max(p => p.ColorCount);
            mPalettes = mPalettes.Where(p => p.ColorCount == numColors).ToArray();
        }

        //public BitmapPortion GetImageInPalette(IEnumerable<BitmapPortion> images, ImagePalette targetPalette)
        //{
        //    var imageInPalette = images.FirstOrDefault(p => p.GetPallete().Equals(targetPalette));
        //    if (imageInPalette != null)
        //        return imageInPalette;

        //    var newImage = new Bitmap(images.First().Region.Width, images.First().Region.Height);            
        //    for(int y = 0; y<newImage.Height;y++)
        //        for (int x = 0; x < newImage.Width; x++)
        //            newImage.SetPixel(x, y, targetPalette.GetColor(mColorIndexes[x, y]));

        //    return new BitmapPortion(newImage);
        //}
    }



    class ImagePalette : IEquatable<ImagePalette>
    {
        private Color[] mColors;

        public int ColorCount { get { return mColors.Length; } }

        public ImagePalette(IEnumerable<Color> colors)
        {
            mColors = colors.OrderBy(p => (int)p.R + p.G + p.B + p.A).Distinct().ToArray();
            mColors = new Color[] { Color.Transparent }.Union(mColors).ToArray();
        }

        public override int GetHashCode()
        {
            return mColors.Select(p => p.GetHashCode()).Sum();
        }
     
        public bool Equals(ImagePalette other)
        {
            if (this.GetHashCode() != other.GetHashCode())
                return false;

            if (this.mColors.Length != other.mColors.Length)
                return false;

            for (int x = 0; x < mColors.Length; x++)
                if (!mColors[x].Equals(other.mColors[x]))
                    return false;

            return true;
        }

        public Color GetColor(byte index)
        {
            return mColors[index];
        }

        public byte IndexOf(Color c)
        {
            for (byte i = 0; i < mColors.Length; i++)
                if (mColors[i].Equals(c))
                    return i;

            return 0;
        }
    }
  
}
