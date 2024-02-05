using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Infrastructure.Communication.Services;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class CheckProviderServiceClient : ICheckProviderService
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/ChecksWorker");
        private readonly IServiceClientFactory _serviceClientFactory;

        public CheckProviderServiceClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task<CheckInputParameterDto[]> GetParametersAsync(Guid variantId) =>
            CreateServiceProxy().Execute(actor => actor.GetParametersAsync(variantId));

        public Task RunAsync(CheckRunningContextDto[] contexts) =>
            CreateServiceProxy().Execute(actor => actor.RunAsync(contexts));

        public Task ProcessAsync(CheckProcessingContextDto context) =>
            CreateServiceProxy().Execute(actor => actor.ProcessAsync(context));

        private IServiceClientProxy<ICheckProviderStatelessService> CreateServiceProxy() =>
            _serviceClientFactory.CreateStatelessServiceProxy<ICheckProviderStatelessService>(ServiceUri);
    }
}