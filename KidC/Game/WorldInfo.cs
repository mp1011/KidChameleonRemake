using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{

    enum KCSceneType
    {
        Normal,
        SplashScreen
    }

    class KCWorldInfo : WorldInfo
    {
        public LevelTheme Theme { get; set; }
        public KCSceneType SceneType { get; set; }

        private ILevelTheme CreateTheme()
        {
            return ReflectionHelper.CreateObjectByAttribute<ILevelTheme, ThemeAttribute>(this, t => t.Theme == this.Theme);
        }

        public int SpeedBonusSeconds {get; set;}
        public int PathBonusAmount { get; set; }

        private List<BonusTracker> mBonusTrackers;
        public IEnumerable<BonusTracker> BonusTrackers { get { return mBonusTrackers.AsEnumerable(); } }

        protected override string TilesetName
        {
            get 
            {
                switch (this.Theme)
                {
                    case LevelTheme.Woods: return "woods";
                    default: return null;
                }
            }
        }

        public override World CreateWorld(GameContext context)
        {
            var world = new World(context,this);
            world.BackgroundColor = this.BackgroundColor;

            var levelTheme = this.CreateTheme();
            foreach (var layer in levelTheme.CreateLayers(world, this.Maps))
                world.AddLayer(layer);

            var foreground = world.GetLayers(LayerDepth.Foreground).FirstOrDefault();
            foreach (var entry in this.Objects)
                entry.AddToLayer(foreground);

            if (this.SceneType == KCSceneType.Normal)
            {
                GlobalActionHandler.Create(world);
                context.GetStats().RestoreHealth();

                mBonusTrackers = new List<BonusTracker>();
                mBonusTrackers.Add(new SpeedBonus(world));
                mBonusTrackers.Add(new PathBonus(world));
                mBonusTrackers.Add(new NoPrizeBonus(world));
                mBonusTrackers.Add(new NoHitBonus(world));
                mBonusTrackers.Add(new TimeBonus(world));                
            }

            return world;
        }
    }

   
}
