using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class TriggerActorClient : ITriggerService
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/TriggerActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public TriggerActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task ScheduleAsync(Guid triggerVariantId, Guid userId, Guid applicationId) =>
            CreateActorProxy(triggerVariantId, applicationId).Execute(actor => actor.ScheduleAsync(triggerVariantId, userId, applicationId));

        public Task UnscheduleAsync(Guid triggerId) =>
            CreateActorProxy(triggerId).Execute(actor => actor.UnscheduleAsync(triggerId));

        public Task FireAsync(Guid triggerId, TriggerContextDto context) =>
            CreateActorProxy(triggerId).Execute(actor => actor.FireAsync(triggerId, context));

        public Task CompleteAsync(Guid triggerId) =>
            CreateActorProxy(triggerId).Execute(actor => actor.CompleteAsync(triggerId));

        private IServiceClientProxy<ITriggerActor> CreateActorProxy(Guid triggerVariantId, Guid applicationId) =>
            _serviceClientFactory.CreateActorProxy<ITriggerActor>(new ActorId($"{triggerVariantId}-{applicationId}"), ServiceUri);

        private IServiceClientProxy<ITriggerActor> CreateActorProxy(Guid triggerId) =>
            _serviceClientFactory.CreateActorProxy<ITriggerActor>(new ActorId(triggerId), ServiceUri);
    }
}