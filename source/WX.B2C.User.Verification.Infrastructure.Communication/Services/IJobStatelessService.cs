using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Services
{
    public interface IJobStatelessService : IService
    {
        Task ScheduleAsync(JobRequestDto request, CancellationToken cancellationToken = default);

        Task<bool> UnscheduleAsync(JobParametersDto parameters, CancellationToken cancellationToken = default);
        
        Task UnscheduleJobAsync(JobParametersDto parameters, CancellationToken cancellationToken = default);
        
        Task InterruptAsync(string fireInstanceId, CancellationToken cancellationToken = default);

        Task<bool> ExistsTriggerAsync(JobParametersDto jobParametersDto);

        Task<JobTriggerDto[]> GetJobTriggersAsync(JobParametersDto jobParametersDto, CancellationToken cancellationToken = default);
    }
}
