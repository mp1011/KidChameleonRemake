using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    static class KidCObject
    {
        public static MovingObjectCreator IceFragment { get; set; }

        public static void Load(GameContext context)
        {
            var graphicsResource = new CSVResource<MovingObjectCreator>("Objects.csv");

            foreach (var creator in graphicsResource.GetObject(context))
            {
                var property = typeof(KidCObject).GetProperty(creator.Name);
                property.SetValue(null, creator, null);
            }
        }

    }
}
