using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Diagnostic
{
    public abstract class ErrorTracker : LogicObject
    {
        public static List<ErrorTracker> mAllTrackers = new List<ErrorTracker>();

        protected ErrorTracker(ILogicObject owner)
            : base(LogicPriority.World, owner)
        {
            mAllTrackers.Add(this);
        }

        protected override void Update()
        {
            CheckErrorStateNow();
        }

        public abstract void CheckErrorStateNow();
        public abstract void RecordWatchValue();

        public static void CheckForErrors()
        {
            foreach (var tracker in mAllTrackers)
                tracker.CheckErrorStateNow();
        }

        public static void RecordWatchValues()
        {
            foreach (var tracker in mAllTrackers)
                tracker.RecordWatchValue();
        }
    }

    
    public class ErrorTracker<T,K> : ErrorTracker where T:ILogicObject 
    {
        class WatchValue
        {
            public ulong Frame { get;set;}
            public K Value { get;set;}

            public override string ToString()
            {
 	            return Frame.ToString() + ": " + Value.ToString();
            }
        }

        private Func<T, K> mWatch;
        private Predicate<K> mErrorCondition;
        private K mWatchValue;

        private List<WatchValue> mWatchValues = new List<WatchValue>();

        private WatchValue[] GetRecentWatchValues(ulong framesBack)
        {
            return mWatchValues.Where(p => p.Frame > Context.CurrentFrameNumber - framesBack).ToArray();
        }

        private T mObject;
        private ulong mErrorFrame = 0;

        public ErrorTracker(T obj, Func<T,K> watch, Predicate<K> errorCondition) : base(obj)
        {
            mWatch = watch;
            mErrorCondition = errorCondition;
            mObject = obj;
        }

        public override void RecordWatchValue()
        {
            mWatchValues.Add(new WatchValue { Frame = Context.CurrentFrameNumber, Value = mWatch(mObject) });
        }

        public override void CheckErrorStateNow()
        {
            mWatchValue = mWatch(mObject);
            if (mErrorFrame == 0 && mErrorCondition(mWatchValue))            
                mErrorFrame = Context.CurrentFrameNumber;            
        }
    }

}
