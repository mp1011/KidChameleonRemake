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
            return null;    
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
