using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Actors
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IApplicationActor : IActor
    {
        [Obsolete("Remove after release 'Country change flow'")]
        Task RegisterAsync(Guid userId, NewVerificationApplicationDto newApplication);

        Task RegisterV2Async(Guid userId, NewVerificationApplicationDto newApplication, InitiationDto initiationDto);
        
        Task AddRequiredTasksAsync(Guid applicationId, Guid[] taskVariantId, InitiationDto initiationDto);

        Task ApproveAsync(Guid applicationId, InitiationDto initiationDto);

        Task RejectAsync(Guid applicationId, InitiationDto initiationDto);

        Task RequestReviewAsync(Guid applicationId, InitiationDto initiationDto);

        Task RevertDecisionAsync(Guid applicationId, InitiationDto initiationDto);

        Task AutomateAsync(Guid applicationId, InitiationDto initiationDto);
    }
}