using System;
using System.Collections;
using System.Collections.Generic;

namespace OccurrencesFinder.Utilities.Extensions
{
    public static class ObjectExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> effect)
        {
            foreach (var item in source)
            {
                effect(item);
            }
        }

        public static TAnother Map<T, TAnother>(this T obj, Func<T, TAnother> mapper)
            => mapper(obj);

        public static T SideEffect<T>(this T obj, Action<T> effect)
        {
            effect(obj);
            return obj;
        }
    }
}