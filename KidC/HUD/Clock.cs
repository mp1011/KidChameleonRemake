using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class Clock : LogicObject, IDrawableRemovable, IWithPosition 
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

        public void AdjustSeconds(int change)
        {
            mStartSeconds += change;
        }

        public bool TimesUp
        {
            get
            {
                if (mClockStart == 0)
                    return false;

                return this.SecondsRemaining == 0;
            }
        }

        public int SecondsRemaining
        {
            get
            {
                var secondsElapsed = Context.ElapsedSecondsSince(mClockStart);           
                return Math.Max(0, mStartSeconds - secondsElapsed);
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


        public RGRectangleI Area
        {
            get { return mLabel.Area; }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }
    }
}
