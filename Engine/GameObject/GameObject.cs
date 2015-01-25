using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public abstract class SimpleObject : LogicObject, IDrawableRemovable, IWithPosition
    {
        public SimpleObject(GameContext context, SimpleGraphic graphic)
            : base(LogicPriority.Behavior, context)
        {
            this.mGraphic = graphic;
        }

        public RGPoint Location
        {
            get
            {
                return mGraphic.Position;
            }
            set
            {
                mGraphic.Position = value;
            }
        }

        public RGRectangle Area
        {
            get { return mGraphic.DestinationRec; }
        }

        public Direction Direction
        {
            get;
            set;
        }


        #region Drawing

        private SimpleGraphic mGraphic;
        void IDrawable.Draw(Graphics.Painter p, RGRectangle canvas)
        {
            mGraphic.Draw(p, canvas);
        }

        #endregion


        public RGPoint LocationOffset
        {
            get { return RGPoint.Empty; }
        }
    }
}
