using System;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Infrastructure.Communication.Services;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class JobServiceClient : IJobService
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/JobsWorker");
        private readonly IServiceClientFactory _serviceClientFactory;

        public JobServiceClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }
        
        public Task ScheduleAsync(JobRequestDto request, CancellationToken cancellationToken = default) =>
            CreateServiceProxy().Execute(service => service.ScheduleAsync(request, cancellationToken));

        public Task<bool> UnscheduleAsync(JobParametersDto parameters, CancellationToken cancellationToken = default) =>
            CreateServiceProxy().Execute(service => service.UnscheduleAsync(parameters, cancellationToken));
        
        public Task UnscheduleJobAsync(JobParametersDto parameters, CancellationToken cancellationToken = default) =>
            CreateServiceProxy().Execute(service => service.UnscheduleJobAsync(parameters, cancellationToken));

        public Task InterruptAsync(string fireInstanceId, CancellationToken cancellationToken) =>
            CreateServiceProxy().Execute(service => service.InterruptAsync(fireInstanceId, cancellationToken));

        public Task<bool> ExistsTriggerAsync(JobParametersDto jobParametersDto) =>
            CreateServiceProxy().Execute(service => service.ExistsTriggerAsync(jobParametersDto));

        public Task<JobTriggerDto[]> GetJobTriggersAsync(JobParametersDto jobParameters, CancellationToken cancellationToken) =>
            CreateServiceProxy().Execute(service => service.GetJobTriggersAsync(jobParameters, cancellationToken));

        private IServiceClientProxy<IJobStatelessService> CreateServiceProxy() =>
            _serviceClientFactory.CreateStatelessServiceProxy<IJobStatelessService>(ServiceUri);
    }
}
