using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class PlayerDieController : SpriteBehavior
    {
        public PlayerDieController(Sprite s) : base(s) { }


        private Clock mClock;

        protected override void OnEntrance()
        {
            mClock = this.Context.CurrentMapHUD().Clock;
        }

        protected override void Update()
        {
            if (mClock.TimesUp)
                this.Sprite.Kill(Engine.ExitCode.Destroyed);
        }

        protected override void OnExit()
        {
            if (this.ExitCode == Engine.ExitCode.Destroyed)
            {
                Context.GetStats().Lives--;
                SoundManager.PlaySound(Sounds.PlayerDie);
                var fallingPlayer = Presets.Debris.Create(this.Sprite, this.Sprite.GetAnimation(KCAnimation.Dead));

                var next = new SplashScreenInfo(new InMemoryResource<WorldInfo>(this.Context.CurrentWorld.WorldInfo));

                ILogicObject action;

                if (mClock.TimesUp)
                    action = fallingPlayer.ContinueWith(new DelayWaiter(this.Context.CurrentWorld, 1.0f)).ContinueWith(new PlaySound(Sounds.NoTime));
                else
                    action = fallingPlayer.ContinueWith(new DelayWaiter(this.Context.CurrentWorld, 2.0f));


                if(Context.GetStats().Lives <= 0)
                    action.ContinueWith(new GameOver(this.Context.CurrentWorld));
                else
                    action.ContinueWith(new FadeoutSceneTransition(this.Context.CurrentWorld, next));

                this.Context.SetCameraCenter(null);
            }
        }


    }

    class GameOver : LogicObject
    {
        private World mWorld;
        private GameText mContinuesText;
        private GameText mContinuesNum;
        private GameText mScoreText;

        public GameOver(World w) : base(LogicPriority.World, w) 
        {
            mWorld = w;
        }

        protected override void OnEntrance()
        {
            var font = FontManager.GetBigFont(this.Context);

            mContinuesText = new GameText(this, font, "continues", new RGPointI(0, 100), Context.ScreenLocation.Width, Alignment.Center, Alignment.Near);
            mContinuesNum = new GameText(this, font, "2", new RGPointI(0, 120), Context.ScreenLocation.Width, Alignment.Center, Alignment.Near);
            mScoreText =new GameText(this, font, "score    2600", new RGPointI(0, 220), Context.ScreenLocation.Width, Alignment.Center, Alignment.Near);
          
            mWorld.ScreenLayer.AddObject(mContinuesText);
            mWorld.ScreenLayer.AddObject(mContinuesNum);
            mWorld.ScreenLayer.AddObject(mScoreText);

            foreach (var layer in mWorld.GetLayersExceptScreen())
                layer.Pause();

            var action = new FadeBG(this.mWorld, RGColor.FromRGB(100, 100, 100), 30)
            .ContinueWithMulti(
                MovingText.MoveIn(mContinuesText, mWorld.ScreenLayer, Direction.Left),
                MovingText.MoveIn(mContinuesNum, mWorld.ScreenLayer, Direction.Right),
                MovingText.MoveIn(mScoreText, mWorld.ScreenLayer, Direction.Down))         
            .ContinueWith(new DelayWaiter(this, 5.0f))
            .ContinueWithMulti(
                MovingText.MoveOut(mContinuesText,mWorld.ScreenLayer, Direction.Right),
                MovingText.MoveOut(mContinuesNum, mWorld.ScreenLayer, Direction.Right))
               .ContinueWith(new FadeoutSceneTransition(this.Context.CurrentWorld, new WorldInfo()));

        }

    }

  
}
