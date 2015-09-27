using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Collision;
using Engine.Input;

namespace KidC
{
    class IceWalkController : SpriteBehavior 
    {

        private PlatformerPlayerController mPlatformCtl;
        private World mWorld;

        public IceWalkController(PlatformerPlayerController platformctl)
            : base(platformctl.Sprite)
        {
            mPlatformCtl = platformctl;
            mWorld = this.Sprite.GetWorld();
        }


        protected override void Update()
        {
            //if (mPlatformCtl.OnIce)
            //    Context.DebugNumbers[0] = 1;
            //else
            //    Context.DebugNumbers[0] = 0;

            var collisionInfo = mWorld.CollisionInfo;
            var groundTile = collisionInfo.GetTile(this.Sprite.Location).GetTilesInLine(Direction.Down).Take(2).FirstOrDefault(p => p.TileDef.IsSolid);

            if (groundTile == null || this.Sprite.Location.Y - groundTile.TileArea.Top > 1 || !groundTile.IsSpecial)
            {
                mPlatformCtl.OnIce = false;
                return;
            }
            
            var kcTile = groundTile as KCTileInstance;
            mPlatformCtl.OnIce = (kcTile.KCTileDef.SpecialType == SpecialTile.Ice);
        }
    }
}
