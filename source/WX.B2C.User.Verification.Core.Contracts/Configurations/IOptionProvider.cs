using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public interface IOptionProvider<T>: IOptionProvider where T : Option
    {
        public Task<T> GetAsync();
    }

    public interface IOptionProvider
    {

    }
}
