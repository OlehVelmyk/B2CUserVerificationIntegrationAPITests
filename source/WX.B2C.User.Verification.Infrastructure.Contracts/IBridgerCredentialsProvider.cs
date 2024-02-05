using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Infrastructure.Contracts
{
    public interface IBridgerCredentialsProvider
    {
        Task<string> GetPasswordAsync(string userId);
    }
}