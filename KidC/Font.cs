using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;


namespace KidC
{
    class FontManager
    {        
        public static IGameFont ScoreFont { get; private set; }
        public static IGameFont ClockFont { get; private set; }
        public static IGameFont SplashScreenFont { get; private set; }
        public static IGameFont PauseMenuFont { get; private set; }
        private static FontResource<VariableSpaceFont> BigFont;

        public static void Init()
        {

            var scoreFont = new FixedSpaceFont(new TextureResource("scorefont"), new RGSizeI(8, 8), RGPointI.Empty);
            scoreFont.AddCharacters("9876543210", RGPointI.Empty);
            ScoreFont = scoreFont;

            var clockFont = new FixedSpaceFont(new TextureResource("clockfont"), new RGSizeI(8, 8), RGPointI.Empty);
            clockFont.AddCharacters("012345", new RGPointI(10, 0));
            clockFont.AddCharacters("6789:X ", new RGPointI(0, 1));
            ClockFont = clockFont;

            var splashScreenFont = new VariableSpaceFont(new TextureResource("splashscreenfont"));
            int[] yPos = new int[] { 0,17,34,51,68 };

            int i = 0;
            splashScreenFont.AddRow(yPos[i], yPos[++i], "abcdefg", 0,17,33,51,68,85,101,118);
            splashScreenFont.AddRow(yPos[i], yPos[++i], "hijklmn", 0,17,26,42,60,76,94,112);
            splashScreenFont.AddRow(yPos[i], yPos[++i], "opqrst ", 0,17,31,51,68,84,103,122);
            splashScreenFont.AddRow(yPos[i], yPos[++i], "uvwxyz", 0,17,32,51,68,84,103); 
            SplashScreenFont = splashScreenFont;

            var pauseMenuFont = new FixedSpaceFont(new TextureResource("pausemenufont"), new RGSizeI(8, 8), RGPointI.Empty);
            pauseMenuFont.AddCharacters("dg >uiv", new RGPointI(9,0));
            pauseMenuFont.AddCharacters("serapmontyl", new RGPointI(0,1));
            PauseMenuFont = pauseMenuFont;

            BigFont = new FontResource<VariableSpaceFont>("bigfont2");
        }

        public static IGameFont GetBigFont(GameContext ctx)
        {
            return BigFont.GetObject(ctx);
        }


    }
}
