using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.XNA
{
    public class XNAEngine : EngineBase 
    {

        private XNAGame mGame;
        private GraphicsDeviceManager mGraphics;
    

        public XNAEngine()
        {
            mGame = new XNAGame(this);
            mGraphics = new GraphicsDeviceManager(mGame);
            mGraphics.SynchronizeWithVerticalRetrace = true;
            mGraphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            mGraphics.IsFullScreen = false;
    
        }

        private ISoundManager mSoundManager = new XNASoundManager();

        public override ISoundManager SoundManager
        {
            get { return mSoundManager; }
        }

        public override IGameInputDevice CreateInputDevice(GameContext context)
        {
            return new Engine.XNA.XNAKeyboardInput(context);
        }

        protected override int SetFPS(int desiredFPS)
        {
            mGame.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1000 / desiredFPS);
            return (int)(1000 / mGame.TargetElapsedTime.TotalMilliseconds);
        }

        protected override RGSizeI SetWindowSize(RGSizeI desiredWindowSize)
        {
            mGraphics.PreferredBackBufferWidth = desiredWindowSize.Width;
            mGraphics.PreferredBackBufferHeight = desiredWindowSize.Height;
            mGame.OnSizeChanged();
            return desiredWindowSize;
        }

        protected override RGSizeI SetGameSize(RGSizeI desiredGameSize)
        {
            mGame.OnSizeChanged();
            return desiredGameSize;
        }

        protected override RGRectangleI WindowLocation
        {
            get { return mGame.WindowLocation; }
        }

        protected override Graphics.Painter CreatePainter()
        {
            return new SpriteBatchPainter(this.Context, mGame.SpriteBatch, mGame.GraphicsDevice);

        }

        protected override void StartGame()
        {
            mGame.Run();
            mGame.Dispose();
            mGame = null;
        }
    }
}
