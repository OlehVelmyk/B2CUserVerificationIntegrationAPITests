using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Integration.Tests.Extensions
{
    internal static class EnumerableExtensions
    {
        public static async Task<IEnumerable<T>> ToEnumerableAsync<T>(this IAsyncEnumerable<T> enumerable)
        {
            var list = new List<T>();
            await foreach (var item in enumerable)
                list.Add(item);

            return list;
        }

        public static void ForeachIndexed<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            int count = 0;
            foreach (var element in enumerable)
                action(element, count++);
        }
    }
}
