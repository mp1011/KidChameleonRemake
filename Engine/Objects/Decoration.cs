using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Engine
{
    /// <summary>
    /// Plays its animation once and then disappears.
    /// </summary>
    public class VanishingDecoration 
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

    public class FlyingDebris 
    {

        public static void Create(RGPointI location, Layer layer, Direction d, float speed, int ySpeed, Animation graphic)
        {
             var debris = new Sprite(layer.Context, layer, ObjectType.Decoration);

             debris.SetSingleAnimation(graphic);

            var g = new GravityController(debris);
            g.CurrentYSpeed = ySpeed;
            new DestroyWhenOutOfFrame(debris);

            debris.MotionManager.MainMotion.Set(d, speed);
            layer.AddObject(debris);

            debris.Location = location;
        }

    }
}
