using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class ButtonListener<TriggerArg> : LogicObject 
    {
        private ITriggerable<TriggerArg> mTrigger;

        public ButtonListener(LogicObject owner, ITriggerable<TriggerArg> trigger)
            : base(LogicPriority.World, owner, RelationFlags.DestroyWhenParentDestroyed)
        {
            mTrigger = trigger;
        }

        public ButtonListener(World w, ITriggerable<TriggerArg> trigger)
            : base(LogicPriority.World, w)
        {
            mTrigger = trigger;
        }

        private Dictionary<int, TriggerArg> mButtonMappings = new Dictionary<int, TriggerArg>();
        public void AddMapping(int button, TriggerArg arg)
        {
            mButtonMappings.Add(button, arg);
        }

        public void AddMappings(TriggerArg arg, params int[] buttons)
        {
            foreach(int button in buttons)
                mButtonMappings.Add(button, arg);
        }

        protected override void Update()
        {
            if (Context.FirstPlayer == null)
                return;

            if (mTrigger.Triggered)
                return;

            foreach (int key in mButtonMappings.Keys)
            {
                if (Context.FirstPlayer.Input.KeyPressed(key))
                {
                    mTrigger.Trigger(mButtonMappings[key]);
                    return;
                }
            }
        }  
    }
}
