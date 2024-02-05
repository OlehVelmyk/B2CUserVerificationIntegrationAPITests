using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class ProfileActorClient : IProfileService
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/ProfileActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public ProfileActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task UpdateAsync(Guid userId, PersonalDetailsPatch patch, InitiationDto initiationDto) =>
            CreateActorProxy(userId).Execute(actor => actor.UpdatePersonalDetailsAsync(userId, patch, initiationDto));

        public Task UpdateAsync(Guid userId, VerificationDetailsPatch patch, InitiationDto initiationDto) =>
            CreateActorProxy(userId).Execute(actor => actor.UpdateVerificationDetailsAsync(userId, patch, initiationDto));

        public Task UpdateAsync(Guid userId, ExternalProfileDto externalProfileDto) =>
            CreateActorProxy(userId).Execute(actor => actor.UpdateExternalProfileAsync(userId, externalProfileDto));

        private IServiceClientProxy<IProfileActor> CreateActorProxy(Guid actorId) =>
            _serviceClientFactory.CreateActorProxy<IProfileActor>(new ActorId(actorId), ServiceUri);
    }
}