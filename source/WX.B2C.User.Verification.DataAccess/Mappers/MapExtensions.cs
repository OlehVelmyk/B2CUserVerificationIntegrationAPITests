using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal static class MapExtensions
    {
        public static TResult[] MapArray<T, TResult>(this IEnumerable<T> items, Func<T, TResult> mapper) =>
            Map(items, mapper).ToArray();

        public static HashSet<TResult> MapHashSet<T, TResult>(this IEnumerable<T> items, Func<T, TResult> mapper) =>
            Map(items, mapper).ToHashSet();

        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> items, Func<T, TResult> mapper)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            return items.Select(mapper);
        }
    }
}