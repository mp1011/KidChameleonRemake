using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Engine
{
    public enum PathType
    {
        Undefined = 0,
        SpriteSheets = 1,
        Textures = 2,
        Tilesets = 3,
        Maps = 4,
        Sounds = 5,
        Info = 6,
        Fonts = 7,
        Recordings = 8
    }

    public static class PathTypeUtil
    {
        public static string GetExtension(this PathType pt)
        {
            switch (pt)
            {
                case PathType.Textures: return "png";
                case PathType.SpriteSheets: return "sprite";
                case PathType.Tilesets: return "tileset";
                case PathType.Maps: return "map";
                case PathType.Sounds: return "wav";
                case PathType.Info: return "txt";
                case PathType.Fonts: return "txt";
                case PathType.Recordings: return "txt";
                default: return "";
            }
        }
        public static string GetFolder(this PathType pt)
        {
            return System.IO.Path.Combine(Paths.DataRoot, pt.ToString());
        }
    }

    public class GamePath
    {
        public PathType Type { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public string Extension
        {
            get
            {
                return Type.GetExtension();
            }
        }

        [JsonIgnore]
        public string FullPath
        {
            get
            {
                var filename = Name;
                if (!filename.Contains('.'))
                    filename += "." + Extension;

                return System.IO.Path.Combine(Paths.DataRoot, this.Type.ToString(), filename);
            }
        }

        public GamePath() { }

        public GamePath(PathType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        public static GamePath Undefined { get { return new GamePath { Type = PathType.Undefined }; } }

        public override string ToString()
        {
            return this.Type.ToString() + "-" + this.Name;
        }

        public System.IO.FileStream OpenFileStream()
        {
            try
            {
                return new System.IO.FileStream(this.FullPath, System.IO.FileMode.Open);
            }
            catch (IOException io)
            {
                var over = this.FullPath + ".over";
                File.Copy(this.FullPath, over,true);
                return new System.IO.FileStream(over, System.IO.FileMode.Open);
            }
        }
    }
    
}
