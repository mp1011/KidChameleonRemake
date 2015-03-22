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
        public string Title { get; set; }

        public SplashScreenInfo(GameResource<WorldInfo> nextMap)
        {
            mNextMap = nextMap;
            this.Title = "this is my level";
            this.Theme = LevelTheme.Woods;
        }
        
        public override World CreateWorld(GameContext ctx)
        {
            var screen = new World(ctx);
            screen.BackgroundColor = RGColor.Black;
            screen.AddLayer(new ImageLayer(ctx, new SimpleGraphic(new TextureResource("Splash_" + this.Theme.ToString()), RGRectangleI.FromXYWH(0, 0, 320, 224))));

            GameText levelText = new GameText(ctx, FontManager.SplashScreenFont, this.Title, new RGPointI(50, 50));
            levelText.MaxTextWidth = 180;
         
            screen.ScreenLayer.AddObject(levelText);

            screen.ScreenLayer.AddObject(new ActiveFont(levelText));
            ctx.CenterCamera();

            KidCGame.CreatePlayer(ctx);

            var nextLevel = mNextMap.GetObject(ctx);
            var levelTransition = new FadeoutSceneTransition(ctx);

            var buttonListener = new ButtonListener<WorldInfo>(ctx, levelTransition);
            buttonListener.AddMappings(nextLevel, KCButton.Jump, KCButton.Run, KCButton.Special);

            return screen;
        }

    }
}
