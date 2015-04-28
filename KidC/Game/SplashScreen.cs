using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class SplashScreenInfo : KCWorldInfo 
    {

        private GameResource<WorldInfo> mNextMap;
       
        public SplashScreenInfo(GameResource<WorldInfo> nextMap)
        {
            mNextMap = nextMap;
            this.Theme = LevelTheme.Woods;
        }
        
        public override World CreateWorld(GameContext ctx)
        {
            var nextLevel = mNextMap.GetObject(ctx);
        
            var screen = new World(ctx,this);
            screen.BackgroundColor = RGColor.Black;
            screen.AddLayer(new ImageLayer(screen, new SimpleGraphic(new TextureResource("Splash_" + this.Theme.ToString()), RGRectangleI.FromXYWH(0, 0, 320, 224))));

            GameText levelText = new GameText(screen, FontManager.SplashScreenFont, nextLevel.Name, new RGPointI(50, 50));
            levelText.MaxTextWidth = 180;
         
            screen.ScreenLayer.AddObject(levelText);

            MovingText.MoveInFromAllSides(levelText, screen.ScreenLayer);
            ctx.CenterCamera();

            KidCGame.CreatePlayer(ctx);

             var levelTransition = new FadeoutSceneTransition(screen);

            var buttonListener = new ButtonListener<WorldInfo>(screen, levelTransition);
            buttonListener.AddMappings(nextLevel, KCButton.Pause, KCButton.Jump, KCButton.Run, KCButton.Special);

            return screen;
        }

    }
}
