using WX.B2C.Survey.Api.Public.Client;

namespace WX.B2C.User.Verification.Integration.Tests.Core.ApiClients;

internal class SurveyPublicApiClient : B2CSurveyApiClient
{
    public SurveyPublicApiClient(string baseUrl, HttpClient client) : base(client, false)
    {
        client.BaseAddress = new Uri(baseUrl);
        BaseUri = new Uri(baseUrl);
    }
}