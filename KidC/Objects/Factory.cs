using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    static partial class KCObjectFactory
    {
        public static void Init()
        {
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Gem, CreateGem);
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Clock, CreateClock);

            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Puff, CreatePuff);

            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.IronKnightHelmet, (lyr) =>
                CreateHelmet(lyr, KCObjectType.IronKnightHelmet));

            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Player, PlayerSprite.CreateKid);
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.JamesKid, PlayerSprite.CreateKid);
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.IronKnight, PlayerSprite.CreateIronKnight);
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.RedStealth, PlayerSprite.CreateRedStealth);


            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Dragon, Dragon.Create);

            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Flag, Flag.Create);

        }

        private static SpriteCreationInfo CreateHelmet(Layer layer, ObjectType helmetType)
        {
            var stats = TransformationStats.GetStats(layer.Context, helmetType);
            return Helmet.Create(layer,helmetType, stats.PlayerType,stats.TransformSound, KidCGraphic.FromString(stats.HelmetGraphic));
        }

               
        private static SpriteCreationInfo CreateGem(Layer layer)
        {
            var ctx = layer.Context;
            var sprite = new Sprite(ctx, layer, KCObjectType.Gem);
            var spriteSheet = KidCResource.SpriteSheets.Gem;

            sprite.SetSingleAnimation(new Animation(spriteSheet.GetObject(ctx), Direction.Right, 0, 1, 2, 3));
            sprite.CurrentAnimation.SetFrameDuration(2);
            new CollectableController(sprite, Sounds.GetDiamond, ctx.CurrentMapHUD().GemsCounter, new AdjustStat(sprite, StatType.Gems, 1));
            new PrizeCollisionDelayer(sprite);
            new GravityController(sprite, GravityStrength.Low);
            sprite.AddCollisionChecks(ObjectType.Block, KCObjectType.Player);

            return new SpriteCreationInfo(sprite);            
        }

        private static SpriteCreationInfo CreateClock(Layer layer)
        {
            var ctx = layer.Context;
            var sprite = new Sprite(ctx, layer, KCObjectType.Clock);
            var spriteSheet = KidCResource.SpriteSheets.Clock;

            sprite.SetSingleAnimation(new Animation(spriteSheet.GetObject(ctx), Direction.Right, 0, 0, 0, 0, 0, 0, 1, 2));
            sprite.CurrentAnimation.SetFrameDuration(8);

            var clockPos = ctx.CurrentMapHUD().Clock;
            var targetPosition = new WorldPoint(ctx, clockPos.Area.Right + 8, clockPos.Location.Y);

            new CollectableController(sprite, Sounds.None, targetPosition, new DelayWaiter(sprite, 3)).ContinueWith(new AdjustStat(sprite, StatType.TimeRemaining, 30));
            new PrizeCollisionDelayer(sprite);
            new GravityController(sprite, GravityStrength.Low);
            new PlaySoundWhileAlive(sprite, Sounds.ClockTick);
            sprite.AddCollisionChecks(ObjectType.Block, KCObjectType.Player);

            return new SpriteCreationInfo(sprite);
        }

        private static SpriteCreationInfo CreatePuff(Layer layer)
        {
            var ctx = layer.Context;
            var spriteSheet = KidCResource.SpriteSheets.Puff.GetObject(ctx);
            var anim = new Animation(spriteSheet, Direction.Right, false, 0, 1, 2);
            return new SpriteCreationInfo(VanishingDecoration.Create(ctx, KCObjectType.Puff, layer, anim, 4));
        }
    }
}
