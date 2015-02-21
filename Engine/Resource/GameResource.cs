using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

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

        public System.IO.FileStream OpenFileStream()
        {
            return new System.IO.FileStream(this.Path.FullPath, System.IO.FileMode.Open);
        }
    }

    public class GameResource<T> : GameResource where T : new()
    {
        private T content;
        private bool mLoaded = false;

        public GameResource(GamePath path) : base(path) { }

        public GameResource() : base(GamePath.Undefined) { }

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
            var json = System.IO.File.ReadAllText(this.Path.FullPath);
            return Serializer.FromJson<T>(json);
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
}
