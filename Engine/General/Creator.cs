using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    /// <summary>
    /// Encapsulates constructor parameters of an object in a JSON-friendly form that can be editted by hand
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Creator<T>
    {
        public abstract T Create(GameContext context);
    }

    public interface IFromCSV
    {
        void FillFromCSV(string[] cells);
    }

    public class GraphicCreator : IFromCSV
    {
        public string Name { get; set; }
        public string Texture { get; set; }
        public int FrameDuration { get; set; }
        public int[] Frames { get; set; }
        public bool Looping { get; set; }

        public void FillFromCSV(string[] cells)
        {
            this.Name = cells[0];
            this.Texture = cells[1];
            this.FrameDuration = cells[2].TryParseInt(0);

            var frames = cells[3];
            if (frames.EndsWith("."))
            {
                this.Looping = false;
                frames = frames.TrimEnd('.');
            }
            else
                this.Looping = true;

            this.Frames = frames.ParseIntArray();
        }

        public SimpleAnimation CreateSimpleAnimation(GameContext context)
        {
            var spriteSheetResource = context.Game.SpriteSheetResources.GetByName(this.Texture);
            var animation = new SimpleAnimation(spriteSheetResource, FrameDuration, context, Frames);
            animation.Looping = this.Looping;
            return animation;
        }

        public SimpleGraphic CreateSimpleGraphic(GameContext context)
        {
            var spriteSheetResource = context.Game.SpriteSheetResources.GetByName(this.Texture);
            return new SimpleGraphic(context, spriteSheetResource, this.Frames);
        }

        public Animation CreateAnimation(GameContext context)
        {
            var spriteSheet = context.Game.SpriteSheetResources.GetByName(this.Texture).GetObject(context);
            var animation = new Animation(spriteSheet, Direction.Right, Frames);
            animation.SetFrameDuration(this.FrameDuration);
            return animation;
        }


    }
   
}
