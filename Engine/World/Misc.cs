using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class WorldPoint : IWithPosition
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
            get
            {
                var x = Location.X;
                var y = Location.Y;
                var pad = 2;
                return RGRectangleI.FromTLBR(y - pad, x - pad, y + pad, x + pad);
            }
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
