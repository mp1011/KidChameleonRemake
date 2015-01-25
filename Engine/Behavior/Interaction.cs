using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public interface IInteraction
    {
        bool Register(object o);
    }

    public abstract class Interaction<T1, T2> : IInteraction
    {
        private T1 mBehavior1;
        private T2 mBehavior2;

        
        public void RegisterController(T1 ctl)
        {
            mBehavior1 = ctl;
            if(mBehavior1 != null && mBehavior2 != null)
                DoAction(mBehavior1,mBehavior2);
        }

        public void RegisterController(T2 ctl)
        {
            mBehavior2 = ctl;
            if(mBehavior1 != null && mBehavior2 != null)
                DoAction(mBehavior1,mBehavior2);
        }

        protected abstract void DoAction(T1 controller1, T2 controller2);

        public bool Register(object o)
        {
            if (mBehavior1 == null && typeof(T1).IsAssignableFrom(o.GetType()))
            {
                RegisterController((T1)o);
                return true;
            }
            if (typeof(T2).IsAssignableFrom(o.GetType()))
            {
                RegisterController((T2)o);
                return true;
            }

            return false;
        }

    }
}
