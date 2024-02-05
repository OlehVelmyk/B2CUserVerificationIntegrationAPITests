using System;
using Quartz;
using WX.B2C.User.Verification.Core.Contracts;
using IntervalUnit = WX.B2C.User.Verification.Domain.Enums.IntervalUnit;

namespace WX.B2C.User.Verification.Worker.Jobs.Extensions
{
    internal static class TriggerBuilderExtensions
    {
        public static TriggerBuilder WithIntervalSchedule(this TriggerBuilder builder, IntervalSchedule intervalSchedule)
        {
            Action<CalendarIntervalScheduleBuilder> setupAction = intervalSchedule.Unit switch
            {
                IntervalUnit.Second => b => b.WithIntervalInSeconds(intervalSchedule.Value),
                IntervalUnit.Minute => b => b.WithIntervalInMinutes(intervalSchedule.Value),
                IntervalUnit.Hour   => b => b.WithIntervalInHours(intervalSchedule.Value),
                IntervalUnit.Day    => b => b.WithIntervalInDays(intervalSchedule.Value),
                IntervalUnit.Week   => b => b.WithIntervalInWeeks(intervalSchedule.Value),
                IntervalUnit.Month  => b => b.WithIntervalInMonths(intervalSchedule.Value),
                IntervalUnit.Year   => b => b.WithIntervalInYears(intervalSchedule.Value),
                _                   => throw new ArgumentOutOfRangeException()
            };

            return builder.WithCalendarIntervalSchedule(setupAction);
        }
    }
}