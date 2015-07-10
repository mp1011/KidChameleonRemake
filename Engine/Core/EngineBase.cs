using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Graphics;

namespace Engine.Core
{
    public abstract class EngineBase
    {
        public static EngineBase Current { get; private set; }


        protected EngineBase()
        {
            EngineBase.Current = this;
        }

  
        #region Abstract Properties and Methods
      
        protected abstract int SetFPS(int desiredFPS);
        protected abstract RGSizeI SetWindowSize(RGSizeI desiredWindowSize);
        protected abstract RGSizeI SetGameSize(RGSizeI desiredGameSize);

        protected abstract RGRectangleI WindowLocation { get; }
             
        protected abstract Painter CreatePainter();
        public abstract IGameInputDevice CreateInputDevice(Engine.GameContext context);
        public abstract ISoundManager SoundManager { get; }

        protected abstract void StartGame();  

        #endregion

        #region Properties

        private RGSizeI mWindowSize, mGameSize;

        public RGSizeI WindowSize { get { return mWindowSize; } set { mWindowSize = value; mWindowSize = SetWindowSize(value); } }
        public RGSizeI GameSize { get { return mGameSize; } set { mGameSize = value; mGameSize = SetGameSize(value); } }

        public GameContext Context { get; private set; }

        private int m_fps = 60;
        public int FPS
        {
            get { return m_fps; }
            set
            {
                if (value >= 10 && value <= 300)
                    m_fps = SetFPS(value);
            }
        }


        #endregion

        public void Run(GameBase game)
        {
            WindowSize = new RGSizeI(1024, 768);
            GameSize = new RGSizeI(320, 240);        
            this.Context = game.GameContextCreate(this,game);
            this.Context.SetWorld(game.StartingWorld.GetObject(this.Context));
            game.OnStartup();
            this.StartGame();            
        }

        #region Update Loop

        public void UpdateFrame()
        {
            this.Context.WindowLocation = this.WindowLocation;
            this.Context.Update();
        }

        public void DrawFrame()
        {
            var painter = this.CreatePainter();
            painter.RenderInfo = this.Context.RenderInfo;
            painter.Paint(this.Context.ScreenLocation, this.Context.CurrentWorld);
        }

        #endregion
    
    
    }



}
