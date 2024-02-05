using System;
using Microsoft.Extensions.Http;
using Serilog;
using WX.B2C.Survey.Api.Internal.Client;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.ServiceFabric.Interfaces;

namespace WX.B2C.User.Verification.B2C.Survey
{
    public interface IB2CSurveyApiClientFactory
    {
        IB2CSurveyApiClient Create();
    }

    internal class B2CSurveyApiClientFactory : IB2CSurveyApiClientFactory
    {
        private readonly IFabricAddressResolver _fabricAddressResolver;
        private readonly IOperationContextProvider _operationContextProvider;
        private readonly IWxEnvironment _environmentConfig;
        private readonly IAppConfig _appConfig;
        private readonly ILogger _logger;

        public B2CSurveyApiClientFactory(
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
            _logger = logger?.ForContext<B2CSurveyApiClientFactory>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public IB2CSurveyApiClient Create()
        {
            var baseUri = ResolveBaseApiUri();
            var operationContext = _operationContextProvider.GetContextOrDefault();
            var handler = new PolicyHttpMessageHandler(PolicyBuilder.RetryTransientErrorsPolicy(_logger));
            return new B2CSurveyApiClient(baseUri, handler)
            {
                CorrelationId = operationContext.CorrelationId,
                OperationId = operationContext.OperationId
            };
        }

        private Uri ResolveBaseApiUri()
        {
            var loadBalancerAddress = new Uri(_appConfig.B2CSurveyApiUrl);

            if (_environmentConfig.IsProduction)
                return loadBalancerAddress;

            return _fabricAddressResolver
                   .ResolveServiceAddress(_appConfig.B2CSurveyServiceUrl)
                   .Map(url => new Uri(url))
                   .Match(result => result, () => loadBalancerAddress);
        }
    }
}