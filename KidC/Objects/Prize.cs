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
        private IWithPosition mHUDTarget;
        private ILogicObject mOnCollected;
        private NoPrizeBonus mNoPrizeBonus;

        public CollectableController(Sprite s, SoundResource collectedSound, IWithPosition hudTarget, ILogicObject onCollected) : base(s) 
        {
            mCollectedSound = collectedSound;
            mHUDTarget = hudTarget;
            mOnCollected = onCollected;
            mNoPrizeBonus = this.Context.GetBonusTrackers().OfType<NoPrizeBonus>().FirstOrDefault();
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(KCObjectType.Player))
            {
                mNoPrizeBonus.Reject();
                SoundManager.PlaySound(mCollectedSound);
                response.DestroyType = Engine.ExitCode.Collected;
                return;
            }
        }

        protected override void OnExit()
        {
            if (this.Sprite.ExitCode == Engine.ExitCode.Collected)
            {
                var collectedSprite = new Sprite(this.Context, this.Context.CurrentMapHUD(), this.Sprite.ObjectType);
                this.Context.CurrentMapHUD().AddObject(collectedSprite);
                collectedSprite.SetSingleAnimation(this.Sprite.CurrentAnimation.Animation);
                collectedSprite.Location = this.Sprite.DrawLayer.LayerPointToScreenPoint(this.Sprite.Location);

                ILogicObject exitBehavior = this;
                
                if(mHUDTarget != null)
                    exitBehavior = exitBehavior.ContinueWith(new SeekPointController(collectedSprite, collectedSprite, mHUDTarget, 5f));
                
                if(mOnCollected != null)
                    exitBehavior = exitBehavior.ContinueWith(mOnCollected);

                exitBehavior.ContinueWith(new KillObject(collectedSprite, Engine.ExitCode.Finished));
            }
        }
    }

    /// <summary>
    /// Prevents prize blocks from colliding with the tile they were spawned from
    /// </summary>
    class PrizeCollisionDelayer : SpriteBehavior 
    {
        private RGPointI mOriginalPosition;
        private TileLayer mLayer;
        private TileInstance mSpawnTile;

        public PrizeCollisionDelayer(Sprite s)
            : base(s)
        {
        }

        protected override void OnEntrance()
        {
            mOriginalPosition = this.Sprite.Location;
            mLayer = this.Sprite.DrawLayer as TileLayer;

            if (mLayer != null)
            {
                var tile = mLayer.Map.GetTileAtLocation(this.Sprite.Location);
                mSpawnTile = mLayer.Map.GetTileAtCoordinates(tile.TileLocation.X, tile.TileLocation.Y + 1);
            }
            this.Sprite.RemoveCollisionType(ObjectType.Block);
        }

        protected override void Update()
        {
            if (mSpawnTile != null && this.Sprite.Area.Top > mSpawnTile.TileArea.Bottom)
                this.Sprite.ReAddCollisionType(ObjectType.Block);
        }

    }
}
