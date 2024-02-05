using System.Net.Http;

namespace WX.B2C.User.Verification.Onfido.Client
{
    public interface IOnfidoPolicyFactory
    {
        DelegatingHandler Create();
    }
}
