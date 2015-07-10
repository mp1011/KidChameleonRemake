using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class Lockable<T>
    {
        private ILogicObject mLockOwner;

        public T Value { get; private set;}

        public Lockable(T initialValue)
        {
            this.Value = initialValue;
        }

        public bool  SetValue(T newValue, ILogicObject o)
        {
            if (mLockOwner == null || mLockOwner == o)
            {
                this.Value = newValue;
                return true;
            }
            return false;
        }

        public void SetValue(T newValue)
        {
            if (mLockOwner == null)
                this.Value = newValue;
        }

        public void SetOwner(ILogicObject newOwner)
        {
            if (mLockOwner == null)
                mLockOwner = newOwner;
        }

        public void ClearOwner(ILogicObject currentOwner)
        {
            if (mLockOwner == currentOwner)
                mLockOwner = null;
        }
    }

    public static class LockableExtensions
    {

        public static void ClaimLock<T>(this ILogicObject obj, Lockable<T> lockedItem)
        {
            lockedItem.SetOwner(obj);
        }

        public static void ReleaseLock<T>(this ILogicObject obj, Lockable<T> lockedItem)
        {
            lockedItem.ClearOwner(obj);
        }

    }
}
