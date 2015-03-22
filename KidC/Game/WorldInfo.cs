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
            var world = new World(context);
            world.BackgroundColor = this.BackgroundColor;

            var levelTheme = this.CreateTheme();
            foreach (var layer in levelTheme.CreateLayers(context, this.Map))
                world.AddLayer(layer);

            var foreground = world.GetLayers(LayerDepth.Foreground).FirstOrDefault();
            foreach (var entry in this.Objects)
                entry.AddToLayer(foreground);

            return world;
        }
    }
}
