using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{

    class CollectableController : SpriteBehavior
    {
        private SoundResource mCollectedSound;

        public CollectableController(Sprite s, SoundResource collectedSound) : base(s) 
        {
            mCollectedSound = collectedSound;
        }


        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(KCObjectType.Player))
            {
                SoundManager.PlaySound(mCollectedSound);
                response.DestroyType = Engine.ExitCode.Collected;
                return;
            }
        }
    }

    class PrizeController : SpriteBehavior 
    {
        private RGPoint mOriginalPosition;
        private TileLayer mLayer;
        private TileInstance mSpawnTile;

        public PrizeController(Sprite s)
            : base(s)
        {
        }

        protected override void OnEntrance()
        {
            mOriginalPosition = this.Sprite.Location;
            mLayer = this.Sprite.DrawLayer as TileLayer;

            var tile = mLayer.Map.GetTileAtLocation(this.Sprite.Location);
            mSpawnTile = mLayer.Map.GetTileAtCoordinates(tile.TileLocation.X, tile.TileLocation.Y + 1);
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherArea.Equals(mSpawnTile.TileArea))
            {
                response.ShouldContinueHandling = false;
                response.ShouldBlock = false;
            }
        }

        protected override void OnExit()
        {            
            if(this.Sprite.ExitCode == Engine.ExitCode.Collected)
                CollectedItem.Create(this.Sprite);
        }
    }
}
