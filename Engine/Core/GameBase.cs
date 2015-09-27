using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Core
{

    public abstract class GameBase  
    {
        public static GameBase Current { get; private set; }

     
        protected GameBase() 
        {
            GameBase.Current = this;
            this.SpriteSheetResources = new ResourceCache<SpriteSheet>(PathType.SpriteSheets);
        }

#region Abstract Properties and Methods      
        public abstract ObjectTypeRelations Relations { get; }
        public abstract Func<TileInstance> TileInstanceCreate { get; }
        public abstract Func<WorldInfo> WorldInfoCreate { get; }

        public abstract Func<EngineBase, GameBase, GameContext> GameContextCreate { get; }
        public abstract GameResource<WorldInfo> StartingWorld { get; }
       
        public abstract void OnStartup();
     //   public abstract void OnShutdown();
#endregion

        #region Resources

        public ResourceCache<SpriteSheet> SpriteSheetResources { get; private set; }

        #endregion
    }


    public class GlobalToken : ILogicObject
    {
        private GlobalToken() { }

        private static GlobalToken mInstance;
        public static GlobalToken Instance
        {
            get
            {
                return mInstance ?? (mInstance = new GlobalToken());
            }
        }

        public bool Alive
        {
            get { return true; }
        }

        public bool Paused
        {
            get;
            set;
        }

        public ExitCode ExitCode
        {
            get { return Engine.ExitCode.StillAlive; }
        }

        public void Kill(ExitCode exitCode)
        {
        }

        public GameContext Context
        {
            get { return EngineBase.Current.Context; }
        }
    }
     
}
