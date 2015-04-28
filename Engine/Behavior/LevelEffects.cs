using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class FadeBG : LogicObject
    {
        private int mDuration;
        private RGColor mTargetColor;
        private World mWorld;

        public FadeBG(World world, RGColor targetColor, int duration)
            : base(LogicPriority.World, world)
        {
            mWorld = world;
            mDuration = duration;
            mTargetColor = targetColor;
        }

        protected override void Update()
        {
            var pct = (float)this.TimeActive / (float)mDuration;
            var currentFadeColor = RGColor.White.Fade(mTargetColor, pct);
            foreach (var layer in mWorld.GetLayers())
            {
                if (layer.Depth < LayerDepth.Screen)
                    layer.ExtraRenderInfo.FadeColor = currentFadeColor;
            }

            if (pct >= 1f)
                this.Kill(Engine.ExitCode.Finished);
        }
    }

}
