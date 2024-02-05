using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface ICountryDetailsProvider
    {
        Task<string> GetRegionAsync(string alpha2);

        public Task<bool> IsSupportedAsync(string alpha2);

        Task<string> GetAlpha3Async(string alpha2);

        Task<string> FindAlpha3Async(string alpha2);

        Task<string> FindAlpha2Async(string alpha3);
    }
}