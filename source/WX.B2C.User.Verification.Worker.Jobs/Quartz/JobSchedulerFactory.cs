using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    internal class JobSchedulerFactory
    {
        private readonly IQuartzPropertiesProvider _propertiesProvider;
        private readonly IJobSchedulerInitializer _schedulerInitializer;
        private readonly IJobFactory _jobFactory;

        public JobSchedulerFactory(
            IQuartzPropertiesProvider propertiesProvider,
            IJobSchedulerInitializer schedulerInitializer,
            IJobFactory jobFactory)
        {
            _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
            _schedulerInitializer = schedulerInitializer ?? throw new ArgumentNullException(nameof(schedulerInitializer));
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
        }

        public async Task<IScheduler> Create(IJobRetryStrategy retryStrategy = null)
        {
            var properties = _propertiesProvider.GetProperties();

            var scheduler = await SchedulerBuilder
                            .Create(properties)
                            .BuildScheduler();


            if (retryStrategy != null)
            {
                var retryListener = new RetryJobListener(retryStrategy);
                scheduler.ListenerManager.AddJobListener(retryListener, GroupMatcher<JobKey>.AnyGroup());
            }

            scheduler.JobFactory = _jobFactory;
            await _schedulerInitializer.InitializeAsync(scheduler);
            return scheduler;
        }
    }
}
