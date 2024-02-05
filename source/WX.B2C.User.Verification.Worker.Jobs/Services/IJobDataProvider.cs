using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Services
{
    public interface IJobData { }

    public interface IJobDataProvider<TData, in TSettings> 
        where TData : IJobData
        where TSettings : JobSettings
    {
        Task<ICollection<TData>> GetAsync(TSettings settings, CancellationToken cancellationToken);
    }

    public interface IBatchJobDataProvider<TData, in TSettings> 
        where TData : IJobData
        where TSettings : BatchJobSettings
    {
        Task<int> GetTotalCountAsync(TSettings settings, CancellationToken cancellationToken);

        IAsyncEnumerable<ICollection<TData>> GetAsync(TSettings settings, CancellationToken cancellationToken);
    }
}