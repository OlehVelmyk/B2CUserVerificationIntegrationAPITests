using System;
using System.Collections.Generic;
using System.Linq;
using MbDotNet.Models.Imposters;
using MbDotNet.Models.Stubs;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Extensions
{
    internal static class HttpImposterExtensions
    {
        public static void AppendStubs(this HttpImposter imposter, IEnumerable<HttpStub> stubs)
        {
            if (imposter == null)
                throw new ArgumentNullException(nameof(imposter));
            if (stubs == null)
                throw new ArgumentNullException(nameof(stubs));
            if (!stubs.Any())
                return;

            if (imposter.Stubs is List<HttpStub> existingStubs)
            {
                existingStubs.InsertRange(0, stubs);
                return;
            }

            imposter.Stubs.AppendRange(stubs);
        }

        public static void AppendRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (!enumerable.Any())
                return;

            var existingItems = collection.ToArray();

            collection.Clear();
            collection.AddRange(enumerable);
            collection.AddRange(existingItems);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            enumerable.Foreach(item => collection.Add(item));
        }
    }
}
