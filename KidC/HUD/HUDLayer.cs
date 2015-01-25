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

        public HUD(GameContext ctx) : base(ctx, LayerDepth.Screen)
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

            this.Clock.Location = new RGPoint(20, 20);
            this.LivesCounter.Location = new RGPoint(304, 16);
            this.GemsCounter.Location = new RGPoint(304, 36);
            this.HealthGuage.Location = new RGPoint(23,32);

            mDebugMessage = new GameText(this.Context, FontManager.ScoreFont, "", new RGPoint(50,100), 100, Alignment.Near, Alignment.Near);
            this.AddObject(mDebugMessage);
        }

        protected override void UpdateEx()
        {
            this.GemsCounter.Amount = mStats.Gems;
            this.HealthGuage.CurrentHealth = mStats.CurrentHealth;
            this.HealthGuage.MaxHealth = mStats.MaxHealth;

            if (Context.DebugNumber1.HasValue)
                mDebugMessage.Text = this.Context.DebugNumber1.Value.ToString("0.0000");
            else
                mDebugMessage.Text = "";
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
