using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Infrastructure.Communication.Services;

namespace WX.B2C.User.Verification.Worker.Jobs
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class JobsService : StatelessService, IJobStatelessService
    {
        private readonly IScheduler _scheduler;
        private readonly IJobService _jobService;
        private readonly ILogger _logger;

        public JobsService(StatelessServiceContext context, IScheduler scheduler, IJobService jobService, ILogger logger)
            : base(context)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _logger = logger?.ForContext<JobsService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task ScheduleAsync(JobRequestDto request, CancellationToken cancellationToken = default) =>
            _jobService.ScheduleAsync(request, cancellationToken);

        public Task<bool> UnscheduleAsync(JobParametersDto parameters, CancellationToken cancellationToken = default) =>
            _jobService.UnscheduleAsync(parameters, cancellationToken);
        
        public Task UnscheduleJobAsync(JobParametersDto parameters, CancellationToken cancellationToken = default) =>
            _jobService.UnscheduleJobAsync(parameters, cancellationToken);

        public Task InterruptAsync(string fireInstanceId, CancellationToken cancellationToken = default) =>
            _jobService.InterruptAsync(fireInstanceId, cancellationToken);

        public Task<bool> ExistsTriggerAsync(JobParametersDto jobParameters) =>
            _jobService.ExistsTriggerAsync(jobParameters);

        public Task<JobTriggerDto[]> GetJobTriggersAsync(JobParametersDto jobParametersDto, CancellationToken cancellationToken = default) =>
            _jobService.GetJobTriggersAsync(jobParametersDto, cancellationToken);

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() =>
            this.CreateServiceRemotingInstanceListeners();

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _scheduler.Start(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.Information("Cancellation requested. RunAsync completed gracefully");
                throw;
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Error in RunAsync");
                throw;
            }
        }

        protected override void OnAbort()
        {
            _logger.Warning("Service aborted. Potentially jobs not finished");
            base.OnAbort();
        }

        protected override Task OnCloseAsync(CancellationToken cancellationToken)
        {
            _logger.Warning("Service closed. Potentially jobs not finished");
            return _scheduler.Shutdown(cancellationToken);
        }
    }
}
