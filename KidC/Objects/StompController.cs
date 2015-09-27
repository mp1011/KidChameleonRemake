using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    /// <summary>
    /// Plays a stomp sound if the sprite hits the ground with enough speed
    /// </summary>
    class StompController : SpriteBehavior 
    {
        private SoundResource mSound;
        private float mMinYSpeed = 2f;
        public StompController(Sprite sprite, SoundResource sound)
            : base(sprite)
        {
            mSound = sound;
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {

            if (cEvent.OtherType.Is(ObjectType.Block) && cEvent.ThisCollisionTimeSpeed.Y >= mMinYSpeed &&
                cEvent.CollisionSide == Engine.Collision.Side.Bottom)
                SoundManager.PlaySound(mSound);
        }
    }
}
