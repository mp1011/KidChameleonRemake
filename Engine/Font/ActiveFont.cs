using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class ActiveFont : LogicObject, IDrawableRemovable 
    {
        private List<ActiveLetter> mLetters;
        private GameText mText;

        public ActiveFont(GameText text) : base(LogicPriority.World, text.Context)
        {
            mText = text;
        }

        protected override void OnEntrance()
        {
            mText.Visible = false;
            mLetters = new List<ActiveLetter>();
            
            for (int i = 0; i < mText.Text.Length; i++)
            {
                mLetters.Add(new ActiveLetter(mText, i, mText.Area.Center.ToPointI(), 100));
            }
        }

        protected override void OnExit()
        {
            mText.Visible = true;
        }

        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            foreach (var letter in mLetters)
                letter.Draw(p, canvas);
        }
    }

    class ActiveLetter : LogicObject, IMoveableWithPosition 
    {        
        private int mLetterIndex;
        private GameText mText;
        private SeekPointControllerX<ActiveLetter> mMotionController;
        private WorldPoint mTarget;

        public ActiveLetter(GameText text, int letterIndex, RGPointI center, int radius) : base(LogicPriority.World, text.Context)
        {
            mText = text;
            mLetterIndex = letterIndex;
            mTarget = new WorldPoint(this.Context, 0, 0);
            this.Location = center.Offset(Direction.Random(), radius);
            mMotionController = new SeekPointControllerX<ActiveLetter>(this, mTarget);
            this.MotionManager = new ObjectMotion(this.Context, this);
         
        }

        protected override void Update()
        {
            mTarget.Location = mText.Letters[mLetterIndex].Location.TopLeft;
        }

        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            var letter = mText.Text[mLetterIndex];
            mText.DrawLetter(letter, this.Area, p, canvas);
        }


        public RGPointI Location
        {
            get;
            set;
        }

        public RGRectangleI Area
        {
            get { return RGRectangleI.Create(Location, mText.Font.LetterSize(mText.Text[mLetterIndex])); }
        }

        public Direction Direction
        {
            get { return this.MotionManager.Direction; }
        }

        public ObjectMotion MotionManager
        {
            get;
            private set;
        }

        public void Move(RGPointI offset)
        {
            this.Location = this.Location.Offset(offset);
        }
    }
}
