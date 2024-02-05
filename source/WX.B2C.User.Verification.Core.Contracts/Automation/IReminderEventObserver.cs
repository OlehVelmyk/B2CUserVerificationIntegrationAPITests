using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Automation
{
    public interface IReminderEventObserver
    {
        Task OnUserActionsChanged(Guid userId);

        Task OnCollectionStepStateChanged(Guid userId, Guid stepId);

        Task OnCollectionStepSubmitted(Guid userId, Guid stepId);

        Task OnJobFinishedAsync(Guid userId, Guid stepId);

        Task OnApplicationRejected(Guid userId);

        Task OnApplicationReverted(Guid userId);
    }
}