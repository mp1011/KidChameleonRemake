using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public abstract class SimpleObject : LogicObject, IDrawableRemovable, IWithPosition
    {
        public SimpleObject(ILogicObject parent, SimpleGraphic graphic)
            : base(LogicPriority.Behavior, parent)
        {
            this.mGraphic = graphic;
        }

        public RGPointI Location
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

        public RGRectangleI Area
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
        void IDrawable.Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            mGraphic.Draw(p, canvas);
        }

        #endregion

    }
}
