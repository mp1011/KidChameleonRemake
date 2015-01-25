using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public class SimpleGraphic : IHasTextureInfo, IDrawable
    {

        public SimpleGraphic(TextureResource texture, params RGRectangleI[] sources)
        {
            Texture = texture;
            mSources = new List<RGRectangleI>(sources);
        }

        public SimpleGraphic(SpriteSheet spriteSheet, params int[] cells)
        {
            this.Texture = spriteSheet.Image;
            mSources = cells.Select(i => spriteSheet.Frames[i].Source).ToList();
        }

        public TextureResource Texture
        {
            get;
            set;
        }

        public RGPoint Position { get; set; }

        public RGPoint CornerPosition
        {
            get
            {
                return Position.Offset(-SourceRec.Width / 2, -SourceRec.Height / 2);
            }
            set
            {
                Position = value.Offset(SourceRec.Width / 2, SourceRec.Height / 2);
            }
        }

        private RGRectangle? mDestination;
        public RGRectangle DestinationRec
        {
            get { return mDestination ?? RGRectangle.Create(CornerPosition, SourceRec.Size.ToSizeF()); }
            set { mDestination = value; }
        }

        private RenderOptions mRenderOptions = RenderOptions.Normal;
        public RenderOptions RenderOptions
        {
            get { return mRenderOptions; }
        }


        public bool Visible { get { return mRenderOptions.Visible; } set { mRenderOptions.Visible = value; } }

        private List<RGRectangleI> mSources;


        public int SourceIndex { get; set; }

        public RGRectangleI SourceRec
        {
            get { return mSources.GetItem(this.SourceIndex); }
        }

        public void Draw(Graphics.Painter p, RGRectangle canvas)
        {
            this.DrawTexture(p, canvas);
        }
    }

}
