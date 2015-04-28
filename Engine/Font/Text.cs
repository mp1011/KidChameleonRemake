using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Graphics;

namespace Engine
{
    public class GameText : LogicObject, IDrawableRemovable, IWithPosition
    {

        private int m_maxTextWidth;
        public int MaxTextWidth { get { return m_maxTextWidth == 0 ? (int)this.Context.ScreenLocation.Width : m_maxTextWidth; } set { m_maxTextWidth = value; PositionText(false); } }

        private RGPointI cursor;
        public List<Letter> Letters { get; private set; }

        private Alignment hAlignment, vAlignment;

        public Func<string> Updater;
        public Func<RGColor> ColorUpdater;
        public IGameFont Font { get; private set; }

        private bool mTrimSpaces = true;
        public bool TrimSpaces { get { return mTrimSpaces; } set { mTrimSpaces = value; } }

        private String message;
        public String Text
        {
            get
            {
                if ((message ?? "").Length == 0 && Updater != null)
                {
                    message = Updater();
                    if (ColorUpdater != null)
                        this.TextColor = ColorUpdater();

                    SetText(message);
                }

                return message;
            }
            set
            {
                SetText(value);
            }
        }

        private float fadePct;
        private RGColor? m_textColor, m_backColor;

        public RGColor BackColor { get { if (!m_backColor.HasValue) m_backColor = RGColor.Black; return m_backColor.Value; } set { m_backColor = value; } }
        public RGColor TextColor { get { if (!m_textColor.HasValue) m_textColor = RGColor.White; return m_textColor.Value; } set { m_textColor = value; } }

        private LetterFade m_Fade = LetterFade.None;

        public LetterFade Fade
        {
            get
            {
                return m_Fade;
            }
            set
            {
                if (m_Fade != value)
                {
                    m_Fade = value;
                    fadePct = 0;
                }
            }
        }

        public bool FadeFinished { get { return Fade == LetterFade.FadeOut && fadePct >= 1; } }

        #region Position

        private RGPointI mLocation;
        public RGPointI Location
        {
            get { return mLocation; }
            set
            {
                mLocation = value;
                PositionText(false);
            }
        }
      
        public RGRectangleI Area
        {
            get { return this.TextArea; }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }
        #endregion

        private RGRectangleI m_TextArea = RGRectangleI.Empty;

        public RGRectangleI TextArea
        {
            get
            {

                if (this.Letters.Count() > 0)
                {
                    m_TextArea.X = Int16.MaxValue;
                    m_TextArea.Y = Int16.MaxValue;

                    int right = 0, bottom = 0;

                    foreach (var letter in this.Letters)
                    {
                        m_TextArea.X = Math.Min(m_TextArea.X, letter.Location.X);
                        m_TextArea.Y = Math.Min(m_TextArea.Y, letter.Location.Y);
                        right = Math.Max(right, letter.Location.Right);
                        bottom = Math.Max(bottom, letter.Location.Bottom);
                    }

                    m_TextArea.Width = right - m_TextArea.X;
                    m_TextArea.Height = bottom - m_TextArea.Y;
                }
                else
                {
                    m_TextArea.X = 0;
                    m_TextArea.Y = 0;
                    m_TextArea.Width = 0;
                    m_TextArea.Height = 0;
                }

                return m_TextArea;
            }
        }

        public GameText(ILogicObject owner, IGameFont font, String message, RGPointI location, int maxWidth, Alignment p_halignment, Alignment p_valignment)
            : base(LogicPriority.World, owner)
        {

            MaxTextWidth = maxWidth;
            hAlignment = p_halignment;
            vAlignment = p_valignment;

            this.Location = location;
            this.Font = font;
            SetText(message);
        }

        public GameText(ILogicObject owner, IGameFont font, String message, RGPointI location, int maxWidth)
            : this(owner, font, message, location, maxWidth, Alignment.Near, Alignment.Near) { }

        public GameText(ILogicObject owner, IGameFont font, String message, RGPointI location)
            : this(owner, font, message, location, 0, Alignment.Near, Alignment.Near) { }


        public void Reposition(RGPointI location, int maxWidth)
        {
            MaxTextWidth = maxWidth;
            this.Location = location;
            PositionText(false);
        }

        private void SetText(String p_message)
        {
            if (message == p_message)
                return;

            fadePct = 0;
            message = p_message;

            PositionText(true);

            if (Fade == LetterFade.FadeIn)
            {
                foreach (var letter in this.Letters)
                {
                    letter.Color = BackColor;
                }
            }

        }

        private List<Letter> currentWord = new List<Letter>();
        private void PositionText(bool resetLetters)
        {
            if (this.Font == null)
                return;

            cursor = new RGPointI(Location.X, Location.Y);

            if (this.Letters == null)
                this.Letters = new List<Letter>();
            else if (resetLetters)
                this.Letters.Clear();

            currentWord.Clear();

            int letterIndex = 0;
            for (int i = 0; i < message.Length; i++)
            {           
                char c = message[i];
                if (c == '\n')
                {
                    currentWord.Clear();
                    AlignLine(cursor.Y);

                    cursor.X = Location.X;
                    cursor.Y += (int)(Font.SpaceSize().Height * 1.5f);
                }
                else if (c == ' ')
                {
                    currentWord.Clear();
                    if (cursor.X > Location.X || !TrimSpaces)
                        cursor.X += Font.SpaceSize().Width;
                }               

                if (c != '\n')
                {
                    Letter l;
                    if (resetLetters)
                        l = new Letter();
                    else
                    {
                        l = this.Letters[letterIndex];
                        letterIndex++;
                    }

                    l.Character = c;
                    l.Location = RGRectangleI.FromXYWH(cursor.X, cursor.Y, this.Font.LetterWidth(c), this.Font.LetterHeight(c));
                    l.Color = TextColor;

                    if (resetLetters)
                        this.Letters.Add(l);


                    if (c != ' ')
                        currentWord.Add(l);
                }

                if (c != '\n' && c != ' ')
                    MoveCursor(currentWord, c);

            }
            AlignLine(cursor.Y);
            VerticalAlign();
        }

        private void VerticalAlign()
        {
            if (this.Letters.Count() == 0)
                return;

            if (vAlignment == Alignment.Center)
            {
                int height = this.Letters.Max(p => p.Location.Bottom) - this.Letters.Min(p => p.Location.Y);
                foreach (var letter in this.Letters)
                {
                    var h = letter.Location.Height;
                    letter.Location.Top -= (height / 2);
                    letter.Location.Height = h;
                }
            }
            else if (vAlignment == Alignment.Near)
            {

            }
            else 
                throw new NotImplementedException();
        }

        private void AlignLine(int lineY)
        {
            if (hAlignment == Alignment.Near)
                return;

            bool foundNonSpace = false;
            int lineWidth = 0;
            int spaceBefore = 0, spaceAfter = 0;

            foreach (Letter lineLetter in this.Letters)
            {
                if (lineLetter.Location.Y != lineY)
                    continue;

                if (!foundNonSpace && Char.IsWhiteSpace(lineLetter.Character))
                    spaceBefore += lineLetter.Location.Width;
                else
                {
                    foundNonSpace = true;

                    if (Char.IsWhiteSpace(lineLetter.Character))
                        spaceAfter += lineLetter.Location.Width;
                    else
                    {
                        spaceBefore = 0;
                        lineWidth += lineLetter.Location.Width;
                    }
                }
            }

            if (lineWidth == 0)
                return;

            if (hAlignment == Alignment.Center)
            {
                lineWidth += spaceBefore + spaceAfter;
                int xPos = this.Location.X + ((MaxTextWidth - lineWidth) / 2);
                foreach (var letter in this.Letters)
                {
                    if (letter.Location.Y != lineY)
                        continue;

                    var w = letter.Location.Width;
                    letter.Location.X = xPos;
                    letter.Location.Width = w;
                    xPos += letter.Location.Width;
                }
            }
            else if (hAlignment == Alignment.Far)
            {
                int xPos = this.Location.X + (MaxTextWidth - lineWidth);
                foreach (var letter in this.Letters)
                {
                    if (letter.Location.Y != lineY)
                        continue;

                    var w = letter.Location.Width;
                    letter.Location.X = xPos;
                    letter.Location.Width = w;
                    xPos += letter.Location.Width;
                }
            }
        }

        private void MoveCursor(List<Letter> currentWord, char newLetter)
        {
            cursor.X += this.Font.LetterWidth(newLetter);
            if (MaxTextWidth > 0 && cursor.X + this.Font.LetterSize(' ').Width > this.Location.X + MaxTextWidth)
            {
                int lineStart = Location.X;
                if (currentWord.Count > 0)
                {
                    int offset = currentWord.First().Location.X - Location.X;
                    if (offset > 0)
                    {
                        foreach (var letter in currentWord)
                        {
                            var size = letter.Location.Size;
                            letter.Location.Y += (int)(letter.Location.Height * 1.5f);
                            letter.Location.X -= offset;
                            letter.Location.Width = size.Width;
                            letter.Location.Height = size.Height;
                        }

                        lineStart = currentWord.Last().Location.X + this.Font.LetterSize(' ').Width;

                        AlignLine(cursor.Y);
                        cursor.X = lineStart;
                        cursor.Y += (int)(this.Font.LetterSize(' ').Height * 1.5f);
                    }
                }

            }
        }

     //   private DebugRectangle<GameText> mDebugOutline;

        protected override void OnEntrance()
        {
        //    mDebugOutline = new DebugRectangle<GameText>(this, p => p.Area, DebugRectangle.RecColor.Red);
        }

        protected override void Update()
        {
            if (Updater != null)
                SetText(Updater());

            if (ColorUpdater != null)
                this.TextColor = ColorUpdater();
        }

     
        #region IDrawable Members

        public void Draw(Painter painter, RGRectangleI canvas)
        {
            if (!Visible)
                return;

          //  mDebugOutline.Draw(painter, canvas);

            if (Fade == LetterFade.FadeIn)
            {
                bool done = true;

                fadePct += .05f;
                float letterFade;
                float fadeOffset = 0;
                foreach (var letter in this.Letters)
                {
                    letterFade = fadePct - fadeOffset;
                    if (letterFade > 1)
                        letterFade = 1;
                    if (letterFade < 0)
                        letterFade = 0;

                    if (letterFade < 1)
                        done = false;

                    letter.Color = TextColor; // Utility.FadeColor(BackColor, TextColor, letterFade);

                    fadeOffset += .1f;
                }

                if (done && this.Letters.Count > 0)
                    Fade = LetterFade.None;
            }
            else if (Fade == LetterFade.FadeOut)
            {
                fadePct += .05f;
                if (fadePct > 1)
                {
                    fadePct = 1;
                }

                foreach (var letter in this.Letters)
                {
                    letter.Color = BackColor; // Utility.FadeColor(TextColor, BackColor, fadePct);
                }
            }
            else
            {
                foreach (var letter in this.Letters)
                {
                    letter.Color = TextColor;
                }
            }

            this.DrawX(painter, canvas);
        }

        private void DrawX(Painter painter, RGRectangleI canvas)
        {
            RGRectangleI dest;

            foreach (var letter in this.Letters)
            {               
                //var letterColor = letter.color;
                //if (letter.color == RGColor.White)
                //    letterColor = filterColor;
                //else if (filterColor != Color.White)
                //{
                //    float p = (float)filterColor.R / 255f;
                //    letterColor = Utility.FadeColor(letterColor, Color.Black, 1 - p);
                //}

                DrawLetter(letter.Character, letter.Location, painter, canvas);                
            }

        }

        public void DrawLetter(char character, RGRectangleI location, Painter painter, RGRectangleI canvas)
        {
            painter.Paint(canvas, this.Font.FontTexture, this.Font.GetLetterTextureLocation(character), location, RenderOptions.Normal);
        }

        public bool DrawInFront
        {
            get { return true; }
        }

        private bool mVisible = true;
        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        #endregion



    }
}
