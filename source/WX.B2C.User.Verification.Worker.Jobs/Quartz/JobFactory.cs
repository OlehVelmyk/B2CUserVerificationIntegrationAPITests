using System;
using Autofac.Features.Indexed;
using Quartz;
using Quartz.Spi;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    internal class JobFactory : IJobFactory
    {
        private readonly IIndex<Type, IJob> _jobs;

        public JobFactory(IIndex<Type, IJob> jobs)
        {
            _jobs = jobs ?? throw new ArgumentNullException(nameof(jobs));
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;

            if (!_jobs.TryGetValue(jobType, out var job))
                throw new ArgumentOutOfRangeException(nameof(jobType), jobType, "Unsupported job type.");

            return job;
        }

        public void ReturnJob(IJob job)
        {
            // No need to release job resources manually.
        }
    }
}
