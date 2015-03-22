using Engine;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Editor
{
    static class Program
    {
        public static GameBase EditorGame { get; private set; }
        public static GameContext EditorContext { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            TypeDescriptor.AddAttributes(typeof(RGColor), new EditorAttribute(typeof(RGColorEditor),typeof(System.Drawing.Design.UITypeEditor)));
            TypeDescriptor.AddAttributes(typeof(TileFlags), new EditorAttribute(typeof(FlagEnumEditor.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor)));
            TypeDescriptor.AddAttributes(typeof(EditorDirectionFlags), new EditorAttribute(typeof(FlagEnumEditor.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor)));
  
         
            var engine = new EditorEngine();
            Program.EditorGame = new KidC.KidCGame();
            Program.EditorContext = Program.EditorGame.GameContextCreate(engine, Program.EditorGame);
            engine.Run(EditorGame);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Editor.Forms.Main());
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
            return desiredWindowSize;
        }

        protected override RGSizeI SetGameSize(RGSizeI desiredGameSize)
        {
            return desiredGameSize;
        }

        protected override RGRectangleI WindowLocation
        {
            get { return RGRectangleI.Empty; }
        }

        protected override Engine.Graphics.Painter CreatePainter()
        {
            throw new NotImplementedException();
        }

        public override IGameInputDevice CreateInputDevice(GameContext context)
        {
            return new NullInputDevice();
        }

        public override ISoundManager SoundManager
        {
            get { throw new NotImplementedException(); }
        }

        protected override void StartGame()
        {
        }
    }

    class EditorContext : Engine.GameContext
    {
        public EditorContext(Engine.Core.EngineBase engine, Engine.Core.GameBase game) : base(engine, game) { }
    }

}
