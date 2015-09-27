using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;

namespace KidC
{
    class PrizeBlock : KCCollidingTile, IBreakableTile 
    {
        private SimpleGraphic mGraphic;
        private int mMotion = 2;
        private int mRange = 4;

        public PrizeBlock(KCTileInstance tile, TileCollisionView collisionView)
            : base(tile, collisionView) 
        {
            mGraphic = KidCGraphic.RockBlock.CreateSimpleGraphic(this.Context);
        }

        protected override SoundResource HitSound
        {
	        get { return Sounds.BlockHit;}
        }

        protected override SpecialTile? FinalTile
        {
	        get 
	        { 
		        return SpecialTile.Rock; 
	        }
        }

        protected override bool ShouldInteract(CollisionEvent collision, CollisionResponse response)
        {
            return this.ShouldBreak;
        }

        protected override void Update()
        {

            var initialY = Tile.TileArea.Center.Y;
            this.Location = this.Location.Offset(0, mMotion);

            if (this.Location.Y > initialY + mRange)
                mMotion = -1 * Math.Abs(mMotion);
            else if (this.Location.Y < initialY - mRange)
                mMotion = Math.Abs(mMotion);

            if (this.Age > 30)
            {
                this.Kill(Engine.ExitCode.Removed);

                var puff = KCObjectType.Puff.CreateSpriteInstance(this.TileLayer, this.Context).Sprite;
                puff.Location = this.Location.Offset(0, -12);

                new CreateObjectWhenDestroyed(puff, this.Prize, RGPointI.Empty);
            }
        }

        private ObjectType Prize
        {
            get
            {
                if (this.TileInstance.Prize == PrizeType.None)
                    return ObjectType.None;
                else
                    return new ObjectType((int)this.TileInstance.Prize, this.TileInstance.Prize.ToString());
            }
        }


        public bool ShouldBreak { get; private set; }
        public void Break(CollisionEvent evt)
        {
            this.ShouldBreak = true;
        }

        protected override void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mGraphic.Position = this.Location;
            mGraphic.Draw(p, canvas);
        }
    }
}
