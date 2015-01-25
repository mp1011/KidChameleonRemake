using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class SimpleDirectionController : SpriteBehavior
    {
        private Player mPlayer;

        public SimpleDirectionController(Sprite s, Player p) : base(s) { mPlayer = p; }

        protected override void OnEntrance()
        {

        }

        protected override void Update()
        {
            if (mPlayer.Input.InputDirection(Orientation.None).HasValue)
            {
                this.Sprite.MotionManager.Direction = mPlayer.Input.InputDirection(Orientation.None).Value;
                this.Sprite.MotionManager.MainMotion.Direction = this.Sprite.MotionManager.Direction;
            }
            else
            {
; 
            }
        }
    
    }
}
