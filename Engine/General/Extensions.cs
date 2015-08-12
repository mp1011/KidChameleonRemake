using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        public static bool NotNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return !list.IsNullOrEmpty();
        }

        public static bool ContainsAll<T>(this IEnumerable<T> list1, IEnumerable<T> list2)        
        {
            foreach (var item in list2.NeverNull())
                if (!list1.Contains(item))
                    return false;

            return true;

        }

        public static bool ContainsAny<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            foreach (var item in list1.NeverNull())
                if (list2.Contains(item))
                    return true;

            return false;

        }
    }
}
