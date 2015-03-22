using System;
using Engine;
using Engine.Core;

namespace KidC
{
    public class KidCGame : GameBase
    {

        public KidCGame()
        {
            KCObjectFactory.Init();
            FontManager.Init();
        }

        private KCObjectRelations mRelations = new KCObjectRelations();
        public override ObjectTypeRelations Relations
        {
            get { return mRelations; }
        }

        public override Func<TileInstance> TileInstanceCreate
        {
            get { return () => new KCTileInstance(); }
        }

        public override Func<EngineBase, GameBase, GameContext> GameContextCreate
        {
            get { return (e, g) => new KCContext(e, g); }
        }

        public override Func<WorldInfo> WorldInfoCreate
        {
            get { return () => new KCWorldInfo(); }
        }

        public override GameResource<WorldInfo> StartingWorld
        {
            get
            {
                var w = new GameResource<WorldInfo>(new GamePath(PathType.Maps, "test"), typeof(KCWorldInfo));
                return new InMemoryResource<WorldInfo>(new SplashScreenInfo(w));              
            }
        }

        public static Player CreatePlayer(GameContext ctx)
        {
            IGameInputDevice input = ctx.Engine.CreateInputDevice(ctx);
            input.ButtonMappings.Add(KCButton.Run, GameKey.Button1);
            input.ButtonMappings.Add(KCButton.Jump, GameKey.Button2);
            input.ButtonMappings.Add(KCButton.Special, GameKey.Button3);
            return new Player(ctx, input);
        }
    }
}

