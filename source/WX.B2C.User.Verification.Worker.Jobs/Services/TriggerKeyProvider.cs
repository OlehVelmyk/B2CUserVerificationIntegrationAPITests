using System;
using System.Collections.Generic;
using Autofac.Features.Indexed;
using Quartz;

namespace WX.B2C.User.Verification.Worker.Jobs.Services
{
    internal delegate TriggerKey TriggerKeyFactory(IReadOnlyDictionary<string, object> parameters);

    internal interface ITriggerKeyProvider
    {
        TriggerKey Get(string jobName, IReadOnlyDictionary<string, object> parameters);
    }
    
    internal class TriggerKeyProvider : ITriggerKeyProvider
    {
        private readonly IIndex<string, TriggerKeyFactory> _triggerKeyExtractors;

        public TriggerKeyProvider(IIndex<string, TriggerKeyFactory> triggerKeyExtractors)
        {
            _triggerKeyExtractors = triggerKeyExtractors ?? throw new ArgumentNullException(nameof(triggerKeyExtractors));
        }

        public TriggerKey Get(string jobName, IReadOnlyDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentNullException(nameof(jobName));

            //Use default trigger key if not provided
            if (!_triggerKeyExtractors.TryGetValue(jobName, out var jobBuilder))
                return new TriggerKey($"{jobName}.trigger");

            return jobBuilder(parameters);
        }
    }
}