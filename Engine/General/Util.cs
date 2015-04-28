using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class Util
    {
        public static bool EqualsAny<T>(this T item, params T[] others) where T : struct
        {
            return others.Contains(item);
        }

        public static float LimitNumber(this float number, float max)
        {
            if (number > 0)
                return Math.Min(number, max);
            if (number < 0)
                return Math.Max(number, -max);
            else
                return 0;
        }

        public static int LimitNumber(this int number, int max)
        {
            if (number > 0)
                return Math.Min(number, max);
            if (number < 0)
                return Math.Max(number, -max);
            else
                return 0;
        }


        public static void AddOrSet<K, V>(this Dictionary<K, V> dic, K key, V val)
        {
            if (dic.ContainsKey(key))
                dic[key] = val;
            else
                dic.Add(key, val);
        }


        public static V TryGet<K, V>(this Dictionary<K, V> dic, K key, V defaultVal)
        {
            if (dic.ContainsKey(key))
                return dic[key];
            else
                return defaultVal;
        }


        public static T GetItem<T>(this T[] array, int index)
        {
            if (index < 0)
                index = 0;
            if (index >= array.Length)
                index = array.Length - 1;

            return array[index];
        }

        public static T GetItem<T>(this List<T> array, int index)
        {
            if (index < 0)
                index = 0;
            if (index >= array.Count)
                index = array.Count - 1;

            return array[index];
        }

        public static float Fix(this float num, float min, float max)
        {
            while (num < min)
                num += (max - min);

            while (num >= max)
                num -= (max - min);

            return num;
        }

        /// <summary>
        /// warning: gc unfriendly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition"></param>
        public static void RemoveAll<T>(this LinkedList<T> list, Predicate<T> condition)
        {
            var item = list.First;
            LinkedListNode<T> nextItem;

            while (item != null)
            {
                nextItem = item.Next;
                if (condition(item.Value))
                    list.Remove(item);

                item = nextItem;
            }

        }

        public static T MinElement<T>(this IEnumerable<T> list, Func<T, float> valueFunc)
        {
            float min = float.MaxValue;
            T minElement = default(T);

            foreach (var item in list)
            {
                float val = valueFunc(item);
                if (val < min)
                {
                    min = val;
                    minElement = item;
                }
            }

            return minElement;

        }

        public static double GetLineAngleR(RGPointI source, RGPointI target)
        {
            return GetLineAngleR(new RGPoint(source.X, source.Y), new RGPoint(target.X, target.Y));
        }

        /// <summary>
        /// Returns the angle between the two points in radians
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static double GetLineAngleR(RGPoint source, RGPoint target)
        {
            double a, b;
            double angle;

            a = target.X - source.X;
            b = target.Y - source.Y;

            if (a == 0 && b == 0)
                return 0;

            angle = Math.Atan(Math.Abs(b) / Math.Abs(a));

            if (a >= 0 && b >= 0)
            {
                angle *= -1;
            }
            else if (a < 0 && b >= 0)
            {
                angle += Math.PI;
            }
            else if (a >= 0 && b < 0)
            {

            }
            else if (a < 0 && b < 0)
            {
                angle = Math.PI - angle;
            }
            return angle;

        }

        /// <summary>
        /// Converts an angle in radians to a point
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static RGPoint AngleToXY(double angle, float length)
        {
            double a, b;

            double t = Math.Tan(angle);

            double c2 = length * length;
            double t2 = t * t;

            a = Math.Sqrt(c2 / (t2 + 1));
            b = Math.Sqrt(c2 - a * a);

            if (angle < (Math.PI / 2))
            {
                b *= -1;
            }
            if (angle >= (Math.PI / 2) && angle < Math.PI)
            {
                a *= -1;
                b *= -1;
            }
            else if (angle >= Math.PI && angle < ((Math.PI / 2) + Math.PI))
            {
                a *= -1;
            }
            else if (angle >= ((Math.PI / 2) + Math.PI))
            {
                ;
            }

            var f = 1f;
            if (length < 0)
                f = -1f;


            return new RGPoint((float)Math.Round(a, 4) * f, (float)Math.Round(b, 4) * f);
        }

        //http://paulbourke.net/geometry/lineline2d/Helpers.cs
        public static bool CheckLineIntersection(RGPoint a1, RGPoint a2, RGPoint b1, RGPoint b2)
        {
            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (b2.Y - b1.Y) * (a2.X - a1.X)
               -
               (b2.X - b1.X) * (a2.Y - a1.Y);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (b2.X - b1.X) * (a1.Y - b1.Y)
               -
               (b2.Y - b1.Y) * (a1.X - b1.X);

            double n_b =
               (a2.X - a1.X) * (a1.Y - b1.Y)
               -
               (a2.Y - a1.Y) * (a1.X - b1.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;


            // PointF ptIntersection = new PointF();
            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                // ptIntersection.X = (float)(a1.X + (ua * (a2.X - a1.X)));
                // ptIntersection.Y = (float)(a1.Y + (ua * (a2.Y - a1.Y)));
                return true;
            }
            return false;
        }
        public static RGPoint GetLineIntersection(RGPoint a1, RGPoint a2, RGPoint b1, RGPoint b2)
        {
            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (b2.Y - b1.Y) * (a2.X - a1.X)
               -
               (b2.X - b1.X) * (a2.Y - a1.Y);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (b2.X - b1.X) * (a1.Y - b1.Y)
               -
               (b2.Y - b1.Y) * (a1.X - b1.X);

            double n_b =
               (a2.X - a1.X) * (a1.Y - b1.Y)
               -
               (a2.Y - a1.Y) * (a1.X - b1.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return new RGPoint(float.PositiveInfinity, float.PositiveInfinity);

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;


            // PointF ptIntersection = new PointF();
            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                RGPoint intersection = new RGPoint((float)(a1.X + (ua * (a2.X - a1.X))),
                    (float)(a1.Y + (ua * (a2.Y - a1.Y))));
                return intersection;
            }
            return new RGPoint(float.PositiveInfinity, float.PositiveInfinity);
        }

        public static double Cosine(double rad)
        {
            return Cosine(rad, 3);
        }

        public static double Cosine(double rad, int decimals)
        {
            var result = Math.Cos(rad);
            return Math.Round(result, decimals);
        }

        public static bool ContainsIndex<T>(this T[,] grid, int x, int y)
        {
            return x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1);
        }

        private static Random mRandom = new Random();


        public static int RandomNumber(int maxExclusive)
        {
            return mRandom.Next(maxExclusive);
        }

        public static int RandomNumber(int minInclusive, int maxExclusive)
        {
            return mRandom.Next(minInclusive, maxExclusive);
        }

        public static bool RandomChance(double chance)
        {
            return mRandom.NextDouble() <= chance;
        }

        public static T RandomItem<T>(this IEnumerable<T> items)
        {
            if (!items.Any())
                return default(T);

            int index = RandomNumber(0, items.Count());
            return items.ElementAt(index);
        }

        /// <summary>
        /// Randomly returns an item from the collection, where a higher weight value indicates a better chance of being picked. A weight value of 0 or lower means the item will never be picked unless there is no other choice.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="weightFunction"></param>
        /// <returns></returns>
        public static T RandomItem<T>(this IEnumerable<T> items, Func<T, int> weightFunction)
        {
            var weightedItems = items.Select(p=> new { Item = p, Weight = weightFunction(p)}).Where(p=>p.Weight > 0).OrderByDescending(p=>p.Weight).ToArray();

            int sum = weightedItems.Sum(p => p.Weight);

            int currentSum = 0;
            weightedItems = weightedItems.Select(p =>
            {
                var newItem = new { Item = p.Item, Weight = currentSum + p.Weight };
                currentSum += p.Weight;
                return newItem;
            }).ToArray();

            int chosenValue = RandomNumber(sum);
            foreach (var item in weightedItems)
            {
                if (chosenValue < item.Weight)
                    return item.Item;
            }

            return items.RandomItem();

            
        }


        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static bool NotNullOrEmpty(this string str)
        {
            return !String.IsNullOrEmpty(str);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static bool NotNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return !list.IsNullOrEmpty();
        }

        public static string StringJoin(this IEnumerable<string> list, string separator)
        {
            return String.Join(separator, list.NeverNull().ToArray());
        }

        public static T NotNull<T>(this T obj) where T:new()
        {
            if(obj == null)
                return new T();
            else
                return obj;
        }

        public static int MaxOrDefault<T>(this IEnumerable<T> list, Func<T, int> fnGetValue, int defaultValue)
        {
            if (list.IsNullOrEmpty())
                return defaultValue;
            return list.Select(p => fnGetValue(p)).Max();
        }

        public static IEnumerable<T> NeverNull<T>(this IEnumerable<T> list) 
        {
            if (list == null)
                return new T[]{ };
            else
                return list;
        }


        public static string AppendCSV(this string str, string newItems)
        {
            if (String.IsNullOrEmpty(str))
                return newItems;

            foreach (var newItem in newItems.Split(','))
            {            
                var list = str.Split(',');
                if (!list.Contains(newItem))
                    str += "," + newItem;
            }

            return str;
        }

        public static string RemoveCSV(this string str, string item)
        {
            if (String.IsNullOrEmpty(str))
                return "";

            return str.Split(',').Where(p => !p.Equals(item)).StringJoin(",");            
        }

        public static string NotNull(this string str) 
        {
            if (str == null)
                return "";
            else
                return str;
        }

        public static T AddItem<T>(this ICollection<T> list, T newItem)
        {
            list.Add(newItem);
            return newItem;
        }

    }

   
}
