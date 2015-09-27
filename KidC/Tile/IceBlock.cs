using Engine;
using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class IceBlock : KCCollidingTile, IBreakableTile
    {
        private SimpleGraphic mGraphic;
        private Direction mDirection;

        public IceBlock(KCTileInstance tile, TileCollisionView collisionView)
            : base(tile, collisionView) 
        {
            mGraphic = KidCGraphic.IceBlockBreak.CreateSimpleGraphic(this.Context);
        }

        protected override SoundResource HitSound
        {
            get { return Sounds.RockBlockDestroyed; }
        }

        protected override bool ShouldInteract(CollisionEvent collision, CollisionResponse response)
        {
             return this.ShouldBreak;
        }

        protected override void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mGraphic.Position = this.Location;
            mGraphic.Draw(p, canvas);
        }

        protected override void Update()
        {
            if (this.Age > 2)
            {
                this.Kill(Engine.ExitCode.Removed);

                var loc = this.Location;
                var breakDirection = Direction.Up;

                List<Sprite> spikes = new List<Sprite>();
                spikes.Add(KidCObject.IceFragment.Create(this.TileLayer));
                spikes.Add(KidCObject.IceFragment.Create(this.TileLayer));
                spikes.Add(KidCObject.IceFragment.Create(this.TileLayer));

                foreach (var spike in spikes)
                {
                    spike.MotionManager.MainMotion.Direction = mDirection;
                    spike.Direction = mDirection;
                }

                var center = this.TileInstance.TileArea.Center;
                spikes[0].Location = center.Offset(mDirection, 4);
                spikes[1].Location = center.Offset(mDirection.RotateD(RotationType.Clockwise, 90), 4);
                spikes[2].Location = center.Offset(mDirection.RotateD(RotationType.Counterclockwise, 90), 4);


            }
        }

        public bool ShouldBreak { get; private set; }
        public void Break(CollisionEvent evt)
        {
            this.ShouldBreak = true;
            mDirection = evt.CollisionSide.ToDirection();
        }

      
    }


}
