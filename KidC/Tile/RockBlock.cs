using Engine;
using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class RockBlock : KCCollidingTile, IBreakableTile  
    {
        private SimpleGraphic mBreakingBlock;
        public RockBlock(KCTileInstance tile, TileCollisionView collisionView)
            : base(tile, collisionView) 
        {
            mBreakingBlock = KidCGraphic.RockBlockBreak.CreateSimpleGraphic(this.Context);
        }

        protected override Engine.SoundResource HitSound
        {
            get { return Sounds.RockBlockDestroyed; }
        }

        protected override bool ShouldInteract(Engine.Collision.CollisionEvent collision, CollisionResponse response)
        {
            return this.ShouldBreak;
        }

        protected override void Update()
        {
            if (this.Age > 2)
            {
                this.Kill(Engine.ExitCode.Removed);

                var loc = this.Location.Offset(-8, -8);
                var graphic = KidCGraphic.RockBlockFragment.CreateAnimation(this.Context);
                FlyingDebris.Create(loc, TileLayer, Direction.Left, 1f, -4, graphic);
                FlyingDebris.Create(loc, TileLayer, Direction.Right, 1f, -4, graphic);
                FlyingDebris.Create(loc, TileLayer, Direction.Left, 1.5f, -1, graphic);
                FlyingDebris.Create(loc, TileLayer, Direction.Right, 1.5f, -1, graphic);
            }
        }

        public bool ShouldBreak { get; private set; }
        public void Break(CollisionEvent evt)
        {
            this.ShouldBreak = true;
        }

        protected override void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mBreakingBlock.Position = this.Location;
            mBreakingBlock.Draw(p, canvas);
        }
    }
}
