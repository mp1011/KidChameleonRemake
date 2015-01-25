using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class TimedAction<T> : LogicObject where T:LogicObject
    {

        #region Static

        public static void DelayedAction(Action<T> act, T obj, ulong delay)
        {
            var action = new TimedAction<T>(obj, act);
            action.mActionStart = obj.Context.CurrentFrameNumber + delay;        
        }

        #endregion

        public ulong Duration { get; set; }
        private ulong mActionStart { get; set; }

        private Action<T> mAction;
        private T mObject;

        public TimedAction(T obj, Action<T> act) : base(LogicPriority.Behavior,obj, RelationFlags.DestroyWhenParentDestroyed)
        {
            mAction = act;
            mObject = obj;
        }

        protected override void Update()
        {
            if (mActionStart == 0)
                return;

            if (this.Context.CurrentFrameNumber >= mActionStart && this.Context.CurrentFrameNumber <= (mActionStart + Duration))
                mAction(mObject);
        }

        public void Start()
        {
            mActionStart = this.Context.CurrentFrameNumber;
        }


        public void Start(ulong duration)
        {
            this.Duration = duration;
            mActionStart = this.Context.CurrentFrameNumber;
        }

        public void Stop()
        {
            mActionStart = 0;
        }
    }
}
