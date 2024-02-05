using System.Net.Http.Headers;
using WX.B2C.User.Profile.Api.Client.Admin;
using WX.B2C.User.Profile.Api.Client.Admin.Contracts;
using WX.B2C.User.Verification.Integration.Tests.Core.DelegatingHandlers;
using WX.B2C.User.Verification.Integration.Tests.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Factories;

internal class ProfileAdminApiClientFactory
{
    private readonly Uri _baseUri;
    private readonly AdminCredentialsProvider _credentialsProvider;
    private readonly RetryPolicyHandlerFactory _retryPolicyHandlerFactory;
    private readonly LogDelegatingHandlerFactory _logDelegatingHandlerFactory;

    public ProfileAdminApiClientFactory(Uri baseUri, 
                                        AdminCredentialsProvider credentialsProvider,
                                        RetryPolicyHandlerFactory retryPolicyHandlerFactory,
                                        LogDelegatingHandlerFactory logDelegatingHandlerFactory)
    {
        _baseUri = baseUri;
        _credentialsProvider = credentialsProvider;
        _retryPolicyHandlerFactory = retryPolicyHandlerFactory;
        _logDelegatingHandlerFactory = logDelegatingHandlerFactory;
    }

    public async Task<IUserProfileClient> Create()
    {
        var retryPolicyHandler = _retryPolicyHandlerFactory.Create();
        var logDelegatingHandler = _logDelegatingHandlerFactory.Create();
        var (token, tokenType) = await _credentialsProvider.GetAsync();
        
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;

        var client = HttpClientFactory.Create(handler, retryPolicyHandler, logDelegatingHandler);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);

        return new UserProfileClient(_baseUri.ToString(), client);
    }
}
