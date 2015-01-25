using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;


namespace KidC
{
    class FontManager
    {        
        public static GameFont ScoreFont { get; private set; }
        public static GameFont ClockFont { get; private set; }


        public static void Init()
        {

            ScoreFont = new GameFont(new TextureResource("scorefont"), new RGSizeI(8, 8), RGPointI.Empty);
            ScoreFont.AddCharacters("9876543210", RGPointI.Empty);

            ClockFont = new GameFont(new TextureResource("clockfont"), new RGSizeI(8, 8), RGPointI.Empty);
            ClockFont.AddCharacters("012345", new RGPointI(10, 0));
            ClockFont.AddCharacters("6789:X ", new RGPointI(0, 1));

        }

    }
}
