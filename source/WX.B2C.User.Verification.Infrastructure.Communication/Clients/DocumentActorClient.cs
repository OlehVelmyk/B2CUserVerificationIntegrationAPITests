using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class DocumentActorClient : IDocumentService
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/ProfileActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public DocumentActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task SubmitAsync(Guid userId, SubmitDocumentDto submitDocumentDto, InitiationDto initiationDto) =>
            CreateActorProxy(userId).Execute(actor => actor.SubmitDocumentAsync(userId, submitDocumentDto, initiationDto));

        public Task ArchiveAsync(Guid userId, Guid documentId, InitiationDto initiationDto) =>
            CreateActorProxy(userId).Execute(actor => actor.ArchiveDocumentAsync(userId, documentId, initiationDto));

        private IServiceClientProxy<IProfileActor> CreateActorProxy(Guid actorId) =>
            _serviceClientFactory.CreateActorProxy<IProfileActor>(new ActorId(actorId), ServiceUri);
    }
}