using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.DataCollection;
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
    [ActorService(Name = nameof(CollectionStepActor))]
    internal class CollectionStepActor
        : Microsoft.ServiceFabric.Actors.Runtime.Actor,
          ICollectionStepActor
    {
        private readonly ICollectionStepService _collectionStepService;

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public CollectionStepActor(ActorService actorService, ActorId actorId, ICollectionStepService collectionStepService)
            : base(actorService, actorId)
        {
            _collectionStepService = collectionStepService ?? throw new ArgumentNullException(nameof(collectionStepService));
        }

        public Task<Guid> RequestAsync(Guid userId, NewCollectionStepDto newStepDto, InitiationDto initiationDto) =>
            _collectionStepService.RequestAsync(userId, newStepDto, initiationDto);

        public Task ReviewAsync(Guid stepId, CollectionStepReviewResult reviewResult, InitiationDto initiationDto) =>
            _collectionStepService.ReviewAsync(stepId, reviewResult, initiationDto);

        public Task SubmitAsync(Guid stepId, InitiationDto initiationDto) =>
            _collectionStepService.SubmitAsync(stepId, initiationDto);

        public Task RemoveAsync(Guid stepId, InitiationDto initiationDto) => 
            _collectionStepService.RemoveAsync(stepId, initiationDto);

        public Task UpdateAsync(Guid stepId, CollectionStepPatch collectionStepPatch, InitiationDto initiationDto) =>
            _collectionStepService.UpdateAsync(stepId, collectionStepPatch, initiationDto);

        public Task CancelAsync(Guid stepId, InitiationDto initiationDto) =>
            _collectionStepService.CancelAsync(stepId, initiationDto);

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;
        }
    }
}