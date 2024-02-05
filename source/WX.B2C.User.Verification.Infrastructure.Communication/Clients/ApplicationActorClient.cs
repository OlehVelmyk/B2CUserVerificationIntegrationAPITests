using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Commands.ServiceClients;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class ApplicationActorClient : IApplicationServiceClient
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/ApplicationActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public ApplicationActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }
        
        public Task RegisterAsync(Guid userId, NewVerificationApplicationDto newApplication, InitiationDto initiation) => 
            CreateActorProxy(userId).Execute(actor => actor.RegisterV2Async(userId, newApplication, initiation));

        public Task AddRequiredTasksAsync(Guid applicationId, Guid[] taskVariantIds, InitiationDto initiationDto) =>
            CreateActorProxy(applicationId).Execute(actor => actor.AddRequiredTasksAsync(applicationId, taskVariantIds, initiationDto));

        /// <summary>
        /// TODO Bad that actor id is application when in RegisterAsync is user id. It means that we can register application in one actor but modify in another.
        /// Maybe this is not a problem.
        /// </summary>
        public Task ApproveAsync(Guid applicationId, InitiationDto initiationDto) =>
            CreateActorProxy(applicationId).Execute(actor => actor.ApproveAsync(applicationId, initiationDto));

        public Task RejectAsync(Guid applicationId, InitiationDto initiationDto) =>
            CreateActorProxy(applicationId).Execute(actor => actor.RejectAsync(applicationId, initiationDto));

        public Task RequestReviewAsync(Guid applicationId, InitiationDto initiationDto) =>
            CreateActorProxy(applicationId).Execute(actor => actor.RequestReviewAsync(applicationId, initiationDto));

        public Task RevertDecisionAsync(Guid applicationId, InitiationDto initiationDto) =>
            CreateActorProxy(applicationId).Execute(actor => actor.RevertDecisionAsync(applicationId, initiationDto));

		public Task AutomateAsync(Guid applicationId, InitiationDto initiationDto) =>
            CreateActorProxy(applicationId).Execute(actor => actor.AutomateAsync(applicationId, initiationDto));
        
        private IServiceClientProxy<IApplicationActor> CreateActorProxy(Guid actorId) =>
            _serviceClientFactory.CreateActorProxy<IApplicationActor>(new ActorId(actorId), ServiceUri);
    }
}