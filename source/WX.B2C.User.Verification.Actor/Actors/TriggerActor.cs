using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Actor.Actors
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    [ActorService(Name = nameof(TriggerActor))]
    internal class TriggerActor : Microsoft.ServiceFabric.Actors.Runtime.Actor, ITriggerActor
    {
        private readonly ITriggerService _triggerService;

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public TriggerActor(ActorService actorService, ActorId actorId, ITriggerService triggerService)
            : base(actorService, actorId)
        {
            _triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
        }

        public Task ScheduleAsync(Guid triggerVariantId, Guid userId, Guid applicationId) =>
            _triggerService.ScheduleAsync(triggerVariantId, userId, applicationId);

        public Task UnscheduleAsync(Guid triggerId) =>
            _triggerService.UnscheduleAsync(triggerId);

        public Task FireAsync(Guid triggerId, TriggerContextDto context) =>
            _triggerService.FireAsync(triggerId, context);

        public Task CompleteAsync(Guid triggerId) =>
            _triggerService.CompleteAsync(triggerId);
    }
}