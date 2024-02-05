using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Automation
{
    public interface ICollectionStepEventObserver
    {
        Task OnDocumentSubmitted(Guid userId, string[] changes);

        Task OnSurveySubmitted(Guid userId, string[] changes);

        Task OnPersonalDetailsUpdated(Guid userId, string[] changes);

        Task OnVerificationDetailsUpdated(Guid userId, string[] changes);
    }
}
