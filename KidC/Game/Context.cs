using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Core;

namespace KidC
{
    class KCContext : GameContext  
    {

        public PlayerStats Stats { get; private set; }

        public KCContext(EngineBase engine, GameBase game) : base(engine,game)
        {
            this.Stats = new PlayerStats();
            KidCResource.Init(this);
        }
    }
}
