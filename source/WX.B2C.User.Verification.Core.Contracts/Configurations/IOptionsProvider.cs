using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IOptionsProvider
    {
        public Task<T> GetAsync<T>() where T : Option;
    }
}