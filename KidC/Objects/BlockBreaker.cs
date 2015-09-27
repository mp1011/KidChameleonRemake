using Engine;
using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    /// <summary>
    /// Object that breaks blocks upon collision
    /// </summary>
    public class BlockBreaker : ITypedCollisionResponder<IBreakableTile>
    {
        private Sprite mSprite;
        public BlockBreaker(Sprite s)
        {
            mSprite = s;
            this.RegisterTypedCollider(mSprite);
        }

        protected virtual bool ShouldBreakBlock(IBreakableTile block, CollisionEvent collision)
        {
            return true;
        }

        public void HandleCollision(IBreakableTile other, CollisionEvent collision, CollisionResponse response)
        {
            if (this.ShouldBreakBlock(other, collision))
                other.Break(collision);
        }
    }

    /// <summary>
    /// Causes the object to break tiles by jumping into them
    /// </summary>
    class PlatformerBlockBreaker : BlockBreaker
    {

        public PlatformerBlockBreaker(Sprite s) : base(s) { }

        protected override bool ShouldBreakBlock(IBreakableTile tile, CollisionEvent collision)
        {
            if (collision.CollisionSide != Side.Top)
                return false;

            var x = collision.ThisArea.X + (collision.ThisArea.Width / 2);

            var tileInstance = tile.TileInstance;

            if (collision.ThisArea.Right < tileInstance.TileArea.Left || collision.ThisArea.Left > tileInstance.TileArea.Right)
                return false;

            if (tile.TileInstance.ShouldBreak(x))
                return true;

            var leftTile = tileInstance.GetAdjacentTile(-1, 0) as KCTileInstance;
            var rightTile = tileInstance.GetAdjacentTile(1, 0) as KCTileInstance;

            return !leftTile.ShouldBreak(x) && !rightTile.ShouldBreak(x);
        }
    }
}
