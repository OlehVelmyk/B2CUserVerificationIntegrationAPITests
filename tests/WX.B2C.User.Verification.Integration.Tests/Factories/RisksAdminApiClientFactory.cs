﻿using Microsoft.Rest;
using WX.B2C.Risks.Api.Admin.Client;
using WX.B2C.User.Verification.Integration.Tests.Core.DelegatingHandlers;
using WX.B2C.User.Verification.Integration.Tests.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Factories;

internal class RisksAdminApiClientFactory
{
    private readonly Uri _baseUri;
    private readonly AdminCredentialsProvider _credentialsProvider;
    private readonly RetryPolicyHandlerFactory _retryPolicyHandlerFactory;
    private readonly LogDelegatingHandlerFactory _logDelegatingHandlerFactory;

    public RisksAdminApiClientFactory(Uri baseUri, 
                                      AdminCredentialsProvider credentialsProvider,
                                      RetryPolicyHandlerFactory retryPolicyHandlerFactory,
                                      LogDelegatingHandlerFactory logDelegatingHandlerFactory)
    {
        _baseUri = baseUri;
        _credentialsProvider = credentialsProvider;
        _retryPolicyHandlerFactory = retryPolicyHandlerFactory;
        _logDelegatingHandlerFactory = logDelegatingHandlerFactory;
    }

    public async Task<IRisksAdminApiClient> Create()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
        var (token, tokenType) = await _credentialsProvider.GetAsync();
        var credentials = new TokenCredentials(token, tokenType);
        var retryPolicyHandler = _retryPolicyHandlerFactory.Create();
        var logDelegatingHandler = _logDelegatingHandlerFactory.Create();

        return new RisksAdminApiClient(_baseUri, credentials, handler, logDelegatingHandler, retryPolicyHandler);
    }
}
