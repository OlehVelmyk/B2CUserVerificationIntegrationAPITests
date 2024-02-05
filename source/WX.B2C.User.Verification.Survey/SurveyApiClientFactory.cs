using System;
using Microsoft.Extensions.Http;
using Serilog;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.ServiceFabric.Interfaces;
using WX.Survey.Api.Client;

namespace WX.B2C.User.Verification.Survey
{
    public interface ISurveyApiClientFactory
    {
        ISurveyApiClient Create();
    }

    internal class SurveyApiClientFactory : ISurveyApiClientFactory
    {
        private readonly IFabricAddressResolver _fabricAddressResolver;
        private readonly IOperationContextProvider _operationContextProvider;
        private readonly IWxEnvironment _environmentConfig;
        private readonly IAppConfig _appConfig;
        private readonly ILogger _logger;

        public SurveyApiClientFactory(
            IFabricAddressResolver fabricAddressResolver,
            IOperationContextProvider operationContextProvider,
            IWxEnvironment environmentConfig,
            IAppConfig appConfig,
            ILogger logger)
        {
            _fabricAddressResolver = fabricAddressResolver ?? throw new ArgumentNullException(nameof(fabricAddressResolver));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
            _environmentConfig = environmentConfig ?? throw new ArgumentNullException(nameof(environmentConfig));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _logger = logger?.ForContext<SurveyApiClientFactory>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public ISurveyApiClient Create()
        {
            var baseUri = ResolveBaseApiUri();
            var operationContext = _operationContextProvider.GetContextOrDefault();
            var handler = new PolicyHttpMessageHandler(PolicyBuilder.RetryTransientErrorsPolicy(_logger));
            return new SurveyApiClient(baseUri, handler)
            {
                CorrelationId = operationContext.CorrelationId,
                OperationId = operationContext.OperationId
            };
        }

        private Uri ResolveBaseApiUri()
        {
            var loadBalancerAddress = new Uri(_appConfig.SurveyApiUrl);

            if (_environmentConfig.IsProduction)
                return loadBalancerAddress;

            return _fabricAddressResolver
                   .ResolveServiceAddress(_appConfig.SurveyServiceUrl)
                   .Map(url => new Uri(url))
                   .Match(result => result, () => loadBalancerAddress);
        }
    }
}