using System;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts.Monitoring
{
    public abstract class Schedule
    {
        public TimeSpan? Offset { get; set; }
    }

    public class IntervalSchedule : Schedule
    {
        public int Value { get; set; }

        public IntervalUnit Unit { get; set; }
    }

    public class CronSchedule : Schedule
    {
        public string Cron { get; set; }
    }
}