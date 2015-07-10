using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.XNA
{
    class XNATextureCache : TextureCache<Texture2D, GraphicsDevice>
    {
        private static XNATextureCache mInstance;
        public static XNATextureCache Instance
        {
            get
            {
                return mInstance ?? (mInstance = new XNATextureCache());
            }
        }

        protected override Texture2D CreateTexture(TextureResource resource, GraphicsDevice device)
        {
            var fs = resource.OpenFileStream();
            var texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(device, fs);
            fs.Close();
            return texture;
        }
    }

    class SpriteBatchPainter : Painter
    {
        private SpriteBatch mSpriteBatch;
        private GraphicsDevice mDevice;
        private SpriteEffects mEffects = SpriteEffects.None;

        public SpriteBatchPainter(GameContext ctx, SpriteBatch spriteBatch, GraphicsDevice device)
            : base(ctx)
        {
            mSpriteBatch = spriteBatch;
            mDevice = device;
        }

        protected override void PaintToScreen(TextureResource textureResource, RGRectangleI source, RGRectangleI dest, RenderOptions options, StackedRenderInfo extraRenderInfo)
        {
            var texture = XNATextureCache.Instance.GetTexture(textureResource, mDevice);

            mEffects = SpriteEffects.None;
            if (options.FlipX)
                mEffects = mEffects | SpriteEffects.FlipHorizontally;
            if (options.FlipY)
                mEffects = mEffects | SpriteEffects.FlipVertically;

            if (options.TextureBreakup != null)
            {
                foreach (var cell in options.TextureBreakup.GetVisibleCells(source, dest))
                    mSpriteBatch.Draw(texture, cell.Dest.ToXNARec(), cell.Src.ToXNARec(), extraRenderInfo.FadeColor.ToXNAColor(), 0, Vector2.Zero, mEffects, 0);
            }
            else if (extraRenderInfo.TextureBreakup != null)
            {
                foreach (var cell in extraRenderInfo.TextureBreakup.GetVisibleCells(source, dest))
                    mSpriteBatch.Draw(texture, cell.Dest.ToXNARec(), cell.Src.ToXNARec(), extraRenderInfo.FadeColor.ToXNAColor(), 0, Vector2.Zero, mEffects, 0);
            }
            else
                mSpriteBatch.Draw(texture, dest.ToXNARec(), source.ToXNARec(), extraRenderInfo.FadeColor.ToXNAColor(), 0, Vector2.Zero, mEffects, 0);
        }
       
    }

    static class XNAUtil
    {
        public static Rectangle ToXNARec(this RGRectangle rec)
        {
            int x = (int)Math.Floor(rec.X);
            int y = (int)Math.Floor(rec.Y);
            int w = (int)Math.Floor(rec.Width);
            int h = (int)Math.Floor(rec.Height);

            return new Rectangle(x, y, w, h);
        }


        public static Rectangle ToXNARec(this RGRectangleI rec)
        {
            return new Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
        }

        public static Color ToXNAColor(this RGColor c)
        {
            return new Color(c.Red, c.Green, c.Blue);
        }
    }

}
