using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Automation
{
    public interface ITaskEventObserver
    {
        Task OnCheckPassed(Guid checkId);

        Task OnCheckFailed(Guid checkId);

        Task OnCollectionStepCompleted(Guid collectionStepId);

        Task OnCollectionStepRequired(Guid collectionStepId);
    }
}
