using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using CronSchedule = WX.B2C.User.Verification.Core.Contracts.Monitoring.CronSchedule;
using IntervalSchedule = WX.B2C.User.Verification.Core.Contracts.Monitoring.IntervalSchedule;
using Schedule = WX.B2C.User.Verification.Core.Contracts.Schedule;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    internal interface IJobRequestMapper
    {
        JobRequestDto ToScheduleJobRequest(TriggerVariantDto triggerVariant, Guid triggerId, Guid userId);

        JobParametersDto ToStopJobRequest(Guid triggerId, Guid userId);
    }

    internal class JobRequestMapper : IJobRequestMapper
    {
        public JobRequestDto ToScheduleJobRequest(TriggerVariantDto triggerVariant, Guid triggerId, Guid userId) =>
            new()
            {
                JobName = JobConstants.ScheduledTriggerJobName,
                Parameters = Map(triggerId, userId),
                Schedule = Map(triggerVariant.Schedule),
                StartAt = Map(triggerVariant.Schedule.Offset)
            };

        public JobParametersDto ToStopJobRequest(Guid triggerId, Guid userId) =>
            new()
            {
                JobName = JobConstants.ScheduledTriggerJobName,
                Parameters = Map(triggerId, userId)
            };

        private static JobParameterDto[] Map(Guid triggerId, Guid userId)
        {
            var triggerIdParameter = new JobParameterDto
            {
                Name = nameof(JobConstants.TriggerId),
                Value = triggerId
            };
            var userIdParameter = new JobParameterDto
            {
                Name = nameof(JobConstants.UserId),
                Value = userId
            };
            return new[] { triggerIdParameter, userIdParameter };
        }

        private DateTime? Map(TimeSpan? startOffset)
        {
            return !startOffset.HasValue ? null : DateTime.UtcNow.Add(startOffset.Value);
        }

        private static Schedule Map(Core.Contracts.Monitoring.Schedule schedule)
        {
            return schedule switch
            {
                CronSchedule cronSchedule => new Core.Contracts.CronSchedule
                {
                    Cron = cronSchedule.Cron
                },
                IntervalSchedule intervalSchedule => new Core.Contracts.IntervalSchedule
                {
                    Unit = intervalSchedule.Unit,
                    Value = intervalSchedule.Value
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}