using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace WX.B2C.User.Verification.Extensions
{
    public static class ReadOnlyDictionaryExtensions
    {
        public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this IReadOnlyDictionary<TKey, TElement> source) =>
            source.ToDictionary(x => x.Key, x => x.Value);

        public static string GetStringValue(this IReadOnlyDictionary<string, object> source, string key) =>
            source.Get<string>(key);

        public static T Get<T>(this IReadOnlyDictionary<string, object> source, string key)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (!source.ContainsKey(key))
                throw new ArgumentException($"Value for the key {key} was not found in the dictionary.", key);

            return (T)source[key];
        }

        public static Option<TValue> Find<TValue>(this IReadOnlyDictionary<string, object> source, string key) =>
            source.Find<string, TValue>(key);

        public static Option<TValue> Find<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (!source.TryGetValue(key, out var value) || value is null)
                return Option.None<TValue>();

            return value.Some();
        }

        public static Option<TValue> Find<TKey, TValue>(this IReadOnlyDictionary<TKey, object> source, TKey key)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (!source.TryGetValue(key, out var value) || value is null)
                return Option.None<TValue>();

            return value is TValue result
                ? result.Some()
                : throw new InvalidCastException(
                    $"Value for the key {key} in dictionary does not match the expected type: {typeof(TValue).FullName}. Actual type is {value.GetType().FullName}.");
        }

    }
}
