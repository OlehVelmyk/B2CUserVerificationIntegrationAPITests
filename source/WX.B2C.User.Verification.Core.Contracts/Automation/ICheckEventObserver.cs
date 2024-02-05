using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Automation
{
    public interface ICheckEventObserver
    {
        Task OnProfileChanged(Guid userId, string[] xPathes);

        Task OnCollectionStepRequested(Guid userId, string xPath);

        Task OnCollectionStepCompleted(Guid userId, string xPath);

        Task OnCheckCreated(Guid userId, Guid checkId);

        Task OnCheckPerformed(Guid checkId);

        Task OnCheckPassed(Guid checkId);

        Task OnCheckFailed(Guid checkId);

        /// <summary>
        /// TODO WRXB-10546 remove in phase 2 when all users finally migrated
        /// </summary>
        Task OnApplicationAutomatedAsync(Guid userId);

        Task OnChecksCreated(Guid userId, Guid[] checks);

        Task OnTaskCompleted(Guid userId);
    }
}
