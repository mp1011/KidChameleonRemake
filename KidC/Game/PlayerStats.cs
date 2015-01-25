using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class PlayerStats
    {
        public const bool InfiniteHealth=true;

        private int mGems;
        public int Gems
        {
            get { return mGems; }
            set { mGems = value.LimitNumber(99);}
        }

        private int mMaxHealth;
        public int MaxHealth
        {
            get { return mMaxHealth; }
            set { mMaxHealth = value.LimitNumber(6); }
        }

        private int mCurrentHealth;
        public int CurrentHealth
        {
            get { return InfiniteHealth ? 8 : mCurrentHealth; }
            set { mCurrentHealth = value.LimitNumber(this.MaxHealth); }
        }


        
    }

    static class PlayerStatsExtensions
    {
        public static PlayerStats GetStats(this GameContext ctx)
        {
            return (ctx as KCContext).Stats;
        }
    }
}
