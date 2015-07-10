using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class TestWorldInfo : WorldInfo
    {
        public const bool UseTestWorld = false;
        
        public override World CreateWorld(GameContext context)
        {
            var w = new World(context, this);

            var screenWidth = (w.ScreenLayer.Location.Width/2)+35;

            var gt= new GameText(w, FontManager.GetBigFontGreen(context), "\"no prize bonus\"", new RGPointI(0, 100), screenWidth, Alignment.Far, Alignment.Near);
            w.ScreenLayer.AddObject(gt);

            return w;
        }
    }
}
