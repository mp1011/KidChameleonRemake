using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class ResourceCache<T> where T:new() 
    {
        private PathType mPathType;
        class CacheEntry
        {
            public GameResource<T> Resource;             
        }

        private Dictionary<string, CacheEntry> mCache = new Dictionary<string, CacheEntry>();

        public ResourceCache(PathType pathType)
        {
            mPathType = pathType;
        }

        public GameResource<T> GetByName(string key)
        {
            var entry = mCache.TryGet(key, null);
            if (entry == null)
            {
                entry = new CacheEntry()
                {
                    Resource = new GameResource<T>(new GamePath(mPathType,key))
                };
                mCache.Add(key, entry);
            }

            return entry.Resource;
        }



    }



}
