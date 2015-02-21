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
    public class DevelopmentResource<T> : GameResource<T> where T:new ()
    {
        private DateTime mFileUpdated;

        public DevelopmentResource(GamePath path) : base(path) 
        {
            mFileUpdated = File.GetLastWriteTime(this.Path.FullPath);
        }


        protected override bool NeedsLoad()
        {
            return base.NeedsLoad() || File.GetLastWriteTime(this.Path.FullPath) > mFileUpdated;
        }

        protected override T CreateNewObject(GameContext context)
        {
            mFileUpdated = File.GetLastWriteTime(this.Path.FullPath);
            return base.CreateNewObject(context);
        }
    }
}
