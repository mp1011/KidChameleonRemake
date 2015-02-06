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
        private RGPointI mOriginalPosition;
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

            this.Sprite.RemoveCollisionType(ObjectType.Block);
        }

        protected override void Update()
        {
            if (this.Sprite.Area.Top > mSpawnTile.TileArea.Bottom)
                this.Sprite.ReAddCollisionType(ObjectType.Block);

        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
        }

        protected override void OnExit()
        {            
            if(this.Sprite.ExitCode == Engine.ExitCode.Collected)
                CollectedItem.Create(this.Sprite);
        }
    }
}
