using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class Clock : LogicObject, IDrawableRemovable 
    {
        private GameText mLabel;
    
        private ulong mClockStart;
        private int mStartSeconds = 120;
        private ulong mFramesElapsed = 0;

        public RGPointI Location { get; set; }

        public Clock(Layer layer)
            : base(LogicPriority.World, layer)
        {
        }

        public bool TimesUp
        {
            get
            {
                if (mClockStart == 0)
                    return false;

                var secondsElapsed = Context.ElapsedSecondsSince(mClockStart);
                var secondsRemaining = Math.Max(0, mStartSeconds - secondsElapsed);
                return secondsRemaining == 0;
            }
        }


        protected override void OnEntrance()
        {
            mLabel = new GameText(this, FontManager.ClockFont, "1:00", this.Location, 32, Alignment.Far, Alignment.Center);
            mClockStart = Context.CurrentFrameNumber;
        }
        
        protected override void Update()
        {
            mLabel.Location = this.Location;
            mFramesElapsed++;

            var secondsElapsed = Context.ElapsedSeconds(mFramesElapsed);

            var secondsRemaining = Math.Max(0, mStartSeconds - secondsElapsed);
            var minutes = secondsRemaining / 60;
            var seconds = secondsRemaining % 60;

            mLabel.Text = minutes + ":" + seconds.ToString("00");
        }


        public void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            if (this.mLabel == null)
                return;

            this.mLabel.Draw(p, canvas);
        }
    }
}
