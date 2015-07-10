using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class SimpleAnimation : LogicObject, IDrawableRemovable 
    {

        private SimpleGraphic mGraphic;
        private int[] mFrames;
        private int mFrameDuration;
        private int mCurrentFrame;
        private ulong mLastFrameChangeTime;

        public bool Finished { get; private set; }

        public bool Looping { get; set; }
        public RGPointI Location
        {
            get { return mGraphic.Position; }
            set
            {
                mGraphic.Position = value;
            }
        }

        public RGPointI CornerPosition { get { return mGraphic.CornerPosition; } set { mGraphic.CornerPosition = value; } }


        public SimpleAnimation(string textureName, int frameDuration, GameContext ctx, params int[] frames)
            : this(new SimpleGraphic(SpriteSheet.Load(textureName, ctx), frames), frameDuration, ctx, frames)
        {           
        }

        public SimpleAnimation(SimpleGraphic graphic, int frameDuration, GameContext ctx, params int[] frames)
            : base(LogicPriority.World, ctx.CurrentWorld)
        {
            mFrameDuration = frameDuration;
            mFrames = frames;
            mGraphic = graphic;
            this.Looping = true;
        }

        protected override void OnEntrance()
        {
            mLastFrameChangeTime = Context.CurrentFrameNumber;

          
        }

        protected override void Update()
        {
            if (Context.ElapsedFramesSince(mLastFrameChangeTime) >= mFrameDuration)
            {
                mLastFrameChangeTime = Context.CurrentFrameNumber;
                mCurrentFrame++;

                if (mCurrentFrame >= mFrames.Length)
                {
                    if (Looping)
                        mCurrentFrame = 0;
                    else
                    {
                        Finished = true;
                        mCurrentFrame = mFrames.Length - 1;
                    }
                }
            }

            mGraphic.SourceIndex = mFrames[mCurrentFrame];
        }


        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            mGraphic.Draw(p, canvas);
        }

        public void ResetAnimation()
        {
            Finished = false;
            mCurrentFrame = 0;
            mLastFrameChangeTime = Context.CurrentFrameNumber;
        }
    }
}
