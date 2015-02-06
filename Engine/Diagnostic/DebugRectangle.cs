using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public static class DebugRectangle
    {
        private static RenderOptions mRenderInfo;
        
        public enum RecColor
        {
            Green,
            Orange,
            Blue,
            Red
        }

        public static RenderOptions RenderOptions
        {
            get
            {
                if (mRenderInfo == null)
                    mRenderInfo = new RenderOptions { Visible = true, Transparency = .5f };

                return mRenderInfo;
            }
        }

    }

    public class DebugRectangle<T> : IDrawableRemovable, IHasTextureInfo where T:LogicObject 
    {
       

        private Func<T,RGRectangleI> mGetLocation;
        private T mObject;

        public DebugRectangle(T obj, Func<T, RGRectangleI> getLocation, DebugRectangle.RecColor color)
        {
            mGetLocation = getLocation;
            mObject = obj;
            this.Texture = new TextureResource("debugcolors");

            switch (color)
            {
                case DebugRectangle.RecColor.Green: this.SourceRec = RGRectangleI.FromXYWH(0, 0, 16, 16); break;
                case DebugRectangle.RecColor.Orange: this.SourceRec = RGRectangleI.FromXYWH(16, 0, 16, 16); break;
                case DebugRectangle.RecColor.Blue: this.SourceRec = RGRectangleI.FromXYWH(0, 16, 16, 16); break;
                case DebugRectangle.RecColor.Red: this.SourceRec = RGRectangleI.FromXYWH(16, 16, 16, 16); break;
            }
        }

        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            this.DrawTexture(p, canvas);
        }

        public TextureResource Texture
        {
            get;
            private set;
        }

        public RGRectangleI DestinationRec
        {
            get { return mGetLocation(mObject); }
        }

        public RenderOptions RenderOptions
        {
            get { return DebugRectangle.RenderOptions; }
        }

        public RGRectangleI SourceRec
        {
            get;
            private set;
        }

        public bool Alive
        {
            get { return mObject.Alive; }
        }
    }
}
