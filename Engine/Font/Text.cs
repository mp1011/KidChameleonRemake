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
        private List<Letter> letters;
        private Alignment hAlignment, vAlignment;

        public Func<string> Updater;
        public Func<RGColor> ColorUpdater;
        public GameFont Font { get; private set; }

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
                SetText(value.ToUpper());
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

                if (this.GetLetters().Count() > 0)
                {
                    m_TextArea.X = Int16.MaxValue;
                    m_TextArea.Y = Int16.MaxValue;

                    int right = 0, bottom = 0;

                    foreach (var letter in letters)
                    {
                        m_TextArea.X = Math.Min(m_TextArea.X, letter.location.X);
                        m_TextArea.Y = Math.Min(m_TextArea.Y, letter.location.Y);
                        right = Math.Max(right, letter.location.X + Font.CellSize.Width);
                        bottom = Math.Max(bottom, letter.location.Y + Font.CellSize.Height);
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

        public GameText(GameContext context, GameFont font, String message, RGPointI location, int maxWidth, Alignment p_halignment, Alignment p_valignment)
            : base(LogicPriority.World, context)
        {

            MaxTextWidth = maxWidth;
            hAlignment = p_halignment;
            vAlignment = p_valignment;

            this.Location = location;
            this.Font = font;
            SetText(message);
        }

        public GameText(GameContext context, GameFont font, String message, RGPointI location, int maxWidth)
            : this(context, font, message, location, maxWidth, Alignment.Near, Alignment.Near) { }

        public GameText(GameContext context, GameFont font, String message, RGPointI location)
            : this(context, font, message, location, 0, Alignment.Near, Alignment.Near) { }


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
                foreach (var letter in letters)
                {
                    letter.color = BackColor;
                }
            }

        }

        private List<Letter> currentWord = new List<Letter>();
        private void PositionText(bool resetLetters)
        {
            if (this.Font == null)
                return;

            cursor = new RGPointI(Location.X - Font.CellSize.Width, Location.Y);

            if (letters == null)
                letters = new List<Letter>();
            else if (resetLetters)
                letters.Clear();

            currentWord.Clear();

            int letterIndex = 0;
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if (c == '\n')
                {
                    currentWord.Clear();
                    AlignLine(cursor.Y);

                    cursor.X = Location.X - Font.CellSize.Width;
                    cursor.Y += (int)(Font.CellSize.Height * 1.5f);
                }
                else if (c == ' ')
                {
                    currentWord.Clear();
                    if (cursor.X > Location.X - Font.CellSize.Width)
                        cursor.X += Font.CellSize.Width;
                }
                else
                {
                    MoveCursor(currentWord);
                }

                if (c != '\n')
                {
                    Letter l;
                    if (resetLetters)
                        l = new Letter();
                    else
                    {
                        l = letters[letterIndex];
                        letterIndex++;
                    }

                    l.character = c;
                    l.location = new RGPointI(cursor.X, cursor.Y);
                    l.color = TextColor;

                    if (resetLetters)
                        letters.Add(l);


                    if (c != ' ')
                        currentWord.Add(l);
                }

            }
            AlignLine(cursor.Y);
            VerticalAlign();
        }

        private void VerticalAlign()
        {
            if (letters.Count() == 0)
                return;

            if (vAlignment == Alignment.Center)
            {
                int height = letters.Max(p => p.location.Y + this.Font.CellSize.Height) - letters.Min(p => p.location.Y);
                foreach (var letter in letters)
                {
                    letter.location.Y -= (height / 2);
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

            foreach (Letter lineLetter in letters)
            {
                if (lineLetter.location.Y != lineY)
                    continue;

                if (!foundNonSpace && Char.IsWhiteSpace(lineLetter.character))
                    spaceBefore += this.Font.CellSize.Width;
                else
                {
                    foundNonSpace = true;

                    if (Char.IsWhiteSpace(lineLetter.character))
                        spaceAfter += this.Font.CellSize.Width;
                    else
                    {
                        spaceBefore = 0;
                        lineWidth += this.Font.CellSize.Width;
                    }
                }
            }

            if (lineWidth == 0)
                return;

            if (hAlignment == Alignment.Center)
            {
                lineWidth += spaceBefore + spaceAfter;
                int xPos = this.Location.X + ((MaxTextWidth - lineWidth) / 2);
                foreach (var letter in letters)
                {
                    if (letter.location.Y != lineY)
                        continue;

                    letter.location.X = xPos;
                    xPos += this.Font.CellSize.Width;
                }
            }
            else if (hAlignment == Alignment.Far)
            {
                int xPos = this.Location.X + (MaxTextWidth - lineWidth);
                foreach (var letter in letters)
                {
                    if (letter.location.Y != lineY)
                        continue;

                    letter.location.X = xPos;
                    xPos += this.Font.CellSize.Width;
                }
            }
        }

        private void MoveCursor(List<Letter> currentWord)
        {
            cursor.X += this.Font.CellSize.Width;
            if (MaxTextWidth > 0 && cursor.X + this.Font.CellSize.Width > this.Location.X + MaxTextWidth)
            {
                int lineStart = Location.X;
                if (currentWord.Count > 0)
                {
                    int offset = currentWord.First().location.X - Location.X;
                    foreach (var letter in currentWord)
                    {
                        letter.location.Y += (int)(this.Font.CellSize.Height * 1.5f);
                        letter.location.X -= offset;
                    }

                    lineStart = currentWord.Last().location.X + this.Font.CellSize.Width;
                }

                AlignLine(cursor.Y);
                cursor.X = lineStart;
                cursor.Y += (int)(this.Font.CellSize.Height * 1.5f);
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

        public List<Letter> GetLetters() { return letters; }

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
                foreach (var letter in letters)
                {
                    letterFade = fadePct - fadeOffset;
                    if (letterFade > 1)
                        letterFade = 1;
                    if (letterFade < 0)
                        letterFade = 0;

                    if (letterFade < 1)
                        done = false;

                    letter.color = TextColor; // Utility.FadeColor(BackColor, TextColor, letterFade);

                    fadeOffset += .1f;
                }

                if (done && letters.Count > 0)
                    Fade = LetterFade.None;
            }
            else if (Fade == LetterFade.FadeOut)
            {
                fadePct += .05f;
                if (fadePct > 1)
                {
                    fadePct = 1;
                }

                foreach (var letter in letters)
                {
                    letter.color = BackColor; // Utility.FadeColor(TextColor, BackColor, fadePct);
                }
            }
            else
            {
                foreach (var letter in letters)
                {
                    letter.color = TextColor;
                }
            }

            this.DrawX(painter, canvas);
        }

        private void DrawX(Painter painter, RGRectangleI canvas)
        {
            RGRectangleI dest;

            foreach (var letter in this.GetLetters())
            {
                dest = RGRectangleI.FromXYWH(letter.location.X, letter.location.Y, this.Font.CellSize.Width, this.Font.CellSize.Height);

                var letterColor = letter.color;
                //if (letter.color == RGColor.White)
                //    letterColor = filterColor;
                //else if (filterColor != Color.White)
                //{
                //    float p = (float)filterColor.R / 255f;
                //    letterColor = Utility.FadeColor(letterColor, Color.Black, 1 - p);
                //}
                
                painter.Paint(canvas, this.Font.FontTexture, this.Font.GetLetterTextureLocation(letter.character), dest, RenderOptions.Normal);
            }

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
