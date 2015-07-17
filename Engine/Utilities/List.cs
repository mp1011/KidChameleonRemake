using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public static class ListUtils
    {

        public static void Transfer<T>(this ICollection<T> src, ICollection<T> dest, IEnumerable<T> items)
        {
            var itemsToAdd = items.ToArray();
            foreach (var item in itemsToAdd)
            {
                src.Remove(item);
                dest.Add(item);
            }
        }
    }
}
