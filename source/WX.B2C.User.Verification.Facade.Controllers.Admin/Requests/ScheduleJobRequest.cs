using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class ScheduleJobRequest
    {
        public string JobName { get; set; }

        [NotRequired]
        public DateTime? StartAt { get; set; }

        [NotRequired]
        public JobSchedule Schedule { get; set; }

        public Dictionary<string, object> JobParameters { get; set; }
    }

    public enum JobScheduleType
    {
        Cron = 1,
        Interval = 2
    }

    [KnownType(typeof(CronJobSchedule))]
    [KnownType(typeof(IntervalJobSchedule))]
    public abstract class JobSchedule
    {
        public JobScheduleType Type { get; set; }
    }

    public class CronJobSchedule : JobSchedule
    {
        public string Cron { get; set; }
    }

    public class IntervalJobSchedule : JobSchedule
    {
        public int Value { get; set; }

        public IntervalUnit Unit { get; set;}
    }
}
