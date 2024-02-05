using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Automation.Services.Approve;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using Tasks = WX.B2C.User.Verification.Extensions.TaskExtensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface IApplicationApprovalService
    {
        Task<ApprovalBlocker[]> GetBlockersAsync(ApplicationDto application);
    }

    internal class ApplicationApprovalService : IApplicationApprovalService
    {
        private readonly IEnumerable<IApplicationApprovalCondition> _approvalConditions;
        private readonly IProfileStorage _profileStorage;
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly ITriggerStorage _triggerStorage;

        public ApplicationApprovalService(
            IEnumerable<IApplicationApprovalCondition> approvalConditions,
            IProfileStorage profileStorage,
            IVerificationPolicyStorage policyStorage,
            ITriggerStorage triggerStorage)
        {
            _approvalConditions = approvalConditions ?? throw new ArgumentNullException(nameof(approvalConditions));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _triggerStorage = triggerStorage ?? throw new ArgumentNullException(nameof(triggerStorage));
        }

        public async Task<ApprovalBlocker[]> GetBlockersAsync(ApplicationDto application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            var context = await CreateContextAsync(application);

            return _approvalConditions
                   .Aggregate(Enumerable.Empty<ApprovalBlocker>(), (results, condition) => 
                                  condition.IsSatisfied(context, out var blockers)
                                  ? results
                                  : results.Concat(blockers))
                   .ToArray();
        }

        private async Task<ApplicationApprovalContext> CreateContextAsync(ApplicationDto application)
        {
            var taskVariantIds = application.Tasks.Select(x => x.VariantId).ToArray();

            var (verificationDetails, taskVariants, triggers) = await Tasks.WhenAll(
                _profileStorage.GetVerificationDetailsAsync(application.UserId),
                _policyStorage.GetTaskVariantsAsync(taskVariantIds),
                _triggerStorage.GetAllAsync(application.Id));

            return new ApplicationApprovalContext
            {
                RiskLevel = verificationDetails.RiskLevel,
                ApplicationTasks = application.Tasks,
                TaskVariants = taskVariants,
                Triggers = triggers,
            };
        }
    }
}