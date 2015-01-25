using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Graphics
{
    abstract class TextureCache<TTexture, TTextureProvider> where TTexture : class
    {
        private Dictionary<string, TTexture> mCache = new Dictionary<string, TTexture>();

        public TTexture GetTexture(TextureResource resource, TTextureProvider textureProvider)
        {
            var texture = mCache.TryGet(resource.Path.Name,null);

            if (texture == null)
            {
                texture = CreateTexture(resource, textureProvider);
                mCache.AddOrSet(resource.Path.Name, texture);
            }

            return texture;
        }

        protected abstract TTexture CreateTexture(TextureResource resource, TTextureProvider textureProvider);
    }
}
