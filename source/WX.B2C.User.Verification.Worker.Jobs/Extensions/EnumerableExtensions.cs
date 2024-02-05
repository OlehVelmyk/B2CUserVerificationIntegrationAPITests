using System;
using System.Collections.Generic;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> RemoveAll<T>(this List<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count == 0)
                return Array.Empty<T>();

            var results = items.ToArray();
            items.Clear();
            return results;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source, int batchSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if(batchSize <= 0)
                throw new ArgumentException(nameof(batchSize));

            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                yield return YieldBatchElements(enumerator, batchSize - 1);
        }

        private static IEnumerable<T> YieldBatchElements<T>(
            IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (var i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        }
    }
}