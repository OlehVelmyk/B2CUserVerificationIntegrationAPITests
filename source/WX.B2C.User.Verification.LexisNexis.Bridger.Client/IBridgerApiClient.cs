using System.Threading.Tasks;
using BridgerReference;

namespace WX.B2C.User.Verification.LexisNexis.Bridger.Client
{
    public interface IBridgerApiClient
    {
        Task<int> GetDaysUntilPasswordExpiresAsync(string userId, string password);

        Task<bool> ChangeAccountPasswordAsync(string userId, string password, string newPassword);

        Task<SearchResults> SearchAsync(SearchInput input, string predefinedSearchName);
    }
}
