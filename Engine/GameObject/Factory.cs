using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class ObjectFactory
    {
        private static Dictionary<int, Func<GameContext, Layer, LogicObject>> mFactories = new Dictionary<int, Func<GameContext, Layer, LogicObject>>();

        public static void AddFactoryMethod(ObjectType o, Func<GameContext, Layer, LogicObject> factory)
        {
            mFactories.Add(o.Value, factory);
        }

        public static T CreateInstance<T>(this ObjectType type, Layer layer, GameContext ctx) where T : LogicObject, IDrawableRemovable
        {
            var item = mFactories[type.Value](ctx, layer);
            var itemT = item as T;
            layer.AddObject(itemT);
            return itemT;
        }

        public static Sprite CreateSprite(this ObjectType type, Layer layer, GameContext ctx)
        {
            return type.CreateInstance<Sprite>(layer, ctx);
        }
    }
}
