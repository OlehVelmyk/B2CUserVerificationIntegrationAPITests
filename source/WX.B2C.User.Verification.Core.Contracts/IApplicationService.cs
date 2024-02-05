using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IApplicationService
    {
        Task RegisterAsync(Guid userId, NewVerificationApplicationDto newApplication, InitiationDto initiationDto);

        Task AddRequiredTasksAsync(Guid applicationId, Guid[] taskIds, InitiationDto initiationDto);

        Task ApproveAsync(Guid applicationId, InitiationDto initiationDto);

        Task RejectAsync(Guid applicationId, InitiationDto initiationDto);

        Task RequestReviewAsync(Guid applicationId, InitiationDto initiationDto);

        Task RevertDecisionAsync(Guid applicationId, InitiationDto initiationDto);

        Task AutomateAsync(Guid applicationId, InitiationDto initiationDto);
    }
}
