using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Infrastructure.Communication.Services;

namespace WX.B2C.User.Verification.Providers
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class ChecksWorker : StatelessService, ICheckProviderStatelessService
    {
        private readonly ICheckProviderService _checkProviderService;

        public ChecksWorker(StatelessServiceContext context, ICheckProviderService checkProviderService)
            : base(context)
        {
            _checkProviderService = checkProviderService ?? throw new ArgumentNullException(nameof(checkProviderService));
        }

        public Task<CheckInputParameterDto[]> GetParametersAsync(Guid variantId) => _checkProviderService.GetParametersAsync(variantId);

        public Task RunAsync(CheckRunningContextDto[] contexts) => _checkProviderService.RunAsync(contexts);

        public Task ProcessAsync(CheckProcessingContextDto context) => _checkProviderService.ProcessAsync(context);

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() =>
            this.CreateServiceRemotingInstanceListeners();
    }
}
