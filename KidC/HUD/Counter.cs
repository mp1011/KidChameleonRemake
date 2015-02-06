using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class Counter : LogicObject, IDrawableRemovable, IWithPosition 
    {
        private SimpleAnimation mAnimation;
        private GameText mAmountText;
       
        public RGPointI Location { get; set; }
        public RGRectangleI Area
        {
            get { throw new NotImplementedException();  }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }
        
        
        public int Amount { get; set; }

        public Counter(Layer layer, SimpleAnimation animation) : base(LogicPriority.World, layer.Context)
        {
            mAnimation = animation;        
        }

        protected override void OnEntrance()
        {
            mAmountText = new GameText(this.Context, FontManager.ClockFont, "9x", RGPointI.Empty, 16, Alignment.Far, Alignment.Center);
        }

        protected override void Update()
        {
            mAmountText.Text = this.Amount + "x";
            mAmountText.Location = Location.Offset(-mAmountText.TextArea.Width-8,0);
            mAnimation.Location = Location;
        }

        public static Counter CreateLivesCounter(Layer layer)
        {
            var graphic = new SimpleGraphic(SpriteSheet.Load("items",layer.Context),4,5,6);
            var animation = new SimpleAnimation(graphic, 8, layer.Context, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 1);
            return new Counter(layer, animation);
        }

        public static Counter CreateGemsCounter(Layer layer)
        {
            var graphic = new SimpleGraphic(SpriteSheet.Load("items", layer.Context), 0,1,2,3);
            var animation = new SimpleAnimation(graphic, 4, layer.Context, 0, 1, 2, 3);
            return new Counter(layer, animation);
        }

        public void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            if (mAmountText == null || mAnimation == null)
                return;

            mAmountText.Draw(p, canvas);
            mAnimation.Draw(p, canvas);
        }





        public RGPointI LocationOffset
        {
            get { return RGPointI.Empty; }
        }
    }
}
