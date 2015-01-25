using Engine;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Editor
{
    static class Program
    {
        public static EditorGame EditorGame { get; private set; }
        public static GameContext EditorContext { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Program.EditorGame = new EditorGame();
            Program.EditorContext = Program.EditorGame.GameContextCreate(new EditorEngine(), Program.EditorGame);
    
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Editor.Forms.Main());
        }
    }

    public class EditorGame : GameBase    {

        public override ObjectTypeRelations Relations
        {
            get { throw new NotImplementedException(); }
        }

        public override Func<TileInstance> TileInstanceCreate
        {
            get { throw new NotImplementedException(); }
        }

        public override Func<EngineBase, GameBase, GameContext> GameContextCreate
        {
            get { throw new NotImplementedException(); }
        }

        public override GameResource<World> StartingWorld
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class EditorEngine : EngineBase
    {
        protected override int SetFPS(int desiredFPS)
        {
            throw new NotImplementedException();
        }

        protected override RGSizeI SetWindowSize(RGSizeI desiredWindowSize)
        {
            throw new NotImplementedException();
        }

        protected override RGSizeI SetGameSize(RGSizeI desiredGameSize)
        {
            throw new NotImplementedException();
        }

        protected override RGRectangleI WindowLocation
        {
            get { throw new NotImplementedException(); }
        }

        protected override Engine.Graphics.Painter CreatePainter()
        {
            throw new NotImplementedException();
        }

        public override IGameInputDevice CreateInputDevice(GameContext context)
        {
            throw new NotImplementedException();
        }

        public override ISoundManager SoundManager
        {
            get { throw new NotImplementedException(); }
        }

        protected override void StartGame()
        {
            throw new NotImplementedException();
        }
    }

    class EditorContext : Engine.GameContext
    {
        public EditorContext(Engine.Core.EngineBase engine, Engine.Core.GameBase game) : base(engine, game) { }
    }

}
