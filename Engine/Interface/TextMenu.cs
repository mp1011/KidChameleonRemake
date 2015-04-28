using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class TextMenu<TOption>:LogicObject, IDrawable 
    {
        private ITriggerable<TOption> mResponder;
        private List<TextMenuOption<TOption>> mOptions = new List<TextMenuOption<TOption>>();
        private IGameFont mFont;
        private string mSelectionChar = ">";
        private int mSelectedIndex=0;
        private IWithPosition mLocation;

        private TextMenuOption<TOption> SelectedOption
        {
            get
            {
                return mOptions[SelectedIndex];
            }
        }

        public int SelectedIndex
        {
            get
            {
                return mSelectedIndex;
            }
            set
            {
                mSelectedIndex = value;

                if (mSelectedIndex < 0)
                    mSelectedIndex = mOptions.Count - 1;
                if (mSelectedIndex >= mOptions.Count)
                    mSelectedIndex = 0;

                AdjustText();
            }
        }

        private void AdjustText()
        {

            foreach (var text in mOptions)
                text.SetSelected(false, mSelectionChar);

            mOptions[mSelectedIndex].SetSelected(true, mSelectionChar);
        }

        public TextMenu(GameContext context, IGameFont font, ITriggerable<TOption> responder, IWithPosition location)
            : base(LogicPriority.World, context.CurrentWorld)
        {
            mFont=font;
            mResponder = responder;
            mLocation = location;
        }
        
        public void AddOption(string text, TOption arg)
        {
            var opt = mOptions.AddItem(new TextMenuOption<TOption>(this, text, mFont, arg));
            opt.Location = mLocation.Location.Offset(8, 8+ (mOptions.Count-1) * 16);

            AdjustText();
        }


        protected override void Update()
        {
            var input = this.Context.FirstPlayer.Input;

            if (input.KeyPressed(GameKey.Up))
                this.SelectedIndex--;
            else if (input.KeyPressed(GameKey.Down))
                this.SelectedIndex++;
            else if (input.KeyPressed(GameKey.Start))
                mResponder.Trigger(this.SelectedOption.Arg); 
        }

        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            foreach (var option in mOptions)
                option.Draw(p, canvas);
        }
    }

    class TextMenuOption<TOption> : IDrawable 
    {
        private GameText mText;
        public TOption Arg { get; private set; }

        public RGPointI Location
        {
            get
            {
                return mText.Location;
            }
            set
            {
                mText.Location = value;
            }
        }

        public void SetSelected(bool selected, string selectionChar)
        {
            if (selected)
                mText.Text = ">" + mText.Text.Substring(1);
            else
                mText.Text = " " + mText.Text.Substring(1);
        }

        public TextMenuOption(ILogicObject owner, string text, IGameFont font, TOption arg)
        {
            mText = new GameText(owner, font, " " + text, RGPointI.Empty, 1000);
            mText.TrimSpaces = false;
            this.Arg=arg;
        }

        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            mText.Draw(p, canvas);
        }
    }
}
