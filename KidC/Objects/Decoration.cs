using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    /// <summary>
    /// Plays its animation once and then disappears.
    /// </summary>
    class VanishingDecoration 
    {

        public static Sprite Create(GameContext ctx, ObjectType type, Layer layer, Animation animation, int animationSpeed)
        {
            var sprite = new Sprite(ctx,layer,type);
            sprite.SetSingleAnimation(animation);
            sprite.CurrentAnimation.SetFrameDuration(animationSpeed);
            new DestroyWhenAnimationFinished(sprite);
            return sprite;
        }
    }

    class FlyingDebris 
    {

        public static void Create(RGPointI location, Layer layer, Direction d, float speed, int ySpeed, string spriteSheetName, int frame)
        {
            var debris = new Sprite(layer.Context, layer, ObjectType.Decoration);

            var spriteSheet = SpriteSheet.Load(spriteSheetName, layer.Context);
            debris.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, frame));

            var g = new GravityController(debris);
            g.CurrentYSpeed = ySpeed;
            new DestroyWhenOutOfFrame<Sprite>(debris,false);

            debris.MotionManager.MainMotion.Set(d, speed);
            layer.AddObject(debris);

            debris.Location = location;
        }

    }
}
