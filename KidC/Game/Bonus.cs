using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{

    static class BonusTrackerUtil
    {
        public static IEnumerable<BonusTracker> GetBonusTrackers(this GameContext context)
        {
            if (context.CurrentWorld == null)
                return new BonusTracker[] { };

            var worldInfo = context.CurrentWorld.WorldInfo as KCWorldInfo;
            return worldInfo.BonusTrackers;
        }
    }

    abstract class BonusTracker
    {

        protected GameContext Context { get; private set; }
        protected KCWorldInfo CurrentWorldInfo { get; private set;}
        public abstract string Name { get; }
        public abstract int GetCurrentValue();

        protected BonusTracker (World world) 
        {
            this.CurrentWorldInfo = world.WorldInfo as KCWorldInfo;
            this.Context = world.Context;
        }
    }

    class TimeBonus : BonusTracker
    {
        public TimeBonus(World w) : base(w) { }

        public override string Name
        {
            get { return "time bonus"; }

        }


         public override int GetCurrentValue()
        {
            var secondsOnClock =this.Context.CurrentMapHUD().Clock.SecondsRemaining;
            var minutes =  secondsOnClock / 60;
            var seconds = secondsOnClock% 60;
            return (minutes * 1000) + (seconds * 10);
        }
    }

    class SpeedBonus: BonusTracker
    {
        public SpeedBonus(World w) : base(w) { }

        public override string Name
        {
            get { return "speed bonus"; }
        }

        public override int GetCurrentValue()
        {
            if (this.Context.CurrentMapHUD().Clock.SecondsRemaining <= this.CurrentWorldInfo.SpeedBonusSeconds)
                return 10000;
            else
                return 0;                                                                                      
        }

    }

    class PathBonus : BonusTracker
    {
        public PathBonus(World w) : base(w) { }

        public override string Name
        {
            get { return "path bonus"; }
        }

        public override int GetCurrentValue()
        {
            return this.CurrentWorldInfo.PathBonusAmount;                                                                            
        }

    }

    class NoPrizeBonus : BonusTracker
    {
        public NoPrizeBonus(World w) : base(w) { }

        private bool mRejected;
        public void Reject()
        {
            mRejected = true;
        }

        public override string Name
        {
            get { return "\"no prize\" bonus"; }
        }

        public override int GetCurrentValue()
        {
            return mRejected ? 0 : 5000;                                                                  
        }
    }

    class NoHitBonus : BonusTracker
    {
        public NoHitBonus(World w) : base(w) { }

        private bool mRejected;
        public void Reject()
        {
            mRejected = true;
        }

        public override string Name
        {
            get { return "\"no hit\" bonus"; }
        }

        public override int GetCurrentValue()
        {
            return mRejected ? 0 : 5000;                                                          
        }
    }
}
