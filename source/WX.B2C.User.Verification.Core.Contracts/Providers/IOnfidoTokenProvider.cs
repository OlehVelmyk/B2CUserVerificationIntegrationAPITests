using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IOnfidoTokenProvider
    {
        Task<string> CreateWebTokenAsync(string applicantId);

        Task<string> CreateMobileTokenAsync(string applicantId, string applicationId);
    }
}
