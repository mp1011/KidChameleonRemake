﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class HealthGuage: LogicObject, IDrawableRemovable, IWithPosition 
    {

        private SimpleGraphic mCellGraphic;
        public RGPointI Location { get; set; }
       
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        
        public HealthGuage(Layer layer)
            : base(LogicPriority.World, layer)
        {
            mCellGraphic = KidCGraphic.HealthGuage.CreateSimpleGraphic(this.Context);

            this.MaxHealth = 3;
            this.CurrentHealth = 2;
        }

        public void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
            var cursor = this.Location;

            for (int i = 0; i < MaxHealth; i++)
            {
                if (i < CurrentHealth)
                    mCellGraphic.SourceIndex =1;
                else
                    mCellGraphic.SourceIndex = 0;

                mCellGraphic.Position = cursor;
                mCellGraphic.Draw(p, canvas);
                cursor = cursor.Offset(mCellGraphic.DestinationRec.Width, 0);
            }
        }


        public RGRectangleI Area
        {
            get { return mCellGraphic.DestinationRec; }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }
    }
}
