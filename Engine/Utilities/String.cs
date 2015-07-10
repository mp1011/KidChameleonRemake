using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class StringUtil
    {

        public static int TryParseInt(this string str, int defaultValue)
        {
            int value;
            if (Int32.TryParse(str, out value))
                return value;
            else
                return defaultValue;
        }

        public static float TryParseFloat(this string str, float defaultValue)
        {
            float value;
            if (float.TryParse(str, out value))
                return value;
            else
                return defaultValue;
        }

        public static double TryParseDouble(this string str, double defaultValue)
        {
            double value;
            if (double.TryParse(str, out value))
                return value;
            else
                return defaultValue;
        }
    }
}
