using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class HUD : FixedLayer 
    {
        private PlayerStats mStats;

        public Clock Clock { get; private set; }
        public Counter LivesCounter { get; private set; }
        public Counter GemsCounter { get; private set; }
        public HealthGuage HealthGuage { get; private set; }

        private GameText mDebugMessage;

        public HUD(World world)
            : base(world, LayerDepth.Screen)
        {

        }

        protected override void OnEntrance()
        {

            this.mStats = this.Context.GetStats();

            this.Clock = new Clock(this);
            this.LivesCounter = Counter.CreateLivesCounter(this);
            this.GemsCounter = Counter.CreateGemsCounter(this);
            this.HealthGuage = new HealthGuage(this);

            this.AddObject(this.Clock);
            this.AddObject(LivesCounter);
            this.AddObject(GemsCounter);
            this.AddObject(HealthGuage);

            this.Clock.Location = new RGPointI(20, 20);
            this.LivesCounter.Location = new RGPointI(304, 16);
            this.GemsCounter.Location = new RGPointI(304, 36);
            this.HealthGuage.Location = new RGPointI(23,32);

            mDebugMessage = new GameText(this, FontManager.ScoreFont, "", new RGPointI(50,100), 100, Alignment.Near, Alignment.Near);
            this.AddObject(mDebugMessage);
        }

        protected override void UpdateEx()
        {
            this.GemsCounter.Amount = mStats.Gems;
            this.LivesCounter.Amount = mStats.Lives;
            this.HealthGuage.CurrentHealth = mStats.CurrentHealth;
            this.HealthGuage.MaxHealth = mStats.MaxHealth;

            var debugText = Context.DebugNumbers.Where(p => p != null && p.HasValue).Select(p => p.Value.ToString("0.0000")).ToArray().StringJoin(" ");
            mDebugMessage.Text = debugText;
        }
    }

    static class HudExtensions
    {
        public static HUD CurrentMapHUD(this GameContext ctx)
        {
            return ctx.CurrentWorld.GetLayers(LayerDepth.Screen).OfType<HUD>().FirstOrDefault();
        }
    }
}
