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

            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.IronKnightHelmet, (ctx, lyr) => CreateHelmet(ctx, lyr, KCObjectType.IronKnightHelmet, KCObjectType.IronKnight, Sounds.IronKnightTransform, "ironknight", 28));


            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.RedStealthHelmet, (ctx, lyr) => CreateHelmet(ctx, lyr, KCObjectType.RedStealthHelmet, KCObjectType.RedStealth, Sounds.RedStealthTransform, "redstealth", 0));

            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Player, CreateKid);
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.JamesKid, CreateKid);
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.IronKnight, CreateIronKnight);
            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.RedStealth, CreateRedStealth);


            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Dragon, Dragon.Create);

            ObjectFactory.AddSpriteFactoryMethod(KCObjectType.Flag, Flag.Create);

        }


        private static SpriteCreationInfo CreateHelmet(GameContext ctx, Layer layer, ObjectType helmetType,  ObjectType playerType, SoundResource transformSound, string spriteSheetName, int spriteSheetIndex)
        {
            var sprite = new Sprite(ctx, layer, helmetType);
            var spriteSheet = SpriteSheet.Load(spriteSheetName, ctx);

            sprite.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, spriteSheetIndex));
            new CollectableController(sprite, Sounds.None,null,null);
            new PrizeCollisionDelayer(sprite);
            new GravityController(sprite, GravityStrength.Low);
            sprite.AddCollisionChecks(ObjectType.Block, KCObjectType.Player);
            new HelmetController(sprite, playerType, transformSound);
    
            return new SpriteCreationInfo(sprite);            

        }
               
        private static SpriteCreationInfo CreateGem(GameContext ctx, Layer layer)
        {
            var sprite = new Sprite(ctx, layer, KCObjectType.Gem);
            var spriteSheet = SpriteSheet.Load("gem",ctx);

            sprite.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, 0, 1, 2, 3));
            sprite.CurrentAnimation.SetFrameDuration(2);
            new CollectableController(sprite, Sounds.GetDiamond, ctx.CurrentMapHUD().GemsCounter, new AdjustStat(sprite, StatType.Gems,1));          
            new PrizeCollisionDelayer(sprite);
            new GravityController(sprite, GravityStrength.Low);
            sprite.AddCollisionChecks(ObjectType.Block, KCObjectType.Player);

            return new SpriteCreationInfo(sprite);            
        }

        private static SpriteCreationInfo CreateClock(GameContext ctx, Layer layer)
        {
            var sprite = new Sprite(ctx, layer, KCObjectType.Clock);
            var spriteSheet = SpriteSheet.Load("clock", ctx);

            sprite.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, 0, 0, 0, 0, 0, 0, 1, 2));
            sprite.CurrentAnimation.SetFrameDuration(8);

            var clockPos = ctx.CurrentMapHUD().Clock;
            var targetPosition = new WorldPoint(ctx, clockPos.Area.Right+8, clockPos.Location.Y);

            new CollectableController(sprite, Sounds.None, targetPosition, new DelayWaiter(sprite,3)).ContinueWith(new AdjustStat(sprite, StatType.TimeRemaining,30));
            new PrizeCollisionDelayer(sprite);
            new GravityController(sprite, GravityStrength.Low);
            new PlaySoundWhileAlive(sprite, Sounds.ClockTick);
            sprite.AddCollisionChecks(ObjectType.Block, KCObjectType.Player);

            return new SpriteCreationInfo(sprite);
        }


        private static SpriteCreationInfo CreatePuff(GameContext ctx, Layer layer)
        {
            var anim = new Animation(SpriteSheet.Load("puff",ctx), Direction.Right,false,0,1,2);
            return new SpriteCreationInfo(VanishingDecoration.Create(ctx, KCObjectType.Puff, layer, anim, 4));
        }
    }
}
