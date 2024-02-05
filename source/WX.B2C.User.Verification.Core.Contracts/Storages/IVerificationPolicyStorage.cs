using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IVerificationPolicyStorage
    {
        Task<VerificationPolicyDto[]> GetAsync(IEnumerable<Guid> policyIds);

        Task<VerificationPolicyDto> GetAsync(Guid policyId);

        Task<VerificationPolicyDto> GetAsync(VerificationPolicySelectionContext selectionContext);

        Task<Guid> GetIdAsync(VerificationPolicySelectionContext selectionContext);

        Task<Guid?> FindIdAsync(VerificationPolicySelectionContext selectionContext);

        Task<CheckVariantInfo[]> GetChecksInfoAsync(Guid[] variantIds = null);

        Task<VariantNameDto[]> GetCheckVariantNamesAsync(Guid[] variantIds);

        Task<CheckFailPolicy> FindCheckFailPolicyAsync(Guid checkVariantId);

        Task<AutoCompletePolicy> GetTaskAutoCompletePolicyAsync(Guid taskVariantId);

        Task<TaskVariantDto> GetTaskVariantAsync(Guid taskVariantId);

        Task<TaskVariantDto[]> GetTaskVariantsAsync(Guid[] taskVariantIds);

        Task<CheckVariantInfo> FindCheckInfoAsync(Guid variantId);
    }
}