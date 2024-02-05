using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class ExternalProfileActorClient : IExternalProfileProvider
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/ExternalProfileActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public ExternalProfileActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task<ExternalProfileDto> GetOrCreateAsync(Guid userId, ExternalProviderType externalProviderType) =>
            CreateActorProxy(userId).Execute(actor => actor.GetOrCreateAsync(userId, externalProviderType));

        private IServiceClientProxy<IExternalProfileActor> CreateActorProxy(Guid userId) =>
            _serviceClientFactory.CreateActorProxy<IExternalProfileActor>(new ActorId(userId), ServiceUri);
    }
}