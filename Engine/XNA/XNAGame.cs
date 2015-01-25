using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Engine.XNA
{
    public class XNAGame : Microsoft.Xna.Framework.Game
    {

        private XNAEngine mEngine;

        #region Startup

        public XNAGame(XNAEngine engine)
        {
            this.mEngine = engine;
            this.IsFixedTimeStep = true;
                                   
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            Content.RootDirectory = "Content";            
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            mEngine.WindowSize = new RGSizeI(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);
            this.OnSizeChanged();
        }

        protected override void LoadContent()
        {
            this.SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        #endregion

        protected override void Update(GameTime gameTime)
        {
            mEngine.UpdateFrame();
            base.Update(gameTime);
        }

        #region Drawing

        private RenderTarget2D mRenderTarget;

        public SpriteBatch SpriteBatch { get; private set; }

        protected override void Draw(GameTime gameTime)
        {
            if (mRenderTarget == null)
            {
                mRenderTarget = new RenderTarget2D(this.GraphicsDevice, this.mEngine.GameSize.Width, this.mEngine.GameSize.Height);
                mRenderTarget.ContentLost += new EventHandler<EventArgs>(mRenderTarget_ContentLost);
            }

            this.GraphicsDevice.SetRenderTarget(mRenderTarget);

            try
            {
                GraphicsDevice.Clear(mEngine.Context.CurrentWorld.BackgroundColor.ToXNAColor());
                this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                mEngine.DrawFrame();
            }
            finally
            {
                this.SpriteBatch.End();
            }

            try
            {
                Matrix m = Matrix.CreateScale(this.mWindowScale);

                this.GraphicsDevice.SetRenderTarget(null);
                this.GraphicsDevice.Clear(Color.Black);
                this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, m);
                this.SpriteBatch.Draw(mRenderTarget, mWindowLocation, Color.White);
            }
            finally
            {
                this.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private float mWindowScale;
        private Vector2 mWindowLocation;

        public void OnSizeChanged()
        {
            //find the largest size we can fit into the window
            var window = RGRectangleI.FromXYWH(0, 0, mEngine.WindowSize.Width, mEngine.WindowSize.Height);

            var aspectRatio = (float)mEngine.GameSize.Width / (float)mEngine.GameSize.Height;

            var destRec = RGRectangleI.FromXYWH(0, 0, mEngine.WindowSize.Width, mEngine.WindowSize.Width / aspectRatio);
            destRec = destRec.CenterWithin(window);

            if (!window.Contains(destRec))
            {
                destRec = RGRectangleI.FromXYWH(0, 0, mEngine.WindowSize.Height * aspectRatio, mEngine.WindowSize.Height);
                destRec = destRec.CenterWithin(window);
            }

            mWindowScale = (float)destRec.Width / (float)mEngine.GameSize.Width;
            mWindowLocation = new Vector2(destRec.X / mWindowScale, destRec.Y / mWindowScale);

        }

        public RGRectangleI WindowLocation { get { return RGRectangleI.FromXYWH(mWindowLocation.X, mWindowLocation.Y, mEngine.GameSize.Width, mEngine.GameSize.Height); } }

        void mRenderTarget_ContentLost(object sender, EventArgs e)
        {
            mRenderTarget = null;
        }

        #endregion

    }
}
