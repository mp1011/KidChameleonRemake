using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    class TileEqualityComparer : IEqualityComparer<TileInstance>
    {
        public bool Equals(TileInstance x, TileInstance y)
        {
            return x.TileLocation.Equals(y.TileLocation) && x.TileDef.Equals(y.TileDef);
        }

        public int GetHashCode(TileInstance obj)
        {
            return obj.TileLocation.GetHashCode() + obj.TileDef.TileID;
        }
    }
}
