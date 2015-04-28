using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{

    interface IGlobalAction
    {
        void DoAction(GameContext ctx);
    }

    class GlobalActionHandler : ITriggerable<IGlobalAction>
    {
        private World mWorld;

        public static void Create(World w)
        {
            var gah = new GlobalActionHandler();
            gah.mWorld = w;
            var buttonListener = new ButtonListener<IGlobalAction>(w, gah);
            buttonListener.AddMapping(KCButton.Pause, new PauseAction());
        }

        public int ID
        {
            get { throw new NotImplementedException(); }
        }

        public bool Triggered
        {
            get { return false; }
        }

        public void Trigger(IGlobalAction arg)
        {
            arg.DoAction(mWorld.Context);
        }

    }


}
