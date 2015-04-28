using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{

    enum PauseOption
    {
        ResumePlay = 0,
        RestartRound = 1,
        Quit = 2
    }

    class PauseAction : IGlobalAction
    {
        private bool mIsPaused;
        private RGColor mOriginalBGColor;
        private PauseBox mPauseBox;
        private ulong mUnpauseFrame;

        public void DoAction(GameContext ctx)
        {
            if (!mIsPaused && ctx.ElapsedFramesSince(mUnpauseFrame) > 8)
                Pause(ctx);
        }

        public void Pause(GameContext ctx)
        {
            mOriginalBGColor = ctx.CurrentWorld.BackgroundColor;
            ctx.CurrentWorld.BackgroundColor = ctx.CurrentWorld.BackgroundColor.Fade(RGColor.Black, .5f);

            mIsPaused = true;
            foreach (var layer in ctx.CurrentWorld.GetLayersExceptScreen())
            {
                layer.ExtraRenderInfo.FadeColor = RGColor.FromRGB(70, 70, 70);
                layer.Pause();
            }

            mPauseBox = new PauseBox(ctx,this);        
            ctx.CurrentWorld.ScreenLayer.AddObject(mPauseBox);
        }

        public void Unpause(GameContext ctx)
        {
            ctx.CurrentWorld.BackgroundColor = mOriginalBGColor;
            mIsPaused = false;
            foreach (var layer in ctx.CurrentWorld.GetLayersExceptScreen())
            {
                
                layer.ExtraRenderInfo.FadeColor = RGColor.White;
                layer.Resume();
            }

            mPauseBox.Hide();
            mPauseBox = null;
            mUnpauseFrame = ctx.CurrentFrameNumber;
        }


    }

    class PauseBox : IDrawableRemovable, ITriggerable<PauseOption>
    {
        private InterfaceRectangle mRectangle;
        private TextMenu<PauseOption> mPauseMenu;
        private PauseAction mPauseAction;
        private GameContext mContext;

        public PauseBox(GameContext ctx, PauseAction pauseAction)        
        {
            this.mContext = ctx;
            this.Alive = true;
            mRectangle = new InterfaceRectangle(ctx, new TextureResource("pausemenufont"), new RGSizeI(8, 8), new RGPointI(8, 0));
            mRectangle.AddBorderPart(new RGPointI(6, 0), Direction.UpLeft);
            mRectangle.AddBorderPart(new RGPointI(7, 0), Direction.Up);
            mRectangle.AddBorderPart(new RGPointI(11, 0), Direction.Left);
            mRectangle.SizeInCells = new RGSizeI(16, 5);
            mRectangle.Location = mRectangle.Area.CenterWithin(ctx.ScreenLocation.Size).TopLeft;

            mPauseMenu = new TextMenu<PauseOption>(ctx, FontManager.PauseMenuFont, this, mRectangle);
            mPauseMenu.AddOption("resume play", PauseOption.ResumePlay);

            if(ctx.GetStats().Lives > 1)
                mPauseMenu.AddOption("restart round", PauseOption.RestartRound);
            else
                mPauseMenu.AddOption("give up", PauseOption.Quit);

            mPauseAction = pauseAction;
        }

        public bool Alive
        {
            get;
            private set;
        }

        public void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            mRectangle.Draw(p, canvas);
            mPauseMenu.Draw(p, canvas);
        }

        public void Hide()
        {
            if (this.Alive)
            {
                this.Alive = false;
                mPauseMenu.Kill(ExitCode.Removed);
                mPauseAction.Unpause(mContext);
            }
        }

        void ITriggerable<PauseOption>.Trigger(PauseOption arg)
        {
            this.Hide();

            if (arg == PauseOption.RestartRound)
            {
                mContext.GetStats().Lives--;
                SceneTransition.RestartRound(this.mContext);
            }
            else if (arg == PauseOption.Quit)
                SceneTransition.RestartGame(this.mContext);

            mPauseAction.DoAction(this.mContext);
        }

        int ITriggerable.ID
        {
            get { return 0; }
        }

        bool ITriggerable.Triggered
        {
            get { return false; }
        }
    }
}
