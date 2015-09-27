using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    public static class KidCGraphic
    {
        public static GraphicCreator MetalBlockShine { get; set; }
        public static GraphicCreator RubberBlockBounce { get; set; }
        public static GraphicCreator VRBackground1 { get; set; }
        public static GraphicCreator VRBackground2 { get; set; }
        public static GraphicCreator VRBackground3 { get; set; }
        public static GraphicCreator HealthGuage { get; set; }
        public static GraphicCreator LivesCounter { get; set; }
        public static GraphicCreator GemsCounter { get; set; }

        public static GraphicCreator RockBlock { get; set; }
        public static GraphicCreator RockBlockFragment { get; set; }
        public static GraphicCreator RockBlockBreak { get; set; }
        public static GraphicCreator VanishingBlock { get; set; }

        public static GraphicCreator IceBlockBreak { get; set; }
     
        public static GraphicCreator MushroomGrow { get; set; }
        public static GraphicCreator MushroomShrink { get; set; }
        public static GraphicCreator Mushroom { get; set; }

        public static GraphicCreator IronKnightHelmet { get; set; }
        public static GraphicCreator RedStealthHelmet { get; set; }

        public static void Load(GameContext context)
        {
            var graphicsResource = new CSVResource<GraphicCreator>("Graphics.csv");

            foreach(var creator in graphicsResource.GetObject(context))
            {
                var property = typeof(KidCGraphic).GetProperty(creator.Name);
                property.SetValue(null,creator,null);
            }
        }

        public static GraphicCreator FromString(string text)
        {
            return typeof(KidCGraphic).GetProperty(text).GetValue(null, null) as GraphicCreator;
        }
    }

    public static class KidCResource
    {
        public class SpriteSheets
        {
            public static GameResource<SpriteSheet> Flag { get; private set; }
            public static GameResource<SpriteSheet> Dragon { get; private set; }
            public static GameResource<SpriteSheet> Puff { get; private set; }
            public static GameResource<SpriteSheet> Gem { get; private set; }
            public static GameResource<SpriteSheet> Clock { get; private set; }

        }


        public static void Init(GameContext ctx)
        {
            foreach (var prop in typeof(KidCResource.SpriteSheets).GetProperties())
            {
                prop.SetValue(null, ctx.Game.SpriteSheetResources.GetByName(prop.Name), null);
            }
        }



    }
}
