using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    public enum LevelTheme
    {
        Woods
    }

    interface ILevelTheme
    {
        LevelTheme ThemeType { get; }
        IEnumerable<Layer> CreateLayers(GameContext context, Map map);
    }


    public class ThemeAttribute : Attribute
    {
        public LevelTheme Theme { get; private set; }

        public ThemeAttribute(LevelTheme t)
        {
            this.Theme = t;
        }
    }

    [Theme(LevelTheme.Woods)]
    public class WoodsTheme : ILevelTheme
    {

        public LevelTheme ThemeType
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<Layer> CreateLayers(GameContext context, Map map)
        {
            var bg = SpriteSheet.Load("woods_bg", context);

            var firstLayer = ImageLayer.CreateRepeatingHorizontal(context, new SimpleGraphic(bg, 1), .2f, new RGPointI(0, 200));
            yield return firstLayer;

            var secondLayer = ImageLayer.CreateRepeatingHorizontal(context, new SimpleGraphic(bg, 0), .6f);
            secondLayer.PositionBelow(firstLayer);
            yield return secondLayer;

            var thirdLayer = ImageLayer.CreateRepeatingHorizontal(context, new SimpleGraphic(bg, 2), .6f);
            thirdLayer.PositionBelow(secondLayer);
            yield return thirdLayer;

            yield return new TileLayer(context, map, RGPointI.Empty, LayerDepth.Foreground);

            yield return  new HUD(context);
        }
    }
}
