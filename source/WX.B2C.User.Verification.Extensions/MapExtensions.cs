using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Extensions
{
    public static class MapExtensions
    {
        public static TResult[] MapToArray<T, TResult>(this IEnumerable<T> items, Func<T, TResult> map)
        {
            if(items == null)
                throw new ArgumentNullException(nameof(items));
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return items.Select(map).ToArray();
        }
    }
}
