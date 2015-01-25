using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;

namespace KidC
{
    public enum SpecialTile
    {
        Normal=0,
        Rock=10,
        Prize=20,
        Metal=30,
        Rubber=40 
    }

    public interface IBreakableTile
    {
        void Break();
    }

    public class KCTileInstance : TileInstance
    {
        public SpecialTile SpecialType
        {
            get
            {
               return (SpecialTile)(this.TileDef.TileID - 1000);
            }
        }
        public PrizeType Prize { get; set; }

        private ObjectType GetPrizeType()
        {
            if (Prize == PrizeType.None)
                return ObjectType.None;
            else
                return new ObjectType((int)this.Prize, Prize.ToString());
        }

        public override CollidingTile CreateCollidingTile(TileLayer tileLayer)
        {
            var flags = this.TileDef.Flags;

            switch (SpecialType)
            {
                case SpecialTile.Prize: return new PrizeBlock(this, tileLayer, this.GetPrizeType());
                case SpecialTile.Rock:   return new BreakableTile(this, tileLayer);
                default:  return new PlainTile(this, tileLayer);
            }
        }

        public override bool IsSpecial
        {
            get { return this.SpecialType != SpecialTile.Normal; }
        }

        public bool ShouldBreak(CollisionEvent collision)
        {
            if (!collision.OtherType.Is(KCObjectType.Player))
                return false;

            if (collision.CollisionSide != Side.Bottom)
                return false;

            var x = collision.OtherArea.X + (collision.OtherArea.Width / 2);

            if (collision.OtherArea.Right < this.TileArea.Left || collision.OtherArea.Left > this.TileArea.Right)
                return false;

            if (ShouldTileBreak(this, x))            
                return true;           

            var leftTile = this.GetAdjacentTile(-1, 0) as KCTileInstance;
            var rightTile = this.GetAdjacentTile(1, 0) as KCTileInstance;

            return !ShouldTileBreak(leftTile, x) && !ShouldTileBreak(rightTile, x);

        }

        private static bool ShouldTileBreak(KCTileInstance t, float collisionX)
        {
            return t.CanBreak() && collisionX >= t.TileArea.Left && collisionX < t.TileArea.Right;
        }

        public bool CanBreak()
        {
            switch (this.SpecialType)
            {
                case SpecialTile.Rock: case SpecialTile.Prize: return true;
                default: return false;
            }
        }
    
    }

    class PlainTile : CollidingTile
    {
        public PlainTile(TileInstance tile, TileLayer layer) : base(tile, layer) { }

        public override void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
        }
    }

}
