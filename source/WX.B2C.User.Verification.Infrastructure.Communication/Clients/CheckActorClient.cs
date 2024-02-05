using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Commands.ServiceClients;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class CheckActorClient : ICheckServiceClient
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/CheckActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public CheckActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory =
                serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task RequestAsync(Guid userId, NewCheckDto newCheckDto, InitiationDto initiation) =>
            CreateActorProxy(userId, newCheckDto.VariantId)
                .Execute(actor => actor.RequestAsync(userId, newCheckDto, initiation));

        public Task StartExecutionAsync(Guid checkId, CheckExecutionContextDto executionContextDto) =>
            CreateActorProxy(checkId)
                .Execute(actor => actor.StartExecutionAsync(checkId, executionContextDto));

        public Task FinishExecutionAsync(Guid checkId, CheckExecutionResultDto executionResultDto) =>
            CreateActorProxy(checkId)
                .Execute(actor => actor.FinishExecutionAsync(checkId, executionResultDto));

        public Task SaveProcessingResultAsync(Guid checkId, CheckProcessingResultDto processingResultDto) =>
            CreateActorProxy(checkId)
                .Execute(actor => actor.SaveProcessingResultAsync(checkId, processingResultDto));

        public Task SaveErrorResultAsync(Guid checkId, CheckErrorResultDto errorResultDto) =>
            CreateActorProxy(checkId)
                .Execute(actor => actor.SaveErrorResultAsync(checkId, errorResultDto));

        public Task CancelAsync(Guid checkId) =>
            CreateActorProxy(checkId)
                .Execute(actor => actor.CancelAsync(checkId));

        private IServiceClientProxy<ICheckActor> CreateActorProxy(Guid userId, Guid variantId) =>
            _serviceClientFactory.CreateActorProxy<ICheckActor>(new ActorId($"{userId}-{variantId}"), ServiceUri);

        private IServiceClientProxy<ICheckActor> CreateActorProxy(Guid actorId) =>
            _serviceClientFactory.CreateActorProxy<ICheckActor>(new ActorId(actorId), ServiceUri);
    }
}