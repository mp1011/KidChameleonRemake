using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
namespace KidC
{
    class SceneTransition
    {
        public static void RestartGame(GameContext ctx)
        {
            throw new NotImplementedException();
        }

        public static void RestartRound(GameContext ctx)
        {
            ToSplashScreen(ctx, ctx.CurrentWorld.WorldInfo);
        }

        public static void ToSplashScreen(GameContext ctx, WorldInfo nextMap)
        {
            new FadeoutSceneTransition(ctx.CurrentWorld, new SplashScreenInfo(new InMemoryResource<WorldInfo>(nextMap)));
        }
    }
}
