using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class PlayerStats 
    {
        public const bool InfiniteHealth=false;

        public PlayerStats()
        {
            this.Lives = 3;
        }

        private int mGems;
        public int Gems
        {
            get { return mGems; }
            set { mGems = value.LimitNumber(99);}
        }

        private int mLives;
        public int Lives
        {
            get { return mLives; }
            set { mLives = value.LimitNumber(99); }
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

        public void RestoreHealth()
        {
            this.CurrentHealth = this.MaxHealth;
        }
        
    }

    static class PlayerStatsExtensions
    {
        public static PlayerStats GetStats(this GameContext ctx)
        {
            return (ctx as KCContext).Stats;
        }
    }

    enum StatType
    {
        Gems,
        TimeRemaining,
        Lives,
        Continues,
        Score
    }

    class AdjustStat : Engine.Action
    {
        public StatType Stat { get; private set; }
        public int Change { get; private set; }

        public AdjustStat(ILogicObject owner, StatType stat, int change)
            : base(owner)
        {
            this.Stat = stat;
            this.Change = change;
        }

        protected override ExitCode DoAction()
        {
            switch (Stat)
            {
                case StatType.Gems:
                    Context.GetStats().Gems += Change;
                    return Engine.ExitCode.Finished;
                case StatType.TimeRemaining:
                    Context.CurrentMapHUD().Clock.AdjustSeconds(Change);
                    return Engine.ExitCode.Finished;
                default:
                    throw new NotImplementedException();
            }
        }
 
    }
}
