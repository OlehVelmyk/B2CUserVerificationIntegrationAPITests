using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
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
    [ActorService(Name = nameof(TaskActor))]
    internal class TaskActor : Microsoft.ServiceFabric.Actors.Runtime.Actor, ITaskActor
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initializes a new instance of Actor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public TaskActor(ActorService actorService, ActorId actorId, ITaskService taskService)
            : base(actorService, actorId)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        public Task<Guid> CreateAsync(NewTaskDto newTaskDto, InitiationDto initiation) =>
            _taskService.CreateAsync(newTaskDto, initiation);

        public Task AddCollectionStepsAsync(Guid taskId, Guid[] collectionStepsIds, InitiationDto initiationDto) =>
            _taskService.AddCollectionStepsAsync(taskId, collectionStepsIds, initiationDto);
    
        public Task RemoveCollectionStepAsync(Guid taskId, Guid collectionStepId) =>
            _taskService.RemoveCollectionStepAsync(taskId, collectionStepId);

        public Task CompleteAsync(Guid taskId, TaskResult result, InitiationDto initiationDto) =>
            _taskService.CompleteAsync(taskId, result, initiationDto);

        public Task IncompleteAsync(Guid taskId, InitiationDto initiationDto) =>
            _taskService.IncompleteAsync(taskId, initiationDto);

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