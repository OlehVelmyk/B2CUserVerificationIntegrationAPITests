using System.Threading.Tasks;
 
namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Abstraction
{
    internal interface IHttpImposter
    {
        bool IsActive { get; }

        Task ConfigureDefaultAsync();

        Task ResetAsync();

        Task RemoveAsync();
    }
}
