using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.UserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Core.ApiClients;

public class VerificationPublicApiClient : IPublicClient
{
    public VerificationPublicApiClient(string baseUrl, HttpClient client) : base(client, false)
    {
        client.BaseAddress = new Uri(baseUrl);
        BaseUri = new Uri(baseUrl);
    }
}