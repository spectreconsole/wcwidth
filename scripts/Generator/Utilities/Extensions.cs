using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Generator
{
    public static class Extensions
    {
        public static string GetGroupValue(this Match match, string group, string defaultValue = null)
        {
            return match.Groups[group].Success
                ? match.Groups[group].Value
                : defaultValue;
        }

        public static IEnumerable<(int Index, T Item)> Enumerate<T>(this IEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Enumerate(source.GetEnumerator());
        }


        public static IEnumerable<(int Index, T Item)> Enumerate<T>(this IEnumerator<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var last = !source.MoveNext();
            T current;

            for (var index = 0; !last; index++)
            {
                current = source.Current;
                last = !source.MoveNext();
                yield return (index, current);
            }
        }
    }
}
