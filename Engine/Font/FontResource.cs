using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class FontResource<T> : GameResource<T> where T:IGameFont, new() 
    {
        public FontResource(string name) : base(new GamePath(PathType.Fonts, name), typeof(T)) { }
    }
}
