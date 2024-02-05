using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts
{
    [KnownType(typeof(Dictionary<string, object>))]
    public class JobRequestDto
    {
        public string JobName { get; set; }

        public DateTime? StartAt { get; set; }

        public Schedule Schedule { get; set; }

        public JobParameterDto[] Parameters { get; set; }
    }

    [KnownType(typeof(CronSchedule))]
    [KnownType(typeof(IntervalSchedule))]
    public abstract class Schedule
    {

    }

    public class CronSchedule : Schedule
    {
        public string Cron { get; set; }
    }

    public class IntervalSchedule : Schedule
    {
        public int Value { get; set; }

        public IntervalUnit Unit { get; set; }
    }

    public class JobParametersDto
    {
        public string JobName { get; set; }

        public JobParameterDto[] Parameters { get; set; }
    }

    [KnownType(typeof(object[]))]
    [KnownType(typeof(Guid[]))]
    public class JobParameterDto
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }

    public interface IJobService
    {
        Task ScheduleAsync(JobRequestDto request, CancellationToken cancellationToken = default);

        Task<bool> UnscheduleAsync(JobParametersDto parameters, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unschedule all triggers connected to job.
        /// Note this method is allowed only if job provider JobKeyExtractors
        /// </summary>
        Task UnscheduleJobAsync(JobParametersDto parameters, CancellationToken cancellationToken = default);

        Task InterruptAsync(string fireInstanceId, CancellationToken cancellationToken);

        Task<bool> ExistsTriggerAsync(JobParametersDto jobParameters);

        Task<JobTriggerDto[]> GetJobTriggersAsync(JobParametersDto jobParameters, CancellationToken cancellationToken);
    }

    public class JobTriggerDto
    {
        public string JobName { get; set; }

        public string JobKey { get; set; }

        public string TriggerId { get; set; }

        public DateTime NextFireTime { get; set; }
    }
}
