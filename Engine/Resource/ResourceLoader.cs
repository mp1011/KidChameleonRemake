using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Engine
{

    public interface IResourceLoader<T>
    {
        T Load(GameContext context, GamePath path);
    }

    class StringLoader : IResourceLoader<string>
    {
        public string Load(GameContext context, GamePath path)
        {
            using (var stream = path.OpenFileStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();                            
        }
    }

    class JSONLoader<T> : IResourceLoader<T> where T : new()
    {
        private Type mOverrideType;

        public JSONLoader() { }
        public JSONLoader(Type overrideType)
        {
            mOverrideType = overrideType;
        }

        public T Load(GameContext context, GamePath path)
        {
            var json = System.IO.File.ReadAllText(path.FullPath);
            if (mOverrideType != null)
                return (T)Serializer.FromJson(json, mOverrideType);
            else 
                return Serializer.FromJson<T>(json);        
        }
    }

    class CSVLoader<T> : IResourceLoader<T[]> where T: IFromCSV, new() 
    {
        public T[] Load(GameContext context, GamePath path)        
        {
            var collection = new List<T>();

            var csv = new StringLoader().Load(context, path);
            var lines = csv.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1))
            {
                var cells = line.Split(',');
                T row = new T();
                row.FillFromCSV(cells);
                collection.Add(row);
            }

            return collection.ToArray();
        }
    }

}
