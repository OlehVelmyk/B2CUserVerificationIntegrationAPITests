using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Actors
{
    public interface ITaskActor : IActor
    {
        Task<Guid> CreateAsync(NewTaskDto newTaskDto, InitiationDto initiationDto);

        Task AddCollectionStepsAsync(Guid taskId, Guid[] collectionStepsIds, InitiationDto initiationDto);

        Task RemoveCollectionStepAsync(Guid taskId, Guid collectionStepId);

        Task CompleteAsync(Guid taskId, TaskResult result, InitiationDto initiationDto);

        Task IncompleteAsync(Guid taskId, InitiationDto initiationDto);
    }
}