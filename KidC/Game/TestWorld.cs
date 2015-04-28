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

            var font = FontManager.GetBigFont(context);

            var continuesText = new GameText(w, font, "c", new RGPointI(0, 100), context.ScreenLocation.Width, Alignment.Center, Alignment.Near);
            continuesText.Visible = false;
            w.ScreenLayer.AddObject(continuesText);

            var action = MovingText.MoveIn(continuesText, w.ScreenLayer, Direction.Left);

            action.ContinueWith(MovingText.MoveOut(continuesText, w.ScreenLayer, Direction.Up));

            return w;
        }
    }
}
