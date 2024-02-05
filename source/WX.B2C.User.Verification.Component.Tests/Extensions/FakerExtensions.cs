using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class FakerExtensions
    {
        public static TaskDto PickRandom(this Faker faker, IEnumerable<TaskDto> tasks, IEnumerable<TaskType> except)
        {
            if (faker == null)
                throw new ArgumentNullException(nameof(faker));
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (except == null)
                throw new ArgumentNullException(nameof(except));

            var items = tasks
                          .Where(task => !except.Contains(task.Type))
                          .OrderBy(task => task.Type)
                          .ToArray();

            return faker.PickRandom(items);
        }

        public static void ResetRandomly<TKey, TValue>(this Faker faker, IDictionary<TKey, TValue> properties, int maxCount = int.MaxValue, TValue resetToValue = default)
        {
            if (faker == null)
                throw new ArgumentNullException(nameof(faker));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            var numberToReset = faker.Random.Int(0, Math.Min(properties.Count, maxCount));
            var propertiesToReset = faker.PickRandom(properties, numberToReset);

            foreach (var property in propertiesToReset)
                properties[property.Key] = resetToValue;
        }
    }
}