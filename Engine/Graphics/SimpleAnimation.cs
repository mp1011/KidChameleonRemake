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

        public RGPointI Location
        {
            get { return mGraphic.Position; }
            set
            {
                mGraphic.Position = value;
            }
        }

        public SimpleAnimation(SimpleGraphic graphic, int frameDuration, GameContext ctx, params int[] frames) : base(LogicPriority.World,ctx)
        {
            mFrameDuration = frameDuration;
            mFrames = frames;
            mGraphic = graphic;
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
                    mCurrentFrame = 0;
            }

            mGraphic.SourceIndex = mFrames[mCurrentFrame];
        }


        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            mGraphic.Draw(p, canvas);
        }
    }
}
