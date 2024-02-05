using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Commands.ServiceClients;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Communication.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Clients
{
    internal class TaskActorClient : ITaskServiceClient
    {
        private static readonly Uri ServiceUri = new("fabric:/WX.B2C.User.Verification/TaskActor");
        private readonly IServiceClientFactory _serviceClientFactory;

        public TaskActorClient(IServiceClientFactory serviceClientFactory)
        {
            _serviceClientFactory = serviceClientFactory ?? throw new ArgumentNullException(nameof(serviceClientFactory));
        }

        public Task<Guid> CreateAsync(NewTaskDto newTaskDto, InitiationDto initiationDto) =>
            CreateActorProxy(newTaskDto.UserId, newTaskDto.VariantId).Execute(actor => actor.CreateAsync(newTaskDto, initiationDto));

        public Task AddCollectionStepsAsync(Guid taskId, Guid[] collectionStepsIds, InitiationDto initiationDto) =>
            CreateActorProxy(taskId).Execute(actor => actor.AddCollectionStepsAsync(taskId, collectionStepsIds, initiationDto));

        public Task RemoveCollectionStepAsync(Guid taskId, Guid collectionStepId) =>
            CreateActorProxy(taskId).Execute(actor => actor.RemoveCollectionStepAsync(taskId, collectionStepId));

        public Task CompleteAsync(Guid taskId, TaskResult result, InitiationDto initiationDto) =>
            CreateActorProxy(taskId).Execute(actor => actor.CompleteAsync(taskId, result, initiationDto));

        public Task IncompleteAsync(Guid taskId, InitiationDto initiationDto) =>
            CreateActorProxy(taskId).Execute(actor => actor.IncompleteAsync(taskId, initiationDto));

        private IServiceClientProxy<ITaskActor> CreateActorProxy(Guid taskId) =>
            _serviceClientFactory.CreateActorProxy<ITaskActor>(new ActorId(taskId), ServiceUri);

        private IServiceClientProxy<ITaskActor> CreateActorProxy(Guid userId, Guid taskVariantId) =>
            _serviceClientFactory.CreateActorProxy<ITaskActor>(new ActorId($"{userId}-{taskVariantId}"), ServiceUri);
    }
}