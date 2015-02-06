using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class IntegerX
    {        
        private double mDoubleValue;
        private int mRoundIndex;
        private ulong lastUpdateTime;
      
        public IntegerX(int value)
        {
            mDoubleValue = (double)value;
        }

        public float FValue { get { return (float)mDoubleValue; } set { this.DValue = (double)value; } }

        public double DValue
        {
            get
            {
                return mDoubleValue;
            }
            set
            {
                mDoubleValue = value;
            }
        }

        public int Value
        {
            get
            {
                return CalcValue();
            }            
        }

        private int CalcValue()
        {

            var absValue = Math.Abs(this.DValue);
            var mod = (this.DValue < 0.0) ? -1.0 : 1.0;

            double frac;

            frac = absValue - Math.Floor(absValue);
            if (frac > 0 && mRoundIndex++ >= (1.0 / frac))
            {
                mRoundIndex = 0;
                return (int)(mod * Math.Ceiling(absValue));
            }
            else
                return (int)(mod * Math.Floor(absValue));

        }
    }
}
