using System.Collections.Generic;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Services
{
    internal interface IJobDataAggregationService<in TData, in TSettings, out TJobData> 
        where TSettings : BatchJobSettings
        where TJobData : IJobData
    {
        TJobData[] Aggregate(IEnumerable<TData> batch, TSettings settings);
    }
}
