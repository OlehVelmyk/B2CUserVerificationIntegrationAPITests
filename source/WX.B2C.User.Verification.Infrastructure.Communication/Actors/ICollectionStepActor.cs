using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Actors
{
    public interface ICollectionStepActor : IActor
    {
        Task<Guid> RequestAsync(Guid userId, NewCollectionStepDto newStepDto, InitiationDto initiationDto);

        Task ReviewAsync(Guid stepId, CollectionStepReviewResult reviewResult, InitiationDto initiationDto);

        Task SubmitAsync(Guid stepId, InitiationDto initiationDto);

        Task RemoveAsync(Guid stepId, InitiationDto initiationDto);

        Task UpdateAsync(Guid stepId, CollectionStepPatch collectionStepPatch, InitiationDto initiationDto);

        Task CancelAsync(Guid stepId, InitiationDto initiationDto);
    }
}