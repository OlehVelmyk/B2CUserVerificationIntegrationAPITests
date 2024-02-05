using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IApplicationBuilder
    {
        Task BuildAsync(ApplicationDto application, VerificationPolicyDto policy);
    }
}