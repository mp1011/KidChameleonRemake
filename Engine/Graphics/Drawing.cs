using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Graphics;

namespace Engine
{
    public interface IDrawable
    {
        void Draw(Painter p, RGRectangle canvas);
    }

    public interface IHasTextureInfo
    {
        TextureResource Texture { get; }
        RGRectangle DestinationRec { get;  }
        RenderOptions RenderOptions { get; }
        RGRectangleI SourceRec { get; }
    }

    public static class IHasTextureInfoUtil
    {
        public static void DrawTexture(this IHasTextureInfo i, Painter p, RGRectangle canvas)
        {
            p.Paint(canvas, i.Texture, i.SourceRec, i.DestinationRec, i.RenderOptions);  
        }
    }

    public interface IDrawableRemovable : IDrawable
    {
        bool Alive { get; }
    }

    public class RenderOptions
    {
        public static RenderOptions Normal { get { return new RenderOptions(); } }

        public RenderOptions()
        {
            this.Visible = true;
            this.VisibleInEditor = true;
        }

        public float Transparency = 1f;
        public bool FlipX;
        public bool FlipY;
        public bool Visible = true;
        public bool VisibleInEditor = true;
        public bool Flashing { get; private set; }

        private ulong mLastFlashFrame;
        private bool mFlashOn;
        public bool FlashOn
        {
            get
            {
                if (!Flashing)
                    return false;

                return mFlashOn;               
            }
        }

        public void SetFlashOn(ulong currentFrame)
        {
            this.Flashing = true;
            if ((currentFrame - mLastFlashFrame) > 2)
            {
                mLastFlashFrame = currentFrame;
                mFlashOn = !mFlashOn;
            }
        }

        public void SetFlashOff()
        {
            this.Flashing = false;
        }

        public bool CheckVisible(GameContext ctx)
        {
            return this.Visible;// TBD || (ctx.Listeners.EditorListener.EditorOn && this.VisibleInEditor);
        }
    }

}
