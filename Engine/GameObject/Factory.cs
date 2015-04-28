using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class ObjectFactory
    {
        private static Dictionary<int, Func<GameContext, Layer, LogicObject>> mFactories = new Dictionary<int, Func<GameContext, Layer, LogicObject>>();
        private static Dictionary<int, Func<GameContext, Layer, SpriteCreationInfo>> mSpriteFactories = new Dictionary<int, Func<GameContext, Layer, SpriteCreationInfo>>();

        public static void AddLogicObjectFactoryMethod(ObjectType o, Func<GameContext, Layer, LogicObject> factory)
        {
            mFactories.Add(o.Value, factory);
        }

        public static void AddSpriteFactoryMethod(ObjectType o, Func<GameContext, Layer, SpriteCreationInfo> factory)
        {
            mSpriteFactories.Add(o.Value, factory);
        }

        public static T CreateLogicObjectInstance<T>(this ObjectType type, Layer layer, GameContext ctx) where T : LogicObject, IDrawableRemovable
        {
            var item = mFactories[type.Value](ctx, layer);
            var itemT = item as T;
            layer.AddObject(itemT);
            return itemT;
        }

        public static SpriteCreationInfo CreateSpriteInstance(this ObjectType type, Layer layer, GameContext ctx) 
        {
            var item = mSpriteFactories[type.Value](ctx, layer);
            var itemT = item as SpriteCreationInfo;
            layer.AddObject(itemT.Sprite);
            return itemT;
        }


        public static SpriteCreationInfo CreateSprite(this ObjectType type, Layer layer, GameContext ctx)
        {
            return type.CreateSpriteInstance(layer, ctx);
        }
    }

    public class SpriteCreationInfo
    {
        public Sprite Sprite { get; private set; }
        public List<SpriteBehavior> Behaviors { get; private set; }

        public T GetBehavior<T>() where T : SpriteBehavior
        {
            var behavior = Behaviors.OfType<T>().FirstOrDefault();
            if (behavior == default(T))
                throw new Exception("Expected to find a behavior of type " + typeof(T).Name);
            return behavior;
        }

        public T AddBehavior<T>(T behavior) where T : SpriteBehavior
        {
            Behaviors.Add(behavior);
            return behavior;
        }

        public SpriteCreationInfo(Sprite s)
        {
            this.Sprite = s;
            this.Behaviors = new List<SpriteBehavior>();
        }
    }
}
