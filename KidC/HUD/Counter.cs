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
            get { return RGRectangleI.Create(this.Location, this.Location.Offset(64, 64)); }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }
        
        
        public int Amount { get; set; }

        public Counter(Layer layer, SimpleAnimation animation) : base(LogicPriority.World, layer)
        {
            mAnimation = animation;        
        }

        protected override void OnEntrance()
        {
            mAmountText = new GameText(this, FontManager.ClockFont, "9X", RGPointI.Empty, 16, Alignment.Far, Alignment.Center);
        }

        protected override void Update()
        {
            mAmountText.Text = this.Amount + "X";
            mAmountText.Location = Location.Offset(-mAmountText.TextArea.Width-16,0);
            mAnimation.Location = Location;
        }

        public static Counter CreateLivesCounter(Layer layer)
        {
            var animation = KidCGraphic.LivesCounter.CreateSimpleAnimation(layer.Context);
            return new Counter(layer, animation);
        }

        public static Counter CreateGemsCounter(Layer layer)
        {
            var animation = KidCGraphic.GemsCounter.CreateSimpleAnimation(layer.Context);
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
