using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;
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
    [ActorService(Name = nameof(ExternalProfileActor))]
    public class ExternalProfileActor : 
        Microsoft.ServiceFabric.Actors.Runtime.Actor,
        IExternalProfileActor
    {
        private readonly IExternalProfileProvider _externalProfileProvider;

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ExternalProfileActor(ActorService actorService, ActorId actorId, IExternalProfileProvider externalProfileProvider)
            : base(actorService, actorId)
        {
            _externalProfileProvider = externalProfileProvider ?? throw new ArgumentNullException(nameof(externalProfileProvider));
        }

        public Task<ExternalProfileDto> GetOrCreateAsync(Guid userId, ExternalProviderType externalProviderType) =>
            _externalProfileProvider.GetOrCreateAsync(userId, externalProviderType);

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