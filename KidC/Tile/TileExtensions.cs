using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    static class TileExtensions
    {

        public static KCTileDef GetSpecialTile(this TileSet tileSet, SpecialTile specialType)
        {
            //todo, inefficient!
            return tileSet.GetTiles().FirstOrDefault(p => (p as KCTileDef).SpecialType == specialType) as KCTileDef;
        }
    }
}
