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
            this.Location = new RGPointI(x, y);
        }

        public GameContext Context
        {
            get;
            private set;
        }

        public RGPointI Location
        {
            get;
            set;
        }

        public RGRectangleI Area
        {
            get { return RGRectangleI.Create(Location, RGSizeI.Empty); }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }


        public RGPointI LocationOffset
        {
            get
            {
                return RGPointI.Empty;
            }
        }
    }

}
