using Engine;
using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{

    class VanishingBlock : KCCollidingTile 
    {
        private const int Duration = 10;
        private SimpleAnimation mAnimation;

        public VanishingBlock(KCTileInstance tile, TileCollisionView collisionView)
            : base(tile, collisionView) 
        {
            mAnimation = KidCGraphic.VanishingBlock.CreateSimpleAnimation(TileLayer.Context);
        }

        protected override SoundResource HitSound
        {
            get { return Sounds.BlockVanish; }
        }

        protected override bool ShouldInteract(CollisionEvent collision, CollisionResponse response)
        {
            return collision.OtherType.Is(KCObjectType.Player);
        }

        protected override void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mAnimation.Location = this.Location;
            mAnimation.Draw(p, canvas);
        }

        protected override void Update()
        {
            if (this.Age > Duration)            
                this.Kill(Engine.ExitCode.Finished);            
        }
    }
}
