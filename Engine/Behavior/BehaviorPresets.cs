using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Presets
{
    public class Debris
    {
        public static Sprite Create(Sprite origin, SpriteAnimation graphic)
        {
            return Create(origin, graphic.Animation);
        }

        public static Sprite Create(Sprite origin, Animation graphic)
        {

            var debris = new Sprite(origin.Context, origin.DrawLayer, ObjectType.Thing);
            debris.SetSingleAnimation(graphic);
            debris.Location = origin.Location;
            debris.AddBehavior(new DestroyWhenOutOfFrame(debris));
            
            var gravity = debris.AddBehavior(new GravityController(debris));
            gravity.CurrentYSpeed = -5f;

            origin.DrawLayer.AddObject(debris);
            return debris;
        }

    }
}
