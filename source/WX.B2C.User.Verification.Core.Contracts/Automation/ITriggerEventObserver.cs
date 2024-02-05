using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Automation
{
    public interface ITriggerEventObserver
    {
        Task OnApplicationStateChanged(Guid userId,
                                       Guid applicationId,
                                       ApplicationState previousState,
                                       ApplicationState newState);

        Task OnDetailsChanged(Guid userId);

        Task OnTriggerScheduled(Guid triggerVariantId, Guid triggerId);

        Task OnTriggerReadyToFire(Guid triggerId);

        Task OnActionsRequested(Guid userId, string[] actions, Guid triggersPolicyId);

        /// <summary>
        /// TODO WRXB-10546 Remove in phase 2 when all users will be migrated
        /// </summary>
        Task OnApplicationAutomated(Guid userId, Guid applicationId);

        Task OnTriggerCompleted(Guid userId, Guid applicationId, Guid variantId);
    }
}