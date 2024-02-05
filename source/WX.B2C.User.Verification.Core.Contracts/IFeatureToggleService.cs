using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IFeatureToggleService
    {
        Task<bool> IsVerificationAvailableAsync(VerificationPolicySelectionContext context);
    }
}