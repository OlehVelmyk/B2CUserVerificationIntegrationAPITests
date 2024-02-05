using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Automation
{
    public interface IApplicationEventObserver
    {
        Task OnTaskIncomplete(Guid taskId);

        Task OnTaskPassed(Guid taskId);

        Task OnTaskFailed(Guid taskId);

        Task OnCheckFailed(Guid userId, Guid checkId);

        Task OnDetailsChanged(Guid userId, string[] changes);

        Task OnPoiIssuingCountryChanged(Guid userId, string poiIssuingCountry);

        Task OnTriggerCompleted(Guid applicationId, Guid triggerId);
    }
}