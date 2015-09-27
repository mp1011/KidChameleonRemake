using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class SimpleProjectile
    {

        public static void Create(RGPointI location, Layer layer, Direction d, float speed, GameResource<SpriteSheet> spriteSheet, int frame)
        {
            throw new NotImplementedException();
          //  return Sprite.Create(layer, location).SetSingleAnimation(spriteSheet, frame).SetMotion(d, speed);

            //var spriteSheet = SpriteSheet.Load(spriteSheetName, layer.Context);
            //projectile.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, frame));

            //new DestroyWhenOutOfFrame<Sprite>(projectile,false);
            //projectile.MotionManager.MainMotion.Set(d, speed);
            //layer.AddObject(projectile);
            //projectile.Location = location;
        }

    }
}
