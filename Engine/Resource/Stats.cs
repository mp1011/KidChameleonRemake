using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public class StatsResource<T> : GameResource<T> where T : Stats, new() 
    {

        public StatsResource(string name)  : base(new GamePath(PathType.Info,name)) {
        }

    }

    public abstract class Stats
    {

    }

    
}
