using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface ICollectionStepService
    {
        Task<Guid> RequestAsync(Guid userId, NewCollectionStepDto newStepDto, InitiationDto initiationDto);

        Task SubmitAsync(Guid stepId, InitiationDto initiationDto);

        Task ReviewAsync(Guid stepId, CollectionStepReviewResult reviewResult, InitiationDto initiationDto);

        Task RemoveAsync(Guid stepId, InitiationDto initiationDto);

        Task UpdateAsync(Guid stepId, CollectionStepPatch collectionStepPatch, InitiationDto initiationDto);

        Task CancelAsync(Guid stepId, InitiationDto initiationDto);
    }
}
