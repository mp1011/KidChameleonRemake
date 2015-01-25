using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Editor
{
    class EditorTextureResource : Engine.TextureResource 
    {
        private Bitmap mImage;

        public Bitmap GetBitmap()
        {
            return mImage;
        }

        public EditorTextureResource(Bitmap image, string name) : base(name)
        {
            mImage = image;
        }
    }
}
