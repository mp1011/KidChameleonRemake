using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class MovingObjectCreator : IFromCSV
    {
        public string Name { get; set; }
        public Direction InitialDirection { get; set; }
        public int Speed { get; set; }
        public string[] Behaviors { get; set; }
        public string Texture {get;set;}
        public int FrameDuration {get;set;}
        public int[] Frames {get;set;}

        public void FillFromCSV(string[] cells)
        {
            this.Name = cells[0];
            this.Texture = cells[1];
            this.FrameDuration = cells[2].TryParseInt(0);
            this.Frames = cells[3].ParseIntArray();
            this.InitialDirection = Direction.Parse(cells[4]);
            this.Speed = cells[5].TryParseInt(0);
            this.Behaviors = cells[6].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Sprite Create(Layer layer)
        {
            var context = layer.Context;
            var sprite = new Sprite(layer, ObjectType.Thing);
            sprite.SetMotion(this.InitialDirection, this.Speed);


            var spriteSheetResource = context.Game.SpriteSheetResources.GetByName(this.Texture);
            var animation = new Animation(spriteSheetResource.GetObject(context), this.InitialDirection, this.Frames);
            animation.SetFrameDuration(this.FrameDuration);
            sprite.SetSingleAnimation(animation);

            foreach (var behavior in Behaviors)
                CreateBehaviorFromString(sprite, behavior);

            layer.AddObject(sprite);
            return sprite;
        }

        private static void CreateBehaviorFromString(Sprite sprite, string text)
        {
            try
            {
                object[] args = ParseBehaviorString(text);
                string typename = args[0].ToString();
                args[0] = sprite;
                Type behaviorType = ReflectionHelper.EngineAssembly.GetTypes().FirstOrDefault(p => p.Name == typename);
                if (behaviorType == null)
                {
                    behaviorType = ReflectionHelper.GameAssembly.GetTypes().FirstOrDefault(p => p.Name == typename);
                }
                Activator.CreateInstance(behaviorType, args);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to parse behavior string: " + text, ex);
            }
        }

        private static object[] ParseBehaviorString(string text)
        {
            List<object> args = new List<object>();
            foreach (var cell in text.Split(';'))
            {
                args.Add(cell);
                //todo, parse nums and such
            }
            return args.ToArray();
        }

       
    }
}
