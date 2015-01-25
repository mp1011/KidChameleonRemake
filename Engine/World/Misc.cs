using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    class WorldPoint : IWithPosition
    {
        public WorldPoint(GameContext ctx, float x, float y)
        {
            this.Context = ctx;
            this.Location = new RGPoint(x, y);
        }

        public GameContext Context
        {
            get;
            private set;
        }

        public RGPoint Location
        {
            get;
            set;
        }

        public RGRectangle Area
        {
            get { return RGRectangle.Create(Location, RGSize.Empty); }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }


        public RGPoint LocationOffset
        {
            get
            {
                return RGPoint.Empty;
            }
        }
    }

}
