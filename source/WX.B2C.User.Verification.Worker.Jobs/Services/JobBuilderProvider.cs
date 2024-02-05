using System;
using System.Collections.Generic;
using Autofac.Features.Indexed;
using Quartz;

namespace WX.B2C.User.Verification.Worker.Jobs.Services
{
    internal delegate JobBuilder JobBuilderFactory(IReadOnlyDictionary<string, object> parameters);

    internal interface IJobBuilderProvider
    {
        JobBuilder Get(string jobName, IReadOnlyDictionary<string, object> parameters);
    }

    internal class JobBuilderProvider : IJobBuilderProvider
    {
        private readonly IIndex<string, JobBuilderFactory> _jobBuilders;

        public JobBuilderProvider(IIndex<string, JobBuilderFactory> jobBuilders)
        {
            _jobBuilders = jobBuilders ?? throw new ArgumentNullException(nameof(jobBuilders));
        }

        public JobBuilder Get(string jobName, IReadOnlyDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentNullException(nameof(jobName));

            if (!_jobBuilders.TryGetValue(jobName, out var jobBuilder))
                throw new ArgumentOutOfRangeException(nameof(jobName), jobName, "Unsupported job name.");

            return jobBuilder(parameters);
        }
    }
}