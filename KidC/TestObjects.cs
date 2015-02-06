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
            firstLayer.Position = new RGPointI(0, 32);

            var secondLayer = world.AddLayerBelowLast(ImageLayer.CreateRepeatingHorizontal(ctx, new SimpleGraphic(bg, 0), .6f), firstLayer);
            var thirdLayer = world.AddLayerBelowLast(ImageLayer.CreateRepeatingHorizontal(ctx, new SimpleGraphic(bg, 2), .6f),secondLayer);

            var layer = world.AddLayer(new TileLayer(ctx, map, RGPointI.Empty, LayerDepth.Foreground));


            //var testObject = CreateTestObject(ctx,layer);
            //layer.AddObject(testObject);
            //testObject.Location = new RGPointI(100, 100);

            var kid = KCObjectType.JamesKid.CreateInstance<Sprite>(layer, ctx);
            layer.AddObject(kid);
//            kid.Location = new RGPoint(317.498f, 96.2f);


         //   var dragon = KCObjectType.Dragon.CreateInstance<Sprite>(layer, ctx);
         //   layer.AddObject(dragon);
          //  dragon.Location = new RGPointI(200, 50);
            world.AddLayer(new HUD(ctx));


            return world;       
        }

        private Sprite CreateTestObject(GameContext ctx, Layer l)
        {
            var s = new Sprite(ctx, l, ObjectType.Thing);
            var spriteSheet = GameResource<SpriteSheet>.Load(new GamePath(PathType.SpriteSheets, "kid"), ctx);
            s.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, 0));
            s.AddBehavior(new TestController(s));
            return s;
        }

    }

    class TestController : SpriteBehavior
    {
        public TestController(Sprite s) : base(s) { }

        protected override void OnEntrance()
        {
            this.Sprite.MotionManager.MainMotion.Set(Direction.Right, .5f);
        }

        protected override void Update()
        {
            
        }
    }
   
}
