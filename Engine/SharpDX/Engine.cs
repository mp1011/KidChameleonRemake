using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.SharpDX
{
    public class SharpDXEngine : EngineBase 
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

        protected override Graphics.Painter CreatePainter()
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


}
