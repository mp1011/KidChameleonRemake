using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public class MovingText : ILogicObject, IDrawableRemovable
    {
        private string mName;
        private ActiveLetter[] mLetters;
        private GameText mText;
     
        private MovingText(GameText text, Layer layer)
        {
            mText = text;
            mText.Visible = false;
            mLetters = mText.Letters.Select(p => new ActiveLetter(mText, p)).ToArray();
            this.Context = text.Context;
            layer.AddObject(this);
        }
       
        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            if (mLetters == null || this.Paused)
                return;

            foreach (var letter in mLetters)
                letter.Draw(p, canvas);
        }

        #region Public

        public static MovingText MoveIn(GameText text, Layer layer, Direction fromDirection, int offset)
        {
            var movingText = new MovingText(text, layer);
            movingText.mName = "Move in: \"" + text.Text + "\"";
            movingText.mMakeTextReappearOnFinish = true;
            foreach (var letter in movingText.mLetters)
            {
                letter.Location = letter.OriginalLocation.TopLeft.Offset(fromDirection, offset);
                var seek = new SeekPointController(movingText, letter, new WorldPoint(movingText.Context, letter.OriginalLocation.X, letter.OriginalLocation.Y),5.0f);
                letter.SetMotionBehavior(seek);
            }

            return movingText;
        }

        public static MovingText MoveOut(GameText text, Layer layer, Direction direction)
        {
            var movingText = new MovingText(text,layer);
            
            movingText.mName = "Move out \"" + text.Text + "\"";

            foreach (var letter in movingText.mLetters)
            {
                letter.Location = letter.OriginalLocation.TopLeft;            
                var seekPoint = letter.OriginalLocation.TopLeft.Offset(direction, 200);
                var seek = new SeekPointController(movingText,letter, new WorldPoint(movingText.Context, seekPoint.X, seekPoint.Y),1f);
                letter.SetMotionBehavior(seek);            
            }

            return movingText;
        }


        public static MovingText MoveInFromAllSides(GameText text, Layer layer)
        {
            var movingText = new MovingText(text, layer);
            movingText.mName = "Move in: \"" + text.Text + "\"";
            movingText.mMakeTextReappearOnFinish = true;
            foreach (var letter in movingText.mLetters)
            {
                letter.Location = letter.OriginalLocation.TopLeft.Offset(Direction.Random(), 200);
                var seek = new SeekPointController(movingText, letter, new WorldPoint(movingText.Context, letter.OriginalLocation.X, letter.OriginalLocation.Y), 5.0f);
                letter.SetMotionBehavior(seek);
            }

            return movingText;
        }


        #endregion

        private bool mMakeTextReappearOnFinish = false;

        private bool mWasAlive = true;
        public bool Alive
        {
            get
            {
                foreach (var letter in mLetters)
                    if (!letter.Finished)
                        return true;

                if (mWasAlive)
                {
                    mWasAlive = false;
                    if(mMakeTextReappearOnFinish)
                        mText.Visible = true;
                }
                return false;
            }
        }

        private bool mPaused;
        public bool Paused
        {
            get { return mPaused; }
            set
            {
                if (mPaused && !value)
                    mText.Visible = false;

                mPaused = value;

            }
        }

        public ExitCode ExitCode
        {
            get { return this.Alive ? ExitCode.StillAlive : ExitCode.Finished; }
        }

        public void Kill(ExitCode exitCode)
        {
            throw new NotImplementedException();
        }

        public GameContext Context
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return mName;
        }
    }

    
    class ActiveLetter : IMoveableWithPosition 
    {        
        private Letter mLetter;
        private GameText mText;       
        public RGRectangleI OriginalLocation { get { return mLetter.Location; } }
        private ILogicObject mMotionBehavior;

        public ActiveLetter(GameText text, Letter letter) 
        {
            mText = text;
            mLetter = letter;          
            this.MotionManager = ObjectMotion.Create(text, this);         
        }


        public void SetMotionBehavior(ILogicObject motionBehavior)
        {
            mMotionBehavior = motionBehavior;
        }

        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            mText.DrawLetter(mLetter.Character, this.Area, p, canvas);
        }

        public bool Finished { get { return mMotionBehavior.ExitCode == ExitCode.Finished; } }

        public RGPointI Location
        {
            get;
            set;
        }

        public RGRectangleI Area
        {
            get { return RGRectangleI.Create(Location, mText.Font.LetterSize(mLetter.Character)); }
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


        public GameContext Context
        {
            get { return mText.Context; }
        }
    }
}
