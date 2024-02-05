using System;
using System.ServiceModel;
using BridgerReference;

namespace WX.B2C.User.Verification.LexisNexis.Bridger.Client
{
    public interface IBridgerClientsFactory
    {
        AccountClient CreateAccountClient(Uri baseUri, BasicHttpsBinding binding);

        SearchClient CreateSearchClient(Uri baseUri, BasicHttpsBinding binding);
    }

    internal class BridgerClientsFactory : IBridgerClientsFactory
    {
        public AccountClient CreateAccountClient(Uri baseUri, BasicHttpsBinding binding)
        {
            var baseUrl = baseUri.AbsoluteUri;
            var url = new Uri(new Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), "Account");

            return new AccountClient(binding, new EndpointAddress(url));
        }

        public SearchClient CreateSearchClient(Uri baseUri, BasicHttpsBinding binding)
        {
            var baseUrl = baseUri.AbsoluteUri;
            var url = new Uri(new Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/")), "Search");

            return new SearchClient(binding, new EndpointAddress(url));
        }
    }
}
