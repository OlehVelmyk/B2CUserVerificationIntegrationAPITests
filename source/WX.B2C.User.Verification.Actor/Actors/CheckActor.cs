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
    [ActorService(Name = nameof(CheckActor))]
    internal class CheckActor
        : Microsoft.ServiceFabric.Actors.Runtime.Actor,
          ICheckActor
    {
        private readonly ICheckService _checkService;

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public CheckActor(ActorService actorService, ActorId actorId, ICheckService checkService)
            : base(actorService, actorId)
        {
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
        }

        public Task RequestAsync(Guid userId, NewCheckDto newCheckDto, InitiationDto initiation) => 
            _checkService.RequestAsync(userId, newCheckDto, initiation);

        public Task StartExecutionAsync(Guid checkId, CheckExecutionContextDto executionContextDto) => 
            _checkService.StartExecutionAsync(checkId, executionContextDto);

        public Task FinishExecutionAsync(Guid checkId, CheckExecutionResultDto executionResultDto) =>
            _checkService.FinishExecutionAsync(checkId, executionResultDto);

        public Task SaveProcessingResultAsync(Guid checkId, CheckProcessingResultDto processingResultDto) => 
            _checkService.SaveProcessingResultAsync(checkId, processingResultDto);

        public Task SaveErrorResultAsync(Guid checkId, CheckErrorResultDto errorResultDto) => 
            _checkService.SaveErrorResultAsync(checkId, errorResultDto);

        public Task CancelAsync(Guid checkId) => _checkService.CancelAsync(checkId);

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