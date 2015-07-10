using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Graphics;

namespace Engine
{
    public interface IDrawable
    {
        void Draw(Painter p, RGRectangleI canvas);
    }

    public interface IHasTextureInfo
    {
        TextureResource Texture { get; }
        RGRectangleI DestinationRec { get;  }
        RenderOptions RenderOptions { get; }
        RGRectangleI SourceRec { get; }
    }

    public static class IHasTextureInfoUtil
    {
        public static void DrawTexture(this IHasTextureInfo i, Painter p, RGRectangleI canvas)
        {
            p.Paint(canvas, i.Texture, i.SourceRec, i.DestinationRec, i.RenderOptions);  
        }
    }

    public interface IDrawableRemovable : IDrawable
    {
        bool Alive { get; }
    }


    public class StackedRenderInfo
    {
        public RGColor FadeColor { get; set; }
        public ITextureBreakup TextureBreakup { get; set; }

        public StackedRenderInfo()
        {
            this.FadeColor = RGColor.White;
        }

        public void Add(StackedRenderInfo other)
        {
            if (other == null)
                return;

            if (other.TextureBreakup != null)
                this.TextureBreakup = other.TextureBreakup;

            if(other.FadeColor.Equals(RGColor.White))
                return;

            this.FadeColor = other.FadeColor;

          
        }
    }

    public class RenderOptions
    {
        public static RenderOptions Normal { get { return new RenderOptions(); } }

        public RenderOptions()
        {
            this.Visible = true;
            this.VisibleInEditor = true;
          //  this.TextureBreakup = new GridTextureBreakup();
        }

        public float Transparency = 1f;
        public bool FlipX;
        public bool FlipY;
        public bool Visible = true;
        public bool VisibleInEditor = true;
        public bool Flashing { get; private set; }

        public ITextureBreakup TextureBreakup { get;set;}

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
