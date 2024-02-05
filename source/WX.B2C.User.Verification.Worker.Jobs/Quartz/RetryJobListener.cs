using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    internal class RetryJobListener : IJobListener
    {
        private readonly IJobRetryStrategy _retryStrategy;

        public RetryJobListener(IJobRetryStrategy retryStrategy)
        {
            _retryStrategy = retryStrategy ?? throw new ArgumentNullException(nameof(retryStrategy));
        }

        public string Name => "Retry";

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            if (JobFailed(jobException) && _retryStrategy.ShouldRetry(context))
            {
                var trigger = _retryStrategy.GetTrigger(context);
                var unscheduled = await context.Scheduler.UnscheduleJob(context.Trigger.Key, cancellationToken);
                var nextRunAt = await context.Scheduler.ScheduleJob(context.JobDetail, trigger, cancellationToken);
            }
        }

        private static bool JobFailed(JobExecutionException jobException)
        {
            return jobException != null;
        }
    }
}
