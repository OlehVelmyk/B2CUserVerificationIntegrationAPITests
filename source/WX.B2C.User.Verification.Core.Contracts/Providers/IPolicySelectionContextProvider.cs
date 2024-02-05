using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Providers
{
    public interface IPolicySelectionContextProvider
    {
        Task<VerificationPolicySelectionContext> GetVerificationContextAsync(Guid userId);

        Task<ValidationPolicySelectionContext> GetValidationContextAsync(Guid userId);

        Task<MonitoringPolicySelectionContext> GetMonitoringContextAsync(Guid userId);
    }
}