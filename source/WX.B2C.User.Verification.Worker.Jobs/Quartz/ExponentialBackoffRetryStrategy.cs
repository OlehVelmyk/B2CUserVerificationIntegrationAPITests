using System;
using Quartz;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    public class JobRetrySettings
    {
        public JobRetrySettings(int maxRetries, TimeSpan backoffInterval)
        {
            if (maxRetries < 0)
                throw new ArgumentException("Should not be negative.", nameof(maxRetries));

            MaxRetries = maxRetries;
            BackoffInterval = backoffInterval;
        }

        public int MaxRetries { get; }

        public TimeSpan BackoffInterval { get; }
    }

    public interface IJobRetryStrategy
    {
        bool ShouldRetry(IJobExecutionContext context);

        ITrigger GetTrigger(IJobExecutionContext context);
    }

    internal class ExponentialBackoffRetryStrategy : IJobRetryStrategy
    {
        private const string Retries = "Retries";
        private readonly JobRetrySettings _settings;

        public ExponentialBackoffRetryStrategy(JobRetrySettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public bool ShouldRetry(IJobExecutionContext context)
        {
            var retries = GetAlreadyPerformedRetries(context);
            return retries < _settings.MaxRetries;
        }

        public ITrigger GetTrigger(IJobExecutionContext context)
        {
            var retries = GetAlreadyPerformedRetries(context);
            var startTimeUtc = NextStartTimeUtc(retries);

            var trigger = TriggerBuilder.Create()
                                        .StartAt(startTimeUtc)
                                        .WithSimpleSchedule(x => x.WithRepeatCount(0))
                                        .WithIdentity(context.Trigger.Key)
                                        .ForJob(context.JobDetail)
                                        .Build();

            context.JobDetail.JobDataMap[Retries] = (++retries).ToString();
            return trigger;
        }

        private DateTimeOffset NextStartTimeUtc(int retries)
        {
            var factor = Math.Pow(2, retries);
            var backoff = _settings.BackoffInterval.Multiply(factor);
            return DateTimeOffset.UtcNow.Add(backoff);
        }

        private static int GetAlreadyPerformedRetries(IJobExecutionContext context)
        {
            if (!context.MergedJobDataMap.Contains(Retries))
                return 0;

            return context.MergedJobDataMap.GetIntValue(Retries);
        }
    }
}
