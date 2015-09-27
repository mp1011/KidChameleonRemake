using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Engine
{
    public abstract class GameResource : ISerializable 
    {
        public GamePath Path { get; private set; }

        protected GameResource(GamePath path) { this.Path = path; }

        public object GetSaveModel()
        {
            return Path;
        }

        public Type GetSaveModelType() { return typeof(GamePath); }

        public void Load(object saveModel)
        {
            this.Path = saveModel as GamePath;
        }
    }

    public interface IGameResource<T>
    {
        T GetObject(GameContext ctx);
    }

    public class TypedResource<T> : GameResource, IGameResource<T>
    {
        private IResourceLoader<T> mLoader;
        private T content;
        private bool mLoaded = false;

        public TypedResource(GamePath path, IResourceLoader<T> loader) : base(path)
        {
            mLoader = loader;
        }

        public T Load(GameContext context)
        {
            return mLoader.Load(context,this.Path);
        }
                 
        public bool NeedsLoad()
        {
            return !mLoaded;
        }

        public T GetObject(GameContext context)
        {
            if (!NeedsLoad())
                return content;

            content = CreateNewObject(context);

            mLoaded = true;
            return content;
        }

        protected virtual T CreateNewObject(GameContext context)
        {
            return mLoader.Load(context, this.Path);
        }

        public void Unload()
        {
            mLoaded = false;
            content = default(T);
        }
    }


    public class GameResource<T> : GameResource where T : new()
    {
        private JSONLoader<T> mLoader;
        private T content;
        private bool mLoaded = false;

        public GameResource(GamePath path, Type deserializeType) : this(path) 
        {
            mLoader = new JSONLoader<T>(deserializeType);
        }

        public GameResource(GamePath path) : base(path) 
        {
            mLoader = new JSONLoader<T>();
        }

        public GameResource(string name, PathType pathType) : this(new GamePath(pathType,name)) { }

        public GameResource() : base(GamePath.Undefined) 
        {
            mLoader = new JSONLoader<T>();
        }

        protected virtual bool NeedsLoad()
        {
            return !mLoaded;
        }

        public T GetObject(GameContext context)
        {
            if (!NeedsLoad())
                return content;

            content = CreateNewObject(context);

            mLoaded = true;
            return content;
        }

        protected virtual T CreateNewObject(GameContext context)
        {
            return mLoader.Load(context, this.Path);
        }

        public static T Load(GamePath path, GameContext ctx)
        {
            var res = new GameResource<T>(path);
            return res.GetObject(ctx);
        }
    }

    public class TextureResource : GameResource
    {
        public TextureResource(string name) : base(new GamePath { Type = PathType.Textures, Name = name }) { }

        public TextureResource() : base(GamePath.Undefined) { }

        public TextureResource GetFlashTexture()
        {
            return new TextureResource(this.Path.Name + "_flash");
        }

        public static TextureResource FromCache(string name)
        {
            throw new NotImplementedException();
           // return GameResource.FromCache<TextureResource>(name);
        }
    }

    public class StringResource : TypedResource<String>
    {
        public StringResource(string name) : base(new GamePath(PathType.Info, name), new StringLoader()){}
    }

    public class InMemoryResource<T> : GameResource<T> where T:new()
    {
        private T mItem;

        public InMemoryResource(T obj)
            : base()
        {
            mItem = obj;
        }

        protected override T CreateNewObject(GameContext context)
        {
            return mItem;
        }
    }

    public class CSVResource<T> : TypedResource<T[]> where T:IFromCSV,new()
    {
        public CSVResource(string name) : base(new GamePath(PathType.Info,name), new CSVLoader<T>()) { } 
    }


}
