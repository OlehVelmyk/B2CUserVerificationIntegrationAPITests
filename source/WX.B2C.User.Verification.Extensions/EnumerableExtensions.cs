using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Extensions
{
    public static class EnumerableExtensions
    {
        // TODO: Fix because it works wrong when it compares arrays with duplicate elements,
        // Example: [1, 1] and [1, 1] - false is wrong.
        public static bool IsEquivalent<T>(this ICollection<T> target, ICollection<T> source)
        {
            if (target == null && source == null)
                return true;
            if (target == null || source == null)
                return false;
            if (target.Count != source.Count)
                return false;

            return target.Intersect(source).Count() == target.Count;
        }

        // TODO: Fix wrong behavior
        public static bool IsDifferent<T>(this ICollection<T> target, ICollection<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return !target.IsEquivalent(source);
        }

        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (var element in enumerable)
                action(element);
        }

        public static Task Foreach<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return enumerable.Select(action).WhenAll();
        }

        public static async Task ForeachConsistently<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            foreach (var element in enumerable)
                await action(element);
        }

        public static Task<TResult[]> Foreach<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return enumerable.Select(action).WhenAll();
        }

        public static async Task<IEnumerable<T>> Where<T>(this IEnumerable<T> enumerable, Func<T, Task<bool>> condition)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            var results = await enumerable.Select(item => (Item: item, Condition: condition(item)))
                                          .Foreach(async tuple =>
                                          {
                                              var result = await tuple.Condition;
                                              return (Item: tuple.Item, Result: result);
                                          });

            return results.Where(tuple => tuple.Result).Select(tuple => tuple.Item);
        }

        public static async Task<TAccumulate> Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source,
                                                                              TAccumulate seed,
                                                                              Func<TAccumulate, TSource, Task<TAccumulate>> func)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (seed == null)
                throw new ArgumentNullException(nameof(seed));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var result = seed;
            foreach (var element in source)
            {
                result = await func(result, element);
            }

            return result;
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source,
                                                                  TAccumulate seed,
                                                                  Func<TAccumulate, TSource, int, TAccumulate> func)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (seed == null)
                throw new ArgumentNullException(nameof(seed));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            int count = 0;
            var result = seed;
            foreach (var element in source)
            {
                result = func(result, element, count);
                count++;
            }

            return result;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => !enumerable?.Any() ?? true;

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
            enumerable.SelectMany(i => i);


        public static bool ContainsAny<T>(this IEnumerable<T> items, params T[] other) where T : IEquatable<T>
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return items.Any(other.Contains);
        }

        public static bool In<T>(this T target, IEnumerable<T> enumerable) =>
            enumerable.Contains(target);

        /// <summary>
        /// TODO replace code string.Join(separator, items) to use this extension
        /// </summary>
        public static string JoinIntoString<T>(this IEnumerable<T> items, string separator = ", ") =>
            string.Join(separator, items);
    }
}
