using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Core
{

    public abstract class GameBase
    {
        public static GameBase Current { get; private set; }

     
        protected GameBase()
        {
            GameBase.Current = this;
        }

#region Abstract Properties and Methods
      
        public abstract ObjectTypeRelations Relations { get; }
        public abstract Func<TileInstance> TileInstanceCreate { get; }
        public abstract Func<EngineBase, GameBase, GameContext> GameContextCreate { get; }
        public abstract GameResource<World> StartingWorld { get; }
    
#endregion


    }
}
