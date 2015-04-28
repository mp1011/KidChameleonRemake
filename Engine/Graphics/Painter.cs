using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Graphics
{
    public abstract class Painter
    {
        private GameContext mContext;

        private Stack<StackedRenderInfo> mExtraRenderInfo = new Stack<StackedRenderInfo>();

        public RenderOptions RenderInfo { get; set; }

        protected Painter(GameContext ctx)
        {
            mContext = ctx;
        }

        public void Paint(RGRectangleI canvas, IDrawable drawable)
        {
            if (drawable != null)                
                drawable.Draw(this, canvas);
        }

        public void Paint(RGRectangleI canvas, TextureResource texture, RGRectangleI source, RGRectangleI dest, RenderOptions options)
        {
            if (!options.CheckVisible(mContext))
                return;

            if (!dest.CollidesWith(canvas))
                return;

            if (options.FlashOn)
                texture = texture.GetFlashTexture();

            dest = dest.Offset(-canvas.X, -canvas.Y);
            PaintToScreen(texture, source, dest, options, GetCombinedRenderInfo());
        }

        protected abstract void PaintToScreen(TextureResource texture, RGRectangleI source, RGRectangleI dest, RenderOptions options, StackedRenderInfo combinedRenderInfo);


        public void PushRenderInfo(StackedRenderInfo info)
        {
            mExtraRenderInfo.Push(info);
        }

        public void PopRenderInfo()
        {
            mExtraRenderInfo.Pop();
        }

        private StackedRenderInfo GetCombinedRenderInfo()
        {
            var r = new StackedRenderInfo();
            foreach (var ri in mExtraRenderInfo.Reverse())
                r.Add(ri);

            return r;
        }
    }


}
