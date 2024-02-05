using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Bogus;
using Newtonsoft.Json;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class DumpExtensions
    {
        public static T Dump<T>(this T self, [CallerMemberName] string methodName = null)
        {
            var stringRepresentation = IsNotPrimitive<T>() ? JsonConvert.SerializeObject(self) : self.ToString();
            Console.WriteLine($"{methodName}: {stringRepresentation}");
            return self;
        }

        private static bool IsNotPrimitive<T>()
        {
            var type = typeof(T);
            return type.IsClass && !type.IsPrimitive;
        }

        public static T PickRandomAndDump<T>(this Faker faker, ICollection<T> elements)
        {
            var pick = faker.PickRandom(elements);
            Console.WriteLine($"From elements {ToString(elements.ToArray())} picked {pick}");
            return pick;

            string ToString<T>(IEnumerable<T> items) =>
                $"[{string.Join(", ", items)}]";
        }
    }
}
