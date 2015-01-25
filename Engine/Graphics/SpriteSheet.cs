using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class SpriteSheet
    {
        public static SpriteSheet Load(string name, GameContext ctx)
        {
            return GameResource<SpriteSheet>.Load(new GamePath(PathType.SpriteSheets, name), ctx);
        }

        public List<AnimationFrame> Frames = new List<AnimationFrame>();
        public TextureResource Image;

        public SpriteSheet() { }

        public SpriteSheet(TextureResource image)
        {
            Image = image;
        }

        public AnimationFrame AddFrame(RGRectangleI area, RGPointI origin)
        {
            var frame = new AnimationFrame { Source = area, Origin = origin };
            Frames.Add(frame);
            return frame;
        }


    }
}
