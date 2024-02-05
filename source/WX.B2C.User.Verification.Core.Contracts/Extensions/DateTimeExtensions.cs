using System;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AddInterval(this DateTime self, IntervalUnit unit, int value) =>
            unit switch
            {
                IntervalUnit.Second => self.AddSeconds(value),
                IntervalUnit.Minute => self.AddMinutes(value),
                IntervalUnit.Hour   => self.AddHours(value),
                IntervalUnit.Day    => self.AddDays(value),
                IntervalUnit.Week   => self.AddDays(value * 7),
                IntervalUnit.Month  => self.AddMonths(value),
                IntervalUnit.Year   => self.AddYears(value),
                _                   => throw new ArgumentOutOfRangeException(nameof(unit))
            };
    }
}
