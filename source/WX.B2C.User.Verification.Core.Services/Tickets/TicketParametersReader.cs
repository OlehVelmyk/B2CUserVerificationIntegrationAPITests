using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using Parameter = System.Collections.Generic.KeyValuePair<string, object>; 
using Parameters = System.Collections.Generic.Dictionary<string, object>; 

namespace WX.B2C.User.Verification.Core.Services.Tickets
{
    public interface ITicketParametersReader
    {
        Task<IReadOnlyDictionary<string, object>> ReadParametersAsync(Guid userId,
                                                                      IReadOnlyCollection<string> requestedParameters,
                                                                      IReadOnlyDictionary<string, object> providedParameters);
    }

    internal class TicketParametersReader : ITicketParametersReader
    {
        private readonly IProfileProviderFactory _profileProviderFactory;
        private readonly IOptionProvider<TicketParametersMappingOption> _optionProvider;
        private readonly IOperationContextProvider _operationContextProvider;
        private readonly IExternalProfileStorage _externalProfileStorage;
        private readonly IExternalLinkProvider _externalLinkProvider;
        private readonly ISystemClock _systemClock;

        public TicketParametersReader(IProfileProviderFactory profileProviderFactory,
                                      IOptionProvider<TicketParametersMappingOption> optionProvider,
                                      IOperationContextProvider operationContextProvider,
                                      IExternalProfileStorage externalProfileStorage,
                                      IExternalLinkProvider externalLinkProvider,
                                      ISystemClock systemClock)
        {
            _profileProviderFactory = profileProviderFactory ?? throw new ArgumentNullException(nameof(profileProviderFactory));
            _optionProvider = optionProvider ?? throw new ArgumentNullException(nameof(optionProvider));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
            _externalProfileStorage = externalProfileStorage ?? throw new ArgumentNullException(nameof(externalProfileStorage));
            _externalLinkProvider = externalLinkProvider ?? throw new ArgumentNullException(nameof(externalLinkProvider));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public async Task<IReadOnlyDictionary<string, object>> ReadParametersAsync(Guid userId,
                                                                                   IReadOnlyCollection<string> requestedParameters,
                                                                                   IReadOnlyDictionary<string, object> providedParameters)
        {
            var parameters = providedParameters != null
                ? new Parameters(providedParameters)
                : new Parameters();
            // Each ticket must have UserId by default for mapping to custom field 
            EnrichWithUserId(parameters, userId);
            var required = requestedParameters.Except(parameters.Keys).ToArray();
            if (required.Length == 0)
                return parameters;
            
            var mapping = await _optionProvider.GetAsync();
            var parametersToRead = required.Select(mapping.GetMapping);
            var (functionalParameters, xPathParameters) = SplitByType(parametersToRead);
            var functionalParametersValues = await ReadFunctionValuesAsync(functionalParameters, userId);
            var xPathParametersValues = await ReadXPathValuesAsync(xPathParameters, userId);

            return parameters.Concat(functionalParametersValues)
                             .Concat(xPathParametersValues)
                             .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void EnrichWithUserId(Parameters parameters, Guid userId) =>
            parameters[TemplateParameters.UserId] = userId;

        private (ParameterMapping[] functionalParameters, ParameterMapping[] xPathParameters) SplitByType(
            IEnumerable<ParameterMapping> parameters)
        {
            var xPathParameter = parameters.Where(IsXPath).ToArray();
            var functionalParameters = parameters.Where(mapping => !IsXPath(mapping)).ToArray();
            return (functionalParameters, xPathParameter);

            bool IsXPath(ParameterMapping mapping) =>
                mapping.Source.Contains(".");
        }

        private Task<Parameter[]> ReadFunctionValuesAsync(ParameterMapping[] requestedParameters, Guid userId) =>
            requestedParameters.Foreach(async parameter =>
            {
                var value = await EvaluateFunctionAsync(parameter.Source, userId);
                return new Parameter(parameter.Name, value);
            });

        private async Task<Parameter[]> ReadXPathValuesAsync(ParameterMapping[] requestedParameters, Guid userId)
        {
            if (requestedParameters.Length == 0)
                return Array.Empty<Parameter>();

            var xPathes = requestedParameters.Select(mapping => mapping.Source);
            var profileDataProvider = _profileProviderFactory.Create(userId);
            var xpathValues = await profileDataProvider.ReadAsync(xPathes);
            return requestedParameters
                   .Select(parameterMapping =>
                   {
                       var xpathValue = xpathValues[parameterMapping.Source];
                       return new Parameter(parameterMapping.Name, xpathValue);
                   })
                   .ToArray();
        }

        private async Task<object> EvaluateFunctionAsync(string variableName, Guid userId)
        {
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));

            return variableName switch
            {
                TemplateParameters.UserId            => userId,
                TemplateParameters.CurrentDateTime   => _systemClock.GetDate(),
                TemplateParameters.CorrelationId     => GetCorrelationId(),
                TemplateParameters.PassFortLink      => await GetExternalLinkAsync(userId, ExternalProviderType.PassFort),
                TemplateParameters.OnfidoProfileLink => await GetExternalLinkAsync(userId, ExternalProviderType.Onfido),
                _                                    => throw new ArgumentOutOfRangeException(nameof(variableName), variableName, null)
            };
        }

        private string GetCorrelationId() =>
            _operationContextProvider.GetContextOrDefault()?.CorrelationId.ToString();

        private async Task<string> GetExternalLinkAsync(Guid userId, ExternalProviderType externalProviderType)
        {
            var externalId = await _externalProfileStorage.GetExternalIdAsync(userId, externalProviderType);
            return _externalLinkProvider.Get(externalId, externalProviderType);
        }
    }
}