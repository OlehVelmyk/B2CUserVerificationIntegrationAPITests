using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    public interface IBridgerCredentialsStorage
    {
        Task<string> GetPasswordAsync(string userId);
    }
}