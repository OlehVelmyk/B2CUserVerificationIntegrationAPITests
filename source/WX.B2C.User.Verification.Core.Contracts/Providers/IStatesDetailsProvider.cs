using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IStatesDetailsProvider
    {
        Task<bool> IsSupportedAsync(string stateCode, string countryCode);
    }
}