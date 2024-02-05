using System.Threading.Tasks;
using WX.Logging.Attributes;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IBridgerCredentialsService
    {
        Task<int> GetDaysUntilPasswordExpiresAsync(string userId);

        Task UpdateAsync(string userId,  [SecureParam] string newPassword, bool propagate = true);
    }
}