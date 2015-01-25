using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Input;

namespace Engine
{
    public class Player
    {

        public IGameInputDevice Input { get; private set; }

        public Player(GameContext ctx, IGameInputDevice input)
        {
            ctx.RegisterPlayer(this);
            this.Input = input;
        }


    }
}
