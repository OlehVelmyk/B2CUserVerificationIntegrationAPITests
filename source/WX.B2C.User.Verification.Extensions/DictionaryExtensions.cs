using System;
using System.Collections.Generic;
using Optional;

namespace WX.B2C.User.Verification.Extensions
{
    public static class DictionaryExtensions
    {
        public static T Get<T>(this IDictionary<string, object> source, string key)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (!source.ContainsKey(key))
                throw new ArgumentException($"Value for the key {key} was not found in the dictionary.", key);

            return (T)source[key];
        }

        public static Option<TValue> Find<TValue>(this IDictionary<string, object> source, string key)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (!source.TryGetValue(key, out var value) || value is null)
                return Option.None<TValue>();

            return value is TValue result
                ? result.Some()
                : throw new InvalidCastException(
                    $"Value for the key {key} in dictionary does not match the expected type: {typeof(TValue).FullName}. Actual type is {value.GetType().FullName}.");
        }

        public static object ValueOrNull(this IDictionary<string, object> source, string key)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.TryGetValue(key, out var value) ? value : null;
        }
    }
}