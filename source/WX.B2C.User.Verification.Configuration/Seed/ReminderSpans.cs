using WX.B2C.User.Verification.Configuration.Models;
using WX.B2C.User.Verification.Domain.Enums;
using WX.Configuration.Contracts.Data;

namespace WX.B2C.User.Verification.Configuration.Seed
{
    internal static class ReminderSpans
    {
        public static ReminderSpan[] Seed(EnvironmentData environmentData) =>
            environmentData.Environment.ToLowerInvariant() switch
            {
                "production" => ProductionSeed,
                _            => EnvSeed
            };

        private static ReminderSpan[] EnvSeed =
        {
            new()
            {
                Value = 2,
                Unit = IntervalUnit.Minute,
                Order = 1
            },
            new()
            {
                Value = 2,
                Unit = IntervalUnit.Minute,
                Order = 2
            },
            new()
            {
                Value = 2,
                Unit = IntervalUnit.Minute,
                Order = 3
            },
            new()
            {
                Value = 2,
                Unit = IntervalUnit.Minute,
                Order = 4
            },
            new()
            {
                Value = 1,
                Unit = IntervalUnit.Minute,
                Order = 5
            }
        };
        
        private static ReminderSpan[] ProductionSeed =
        {
            new()
            {
                Value = 7,
                Unit = IntervalUnit.Day,
                Order = 1
            },
            new()
            {
                Value = 7,
                Unit = IntervalUnit.Day,
                Order = 2
            },
            new()
            {
                Value = 7,
                Unit = IntervalUnit.Day,
                Order = 3
            },
            new()
            {
                Value = 7,
                Unit = IntervalUnit.Day,
                Order = 4
            },
            new()
            {
                Value = 2,
                Unit = IntervalUnit.Day,
                Order = 5
            }
        };
    }
}
