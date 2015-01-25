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
            ObjectFactory.AddFactoryMethod(KCObjectType.Gem, CreateGem);
            ObjectFactory.AddFactoryMethod(KCObjectType.Puff, CreatePuff);

            ObjectFactory.AddFactoryMethod(KCObjectType.IronKnightHelmet, (ctx, lyr) => CreateHelmet(ctx, lyr,KCObjectType.IronKnightHelmet, KCObjectType.IronKnight, Sounds.IronKnightTransform, "ironknight", 28));


            ObjectFactory.AddFactoryMethod(KCObjectType.RedStealthHelmet, (ctx, lyr) => CreateHelmet(ctx, lyr, KCObjectType.RedStealthHelmet, KCObjectType.RedStealth, Sounds.RedStealthTransform, "redstealth", 0));

            ObjectFactory.AddFactoryMethod(KCObjectType.Player, CreateKid);
            ObjectFactory.AddFactoryMethod(KCObjectType.JamesKid, CreateKid);
            ObjectFactory.AddFactoryMethod(KCObjectType.IronKnight, CreateIronKnight);
            ObjectFactory.AddFactoryMethod(KCObjectType.RedStealth, CreateRedStealth);


            ObjectFactory.AddFactoryMethod(KCObjectType.Dragon, Dragon.Create);
        }


        private static Sprite CreateHelmet(GameContext ctx, Layer layer, ObjectType helmetType,  ObjectType playerType, SoundResource transformSound, string spriteSheetName, int spriteSheetIndex)
        {
            var sprite = new Sprite(ctx, layer, helmetType);
            var spriteSheet = SpriteSheet.Load(spriteSheetName, ctx);

            sprite.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, spriteSheetIndex));
            sprite.AddBehavior(new CollectableController(sprite, Sounds.None));
            sprite.AddBehavior(new GravityController(sprite));
            sprite.AddCollisionChecks(ObjectType.Block, KCObjectType.Player);

            sprite.AddBehavior(new HelmetController(sprite, playerType, transformSound));
    
            return sprite;            

        }
               
        private static Sprite CreateGem(GameContext ctx, Layer layer)
        {
            var sprite = new Sprite(ctx, layer, KCObjectType.Gem);
            var spriteSheet = SpriteSheet.Load("gem",ctx);

            sprite.SetSingleAnimation(new Animation(spriteSheet, Direction.Right, 0, 1, 2, 3));
            sprite.CurrentAnimation.SetFrameDuration(2);
            sprite.AddBehavior(new CollectableController(sprite, Sounds.GetDiamond));          
            sprite.AddBehavior(new PrizeController(sprite));
            sprite.AddBehavior(new GravityController(sprite));
            sprite.AddCollisionChecks(ObjectType.Block, KCObjectType.Player);

            return sprite;            
        }


        private static Sprite CreatePuff(GameContext ctx, Layer layer)
        {
            var anim = new Animation(SpriteSheet.Load("puff",ctx), Direction.Right,false,0,1,2);
            return VanishingDecoration.Create(ctx, KCObjectType.Puff, layer, anim, 4);
        }
    }
}
