using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Engine
{
    /// <summary>
    /// Resource that allows its file to be updated at run-time, and will reload if neccessary
    /// </summary>
    public class DevelopmentResource<T> : IGameResource<T>
    {
        private TypedResource<T> mBaseResource;
        private DateTime mFileUpdated;

        public GamePath Path { get { return mBaseResource.Path; } }

        public DevelopmentResource(TypedResource<T> baseResource)
        {
            mBaseResource = baseResource;
            mFileUpdated = File.GetLastWriteTime(this.Path.FullPath);
        }


        public T GetObject(GameContext context)
        {
            if (File.GetLastWriteTime(this.Path.FullPath) > mFileUpdated)
            {
                mBaseResource.Unload();
                mFileUpdated = File.GetLastWriteTime(this.Path.FullPath);          
            }

            return mBaseResource.GetObject(context);
        }


    }
}
