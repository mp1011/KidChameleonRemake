using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XKeys = Microsoft.Xna.Framework.Input.Keys;
using Microsoft.Xna.Framework.Input;
using Engine.Input;

namespace Engine.XNA
{
    public class XNAKeyboardInput : GenericInputDevice
    {
        public XNAKeyboardInput(GameContext ctx) : base(ctx) 
        {

            this.SetDirectionKeyMapping(GameKey.Left, (int)Keys.Left, Direction.Left);
            this.SetDirectionKeyMapping(GameKey.Right, (int)Keys.Right, Direction.Right);
            this.SetDirectionKeyMapping(GameKey.Up, (int)Keys.Up, Direction.Up);
            this.SetDirectionKeyMapping(GameKey.Down, (int)Keys.Down, Direction.Down);

            this.SetKeyMapping(GameKey.Button1, (int)Keys.A);         
            this.SetKeyMapping(GameKey.Button2, (int)Keys.S);
            this.SetKeyMapping(GameKey.Button3, (int)Keys.D);

            this.SetKeyMapping(GameKey.Editor1, (int)Keys.LeftControl);
            this.SetKeyMapping(GameKey.Editor2, (int)Keys.E);        
        }

        protected override IEnumerable<int> GetPressedKeys()
        {
            return Keyboard.GetState().GetPressedKeys().Select(p => (int)p).ToArray();
        }
    }

}
