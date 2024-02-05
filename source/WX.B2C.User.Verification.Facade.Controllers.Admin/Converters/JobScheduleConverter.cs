using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Converters;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Converters
{
    public sealed class JobScheduleConverter : PolymorphicDeserializer<JobSchedule>
    {
        private static readonly Dictionary<string, Type> KnownTypes = new()
        {
            [nameof(JobScheduleType.Cron).ToLower()] = typeof(CronJobSchedule),
            [nameof(JobScheduleType.Interval).ToLower()] = typeof(IntervalJobSchedule)
        };

        public JobScheduleConverter()
            : base(nameof(JobSchedule.Type).ToLower())
        {
        }

        protected override bool TryGetType(string discriminator, out Type resolvedType)
        {
            if (string.IsNullOrWhiteSpace(discriminator))
                throw new ArgumentNullException(nameof(discriminator));

            return KnownTypes.TryGetValue(discriminator.ToLower(), out resolvedType);
        }
    }
}