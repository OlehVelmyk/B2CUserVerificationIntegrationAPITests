using System;
using Microsoft.Extensions.Http;
using Serilog;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.IpStack.Client;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.IpStack
{
    internal interface IIpStackApiClientFactory
    {
        IIpStackApiClient Create();
    }

    internal class IpStackApiClientFactory : IIpStackApiClientFactory
    {
        private readonly IAppConfig _appConfig;
        private readonly ILogger _logger;

        public IpStackApiClientFactory(IAppConfig appConfig, ILogger logger)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _logger = logger?.ForContext<IpStackApiClientFactory>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public IIpStackApiClient Create()
        {
            var serviceAddress = new Uri(_appConfig.IpStackApiUrl);
            var accessKey = _appConfig.IpStackAccessKey.UnSecure();
            var responseCodeHandler = new ResponseCodeAdapter();
            var retryHandler = new PolicyHttpMessageHandler(PolicyBuilder.RetryTransientErrorsPolicy(_logger));
            return new IpStackApiClient(serviceAddress, retryHandler, responseCodeHandler) { AccessKey = accessKey };
        }
    }
}