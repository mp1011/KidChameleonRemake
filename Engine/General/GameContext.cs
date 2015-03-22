using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public abstract class GameContext
    {

        
        protected GameContext(Core.EngineBase engine, Core.GameBase game)
        {
            this.Engine = engine;
            this.Game = game;
            this.RenderInfo = RenderOptions.Normal;

            mCameraCenter = new WorldPoint(this, 200, 200);
            this.mLoopManager = new LoopManager();
            this.Timer = new Timer(this);
            this.Listeners = new Listeners(this);
        }


        public RenderOptions RenderInfo { get; private set; }

        private LoopManager mLoopManager;

        public ulong CurrentFrameNumber { get; private set; }

        private List<Player> mPlayers = new List<Player>();
        public Listeners Listeners { get; private set; }

        public World CurrentWorld { get; private set; }

        public void SetWorld(World w)
        {
            CurrentWorld = w;
        }

        public void SetWorld(WorldInfo w)
        {
            CurrentWorld = w.CreateWorld(this);
        }

        public Input.MouseInput Mouse { get; set; }

        public Timer Timer { get; private set; }

        private IWithPosition mCameraCenter;

        public RGRectangleI WindowLocation { get; set; }

        public Core.GameBase Game { get; private set;}
        public Core.EngineBase Engine { get; private set; }

        public int FPS { get { return Engine.FPS; } set { Engine.FPS = value; } }

        public RGRectangleI ScreenLocation
        {
            get
            {
                var size = Engine.GameSize;
                var Screen = RGRectangleI.FromXYWH(mCameraCenter.Location.X - (size.Width / 2f), mCameraCenter.Location.Y - (size.Height / 2f), size.Width, size.Height);

                if (CurrentWorld == null)
                    return Screen;

                if (Screen.Left < 0)
                    Screen = RGRectangleI.FromXYWH(0, Screen.Y, size.Width, size.Height);

                if (Screen.Top < 0)
                    Screen = RGRectangleI.FromXYWH(Screen.X, 0, size.Width, size.Height);

                if (Screen.Right > CurrentWorld.Area.Right)
                    Screen = RGRectangleI.FromXYWH(CurrentWorld.Area.Right - Screen.Width, Screen.Y, size.Width, size.Height);

                if (Screen.Bottom > CurrentWorld.Area.Bottom)
                    Screen = RGRectangleI.FromXYWH(Screen.X, CurrentWorld.Area.Bottom - size.Height, size.Width, size.Height);

                return Screen;
            }
        }

        public void CenterCamera()
        {
            mCameraCenter = new WorldPoint(this, this.Engine.GameSize.Width / 2, this.Engine.GameSize.Height / 2);
        }
        
        public void SetCameraCenter(IWithPosition obj)
        {
            if (obj == null)
            {
                if (mCameraCenter == null)
                    mCameraCenter = new WorldPoint(this, this.Engine.GameSize.Width / 2, this.Engine.GameSize.Height / 2);
                else
                    mCameraCenter = new WorldPoint(this, mCameraCenter.Location.X, mCameraCenter.Location.Y);
            }
            else 
                mCameraCenter = obj;
        }

        public void RegisterPlayer(Player player)
        {
            mPlayers.Add(player);
        }

        public Player FirstPlayer
        {
            get { return mPlayers[0]; }
        }

        public double?[] DebugNumbers = new double?[3];
      
        public void Update()
        {
            CurrentFrameNumber++;
            mLoopManager.Update();

        }

        public void AddObject(LogicObject o)
        {
            mLoopManager.AddObject(o);
        }

        public int ElapsedSecondsSince(ulong startFrame)
        {
            var elapsedFrames = this.CurrentFrameNumber - startFrame;
            return (int)(elapsedFrames / (ulong)FPS);
        }

        public int ElapsedFramesSince(ulong startFrame)
        {
            return (int)(CurrentFrameNumber - startFrame);
        }

        public bool DebugKeyDown()
        {
            return this.FirstPlayer.Input.KeyDown(GameKey.Editor1);
        }
    }

}
