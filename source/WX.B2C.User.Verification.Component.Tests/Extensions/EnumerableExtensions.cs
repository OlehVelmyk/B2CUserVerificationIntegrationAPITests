using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool ContainsAll<T>(this IEnumerable<T> items, params T[] subset)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (subset is null)
                throw new ArgumentNullException(nameof(subset));

            return subset.All(items.Contains);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T value)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            return items.Except(new[] { value });
        }
        public static (IEnumerable<T> satisfied, IEnumerable<T> unsatisfied) Divide<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            var (satisfied, unsatisfied) = (new List<T>(), new List<T>());

            foreach (var item in items)
            {
                if (predicate(item))
                    satisfied.Add(item);
                else
                    unsatisfied.Add(item);
            }

            return (satisfied, unsatisfied);
        }

        public static (IEnumerable<T> first, IEnumerable<T> second, IEnumerable<T> others) Divide<T>(
            this IEnumerable<T> items, 
            Func<T, bool> firstPredicate,
            Func<T, bool> secondPredicate)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (firstPredicate is null)
                throw new ArgumentNullException(nameof(firstPredicate));
            if (secondPredicate is null)
                throw new ArgumentNullException(nameof(secondPredicate));

            var (first, second, others) = (new List<T>(), new List<T>(), new List<T>());

            foreach (var item in items)
            {
                if (firstPredicate(item))
                    first.Add(item);
                else if (secondPredicate(item))
                    second.Add(item);
                else
                    others.Add(item);
            }

            return (first, second, others);
        }

        public static (IEnumerable<TResult> first, IEnumerable<TResult> second, IEnumerable<TResult> others) Select<T, TResult>(
            this (IEnumerable<T> first, IEnumerable<T> second, IEnumerable<T> others) enumerables, 
            Func<T, TResult> selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var first = enumerables.first?.Select(selector) ?? throw new ArgumentNullException(nameof(enumerables.first));
            var second = enumerables.second?.Select(selector) ?? throw new ArgumentNullException(nameof(enumerables.second));
            var others = enumerables.others?.Select(selector) ?? throw new ArgumentNullException(nameof(enumerables.others));

            return (first, second, others);
        }

        public static (T[] first, T[] second, T[] others) ToArray<T>(
            this (IEnumerable<T> first, IEnumerable<T> second, IEnumerable<T> others) enumerables)
        {
            var first = enumerables.first?.ToArray() ?? throw new ArgumentNullException(nameof(enumerables.first));
            var second = enumerables.second?.ToArray() ?? throw new ArgumentNullException(nameof(enumerables.second));
            var others = enumerables.others?.ToArray() ?? throw new ArgumentNullException(nameof(enumerables.others));

            return (first, second, others);
        }

        public static IEnumerable<T> Concat<T>(this (IEnumerable<T> first, IEnumerable<T> second, IEnumerable<T> others) enumerables)
        {
            var first = enumerables.first ?? throw new ArgumentNullException(nameof(enumerables.first));
            var second = enumerables.second ?? throw new ArgumentNullException(nameof(enumerables.second));
            var others = enumerables.others ?? throw new ArgumentNullException(nameof(enumerables.others));

            return first.Concat(second).Concat(others);
        }
    }
}
