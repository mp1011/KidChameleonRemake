using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class TestWorld : GameResource<World>
    {
        protected override World CreateNewObject(GameContext ctx)
        {
            var world = new World(ctx);
            var map = GameResource<Map>.Load(new GamePath(PathType.Maps, "test2"), ctx);

            world.BackgroundColor = RGColor.FromRGB(72, 144, 248);

            var bg = SpriteSheet.Load("woods_bg", ctx);
            var firstLayer = world.AddLayer(ImageLayer.CreateRepeatingHorizontal(ctx, new SimpleGraphic(bg, 1), .2f));
            firstLayer.Position = new RGPoint(0, 32);

            var secondLayer = world.AddLayerBelowLast(ImageLayer.CreateRepeatingHorizontal(ctx, new SimpleGraphic(bg, 0), .6f), firstLayer);
            var thirdLayer = world.AddLayerBelowLast(ImageLayer.CreateRepeatingHorizontal(ctx, new SimpleGraphic(bg, 2), .6f),secondLayer);

            var layer = world.AddLayer(new TileLayer(ctx, map, RGPoint.Empty, LayerDepth.Foreground));
            var kid = KCObjectType.JamesKid.CreateInstance<Sprite>(layer, ctx);
            layer.AddObject(kid);
         //   kid.Location = new RGPoint(317.498f, 96.2f);


            var dragon = KCObjectType.Dragon.CreateInstance<Sprite>(layer, ctx);
            layer.AddObject(dragon);
            dragon.Location = new RGPoint(200, 50);
            world.AddLayer(new HUD(ctx));


            return world;       
        }
    }
   
}
