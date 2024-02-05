using System;
using System.Collections.Generic;
using Autofac.Features.Indexed;
using Quartz;

namespace WX.B2C.User.Verification.Worker.Jobs.Services
{
    internal delegate JobKey JobKeyFactory(IReadOnlyDictionary<string, object> parameters);

    internal interface IJobKeyProvider
    {
        JobKey Get(string jobName, IReadOnlyDictionary<string, object> parameters);
    }
    
    internal class JobKeyProvider : IJobKeyProvider
    {
        private readonly IIndex<string, JobKeyFactory> _jobKeyExtractors;

        public JobKeyProvider(IIndex<string, JobKeyFactory> triggerKeyExtractors)
        {
            _jobKeyExtractors = triggerKeyExtractors ?? throw new ArgumentNullException(nameof(triggerKeyExtractors));
        }

        public JobKey Get(string jobName, IReadOnlyDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentNullException(nameof(jobName));

            if (!_jobKeyExtractors.TryGetValue(jobName, out var jobBuilder))
                throw new NotImplementedException($"Job key factory is not provided for job {jobName}");

            return jobBuilder(parameters);
        }
    }
}