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

        public override Func<EngineBase,GameBase,GameContext> GameContextCreate
        {
            get { return (e,g) => new KCContext(e,g); }
        }


        public override GameResource<World> StartingWorld
        {
            get { return new TestWorld(); }
        }
    }
}

