using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class TestWorldInfo : WorldInfo
    {
        public const bool UseTestWorld = false;
        
        public override World CreateWorld(GameContext context)
        {
            KidCGraphic.Load(context);

            var w = new World(context, this);
            context.SetWorld(w);
            var screenWidth = (w.ScreenLayer.Location.Width/2)+35;
         

            var csv = new CSVResource<TransformationStats>("Transformations.csv");
            var dr = new DevelopmentResource<TransformationStats[]>(csv);

            var rows = csv.GetObject(context);

           
            return w;
        }
    }

}
