using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToSQL.Net.Extensions
{
    public static class CollectionExtensions
    {
        public static ICollection<T> AddRangeIf<T>(this ICollection<T> collection, ICollection<T> secondCollection, Func<T, bool> func)
        {
            foreach (var element in secondCollection)
            {
                if (func(element))
                {
                    collection.Add(element);
                }
            }
            return collection;
        }
    }
}
