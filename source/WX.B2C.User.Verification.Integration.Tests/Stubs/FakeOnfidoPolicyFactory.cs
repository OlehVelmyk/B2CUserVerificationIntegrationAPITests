using System.Net.Http;
using WX.B2C.User.Verification.Onfido.Client;

namespace WX.B2C.User.Verification.Component.Tests.Stubs
{
    class FakeOnfidoPolicyFactory : IOnfidoPolicyFactory
    {
        public DelegatingHandler Create() => null;
    }
}