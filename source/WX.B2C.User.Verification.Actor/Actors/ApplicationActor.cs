using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
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
    [ActorService(Name = nameof(ApplicationActor))]
    internal class ApplicationActor
        : Microsoft.ServiceFabric.Actors.Runtime.Actor,
          IApplicationActor
    {
        private readonly IApplicationService _applicationService;

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public ApplicationActor(ActorService actorService, ActorId actorId, IApplicationService applicationService)
            : base(actorService, actorId)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
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

        public Task RegisterAsync(Guid userId, NewVerificationApplicationDto newApplication) =>
            _applicationService.RegisterAsync(userId, newApplication, InitiationDto.CreateUser());

        public Task RegisterV2Async(Guid userId, NewVerificationApplicationDto newApplication, InitiationDto initiationDto) =>
            _applicationService.RegisterAsync(userId, newApplication, initiationDto);

        public Task AddRequiredTasksAsync(Guid applicationId, Guid[] taskIds, InitiationDto initiationDto) =>
            _applicationService.AddRequiredTasksAsync(applicationId, taskIds, initiationDto);

        public Task ApproveAsync(Guid applicationId, InitiationDto initiationDto) =>
            _applicationService.ApproveAsync(applicationId, initiationDto);

        public Task RejectAsync(Guid applicationId, InitiationDto initiationDto) =>
            _applicationService.RejectAsync(applicationId, initiationDto);

        public Task RequestReviewAsync(Guid applicationId, InitiationDto initiationDto) =>
            _applicationService.RequestReviewAsync(applicationId, initiationDto);

        public Task RevertDecisionAsync(Guid applicationId, InitiationDto initiationDto) =>
            _applicationService.RevertDecisionAsync(applicationId, initiationDto);

        public Task AutomateAsync(Guid applicationId, InitiationDto initiationDto) =>
            _applicationService.AutomateAsync(applicationId, initiationDto);
    }
}