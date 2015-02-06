using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public static class DebugTracker<T>
    {

        private static Dictionary<int, List<T>> mData = new Dictionary<int,List<T>>();


        private static List<T> GetList(object o)
        {
            List<T> list = mData.TryGet(o.GetHashCode(), null);
            if(list == null)
            {
                list = new List<T>();
                mData.Add(o.GetHashCode(), list);
            }

            return list;
        }

        public static T[] GetMostRecentData(object o, int maxItems)
        {
            return GetList(o).Reverse<T>().Take(maxItems).ToArray();
        }

        public static T[] GetData(object o, int maxItems)
        {
            var a = GetList(o);
            return a.Skip(a.Count - maxItems).ToArray();
        }

        public static void AddLog(object o, T data)
        {
            var list = GetList(o);            
            GetList(o).Add(data);
        }

        public static void AddLog(object o, T data, Predicate<T> debugCondition)
        {
            if (debugCondition(data))
                Console.WriteLine("X");

            var list = GetList(o);
            GetList(o).Add(data);
        }
    }
}
