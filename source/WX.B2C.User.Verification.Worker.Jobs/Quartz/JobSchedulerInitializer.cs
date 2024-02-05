using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Worker.Jobs.Jobs;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    public interface IJobSchedulerInitializer
    {
        Task InitializeAsync(IScheduler scheduler);
    }

    internal class JobSchedulerInitializer : IJobSchedulerInitializer
    {
        private readonly IHostSettingsProvider _hostSettingsProvider;

        public JobSchedulerInitializer(IHostSettingsProvider hostSettingsProvider)
        {
            _hostSettingsProvider = hostSettingsProvider ?? throw new ArgumentNullException(nameof(hostSettingsProvider));
        }

        public Task InitializeAsync(IScheduler scheduler)
        {
            var jobsAndTriggers = GetJobsAndTriggers()
                .ToDictionary(x => x.JobDetail, x => x.Triggers);

            return scheduler.ScheduleJobs(jobsAndTriggers, true);
        }

        private IEnumerable<(IJobDetail JobDetail, IReadOnlyCollection<ITrigger> Triggers)> GetJobsAndTriggers()
        {
            // Add default job triggers here

            yield return (RefreshBridgerPasswordJob.Builder(null).Build(), new[]
            {
                TriggerBuilder.Create()
                              .WithIdentity($"{RefreshBridgerPasswordJob.Name}.trigger")
                              .StartNow()
                              .WithSimpleSchedule(builder => builder
                                                             .RepeatForever()
                                                             .WithInterval(TimeSpan.FromDays(1))
                                                             .WithMisfireHandlingInstructionIgnoreMisfires())
                              .Build()
            });

            yield return GetAccountAlertJobAndTrigger();
        }

        private (IJobDetail JobDetail, IReadOnlyCollection<ITrigger> Triggers) GetAccountAlertJobAndTrigger()
        {
            var batchSize = int.Parse(_hostSettingsProvider.GetSetting("DefaultBatchSize"));
            var settings = new BatchJobSettings
            {
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0,
                ReadingBatchSize = batchSize,
                ProcessBatchSize = batchSize
            };

            return (AccountAlertJob.Builder(null).Build(), new[]
            {
                TriggerBuilder.Create()
                              .WithIdentity($"{AccountAlertJob.Name}.trigger")
                              .WithCronSchedule(_hostSettingsProvider.GetSetting("AccountAlertCronExpression"),
                                                c => c.WithMisfireHandlingInstructionIgnoreMisfires())
                              .UsingJobData(Constants.JobSettings, JsonConvert.SerializeObject(settings))
                              .Build()
            });
        }
    }
}
