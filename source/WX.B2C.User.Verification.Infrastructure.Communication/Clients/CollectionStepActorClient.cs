using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Commands.ServiceClients;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class CollectionStepActorClient : ICollectionStepServiceClient
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/CollectionStepActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public CollectionStepActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task<Guid> RequestAsync(Guid userId, NewCollectionStepDto newStepDto, InitiationDto initiationDto) =>
            CreateActorProxy(userId, newStepDto.XPath)
                .Execute(actor => actor.RequestAsync(userId, newStepDto, initiationDto));

        public Task SubmitAsync(Guid stepId, InitiationDto initiationDto) =>
            CreateActorProxy(stepId)
                .Execute(actor => actor.SubmitAsync(stepId, initiationDto));

        public Task ReviewAsync(Guid stepId, CollectionStepReviewResult reviewResult, InitiationDto initiationDto) =>
            CreateActorProxy(stepId)
                .Execute(actor => actor.ReviewAsync(stepId, reviewResult, initiationDto));

        public Task RemoveAsync(Guid stepId, InitiationDto initiationDto) =>
            CreateActorProxy(stepId)
                .Execute(actor => actor.RemoveAsync(stepId, initiationDto));

        public Task UpdateAsync(Guid stepId, CollectionStepPatch collectionStepPatch, InitiationDto initiationDto) =>
            CreateActorProxy(stepId)
                .Execute(actor => actor.UpdateAsync(stepId, collectionStepPatch, initiationDto));

        public Task CancelAsync(Guid stepId, InitiationDto initiationDto) =>
            CreateActorProxy(stepId)
                .Execute(actor => actor.CancelAsync(stepId, initiationDto));

        private IServiceClientProxy<ICollectionStepActor> CreateActorProxy(Guid userId, string xPath) =>
            _serviceClientFactory.CreateActorProxy<ICollectionStepActor>(new ActorId($"{userId}_{xPath}"), ServiceUri);

        private IServiceClientProxy<ICollectionStepActor> CreateActorProxy(Guid stepId) =>
            _serviceClientFactory.CreateActorProxy<ICollectionStepActor>(new ActorId(stepId), ServiceUri);
    }
}