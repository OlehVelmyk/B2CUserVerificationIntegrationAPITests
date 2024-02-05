using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services
{
    internal sealed class CheckProviderService : ICheckProviderService
    {
        // TODO: https://wirexapp.atlassian.net/browse/WRXB-10696
        private readonly ConcurrentDictionary<Guid, CheckRunningContextDto> _runningChecks = new();
        private readonly ConcurrentDictionary<Guid, CheckProcessingContextDto> _processingChecks = new();

        private readonly ICheckProviderConfigurationStorage _configurationStorage;
        private readonly ICheckProviderConfigurationFactory _configurationFactory;
        private readonly ICheckProviderRegistry _checkProviderRegistry;
        private readonly ICheckService _checkService;
        private readonly ICheckOutputDataSerializer _checkOutputDataSerializer;
        private readonly ILogger _logger;

        public CheckProviderService(
            ICheckProviderConfigurationStorage configurationStorage,
            ICheckProviderConfigurationFactory configurationFactory,
            ICheckProviderRegistry checkProviderRegistry,
            ICheckService checkService, 
            ICheckOutputDataSerializer checkOutputDataSerializer,
            ILogger logger)
        {
            _configurationStorage = configurationStorage ?? throw new ArgumentNullException(nameof(configurationStorage));
            _configurationFactory = configurationFactory ?? throw new ArgumentNullException(nameof(configurationFactory));
            _checkProviderRegistry = checkProviderRegistry ?? throw new ArgumentNullException(nameof(checkProviderRegistry));
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
            _checkOutputDataSerializer = checkOutputDataSerializer ?? throw new ArgumentNullException(nameof(checkOutputDataSerializer));
            _logger = logger?.ForContext<CheckProviderService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CheckInputParameterDto[]> GetParametersAsync(Guid variantId)
        {
            // TODO: https://wirexapp.atlassian.net/browse/WRXB-10697
            var configuration = await _configurationStorage.GetAsync(variantId);
            var providerConfiguration = _configurationFactory.Get(configuration.CheckType, configuration.ProviderType, configuration.Config);
            return providerConfiguration.CheckParameters.Select(Map).ToArray();

            static CheckInputParameterDto Map(CheckInputParameter parameter) => new() { XPath = parameter.XPath, IsRequired = parameter.IsRequired };
        }

        public Task RunAsync(CheckRunningContextDto[] contexts)
        {
            var onfidoChecks = contexts.Where(IsOnfidoCheck).ToArray();
            var otherChecks = contexts.Except(onfidoChecks).ToList();

            var runTasks = new List<Task>();
            if (onfidoChecks.Length > 1)
                runTasks.Add(RunGroupAsync(CheckProviderType.Onfido, onfidoChecks));
            else 
                otherChecks.AddRange(onfidoChecks);

            if (otherChecks.Count > 0)
                runTasks.AddRange(otherChecks.Select(RunAsync));

            return runTasks.WhenAll();

            static bool IsOnfidoCheck(CheckRunningContextDto context) =>
                context.ExternalProfile is { Provider: ExternalProviderType.Onfido };
        }

        public async Task ProcessAsync(CheckProcessingContextDto context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!_processingChecks.TryAdd(context.CheckId, context)) return;

            var executionContext = CheckProcessingContext.Create(context.ExternalData);

            var providerVariant = await _configurationStorage.GetAsync(context.VariantId);
            var provider = CreateCheckProvider(providerVariant);
            var processingResult = await provider.GetResultAsync(executionContext);
            await processingResult.Match(
                result => SaveProcessingResultAsync(context.CheckId, result),
                exception => SaveErrorResultAsync(context.CheckId, exception));

            _processingChecks.TryRemove(context.CheckId, out _);
        }

        private async Task RunGroupAsync(CheckProviderType providerType, CheckRunningContextDto[] contexts)
        {
            contexts.Foreach(context => _runningChecks.TryAdd(context.CheckId, context));

            var variantIds = contexts.Select(x => x.VariantId).ToArray();
            var provider = await CreateGroupCheckProvider(providerType, variantIds);
            var inputData = AggregateCheckInputData(contexts);

            var runningResult = await provider.RunAsync(inputData);

            foreach (var context in contexts)
            {
                await runningResult.Match(
                    result => HandleRunningResult(context, result),
                    exception => HandleErrorResult(context, exception));
            }

            contexts.Foreach(context => _runningChecks.TryRemove(context.CheckId, out _));
        }

        private static CheckInputData AggregateCheckInputData(CheckRunningContextDto[] contexts)
        {
            var userId = contexts.Select(x => x.UserId).First();
            var externalProfile = contexts.Select(x => x.ExternalProfile).First();
            var inputData = contexts.SelectMany(x => x.InputData)
                                    .GroupBy(x => x.Key, x => x.Value)
                                    .ToDictionary(x => x.Key, pairs => pairs.First());

            return new CheckInputData(userId, externalProfile.Id, inputData);
        }

        private async Task RunAsync(CheckRunningContextDto context)
        {
            if (!_runningChecks.TryAdd(context.CheckId, context)) return;

            var inputData = new CheckInputData(context.UserId, context.ExternalProfile?.Id, context.InputData);

            var providerVariant = await _configurationStorage.GetAsync(context.VariantId);
            var provider = CreateCheckProvider(providerVariant);
            var runningResult = await provider.RunAsync(inputData);
            await runningResult.Match(
                result => HandleRunningResult(context, result),
                exception => HandleErrorResult(context, exception));

            _runningChecks.TryRemove(context.CheckId, out _);
        }

        private async Task<CheckProvider> CreateGroupCheckProvider(CheckProviderType providerType, Guid[] variantIds)
        {
            var providerVariants = await variantIds.Select(_configurationStorage.GetAsync).WhenAll();
            var configurations = providerVariants.Select(GetConfiguration).ToArray();
            var groupedConfiguration = _configurationFactory.Get(providerType, configurations);

            var providerFactory = _checkProviderRegistry.GetFactory(providerType);
            return providerFactory.Create(groupedConfiguration);

            CheckProviderConfiguration GetConfiguration(CheckProviderConfigurationDto providerVariant) =>
                _configurationFactory.Get(providerVariant.CheckType, providerType, providerVariant.Config);
        }

        private CheckProvider CreateCheckProvider(CheckProviderConfigurationDto providerVariant)
        {
            var configuration = _configurationFactory.Get(providerVariant.CheckType, providerVariant.ProviderType, providerVariant.Config);
            var providerFactory = _checkProviderRegistry.GetFactory(providerVariant.ProviderType, providerVariant.CheckType);
            return providerFactory.Create(configuration);
        }

        private Task HandleRunningResult(CheckRunningContextDto context, CheckRunningResult result)
        {
            var executionContextDto = new CheckExecutionContextDto { InputData = context.InputData };

            return result switch
            {
                SyncCheckRunningResult syncResult => HandleSyncResult(syncResult),
                AsyncCheckRunningResult asyncResult => HandleAsyncResult(asyncResult),
                _ => throw new ArgumentOutOfRangeException(nameof(result), result.GetType(), "Unsupported check result.")
            };

            async Task HandleSyncResult(SyncCheckRunningResult syncResult)
            {
                await _checkService.StartExecutionAsync(context.CheckId, executionContextDto);
                await SaveProcessingResultAsync(context.CheckId, syncResult.ProcessingResult);
            }

            async Task HandleAsyncResult(AsyncCheckRunningResult asyncResult)
            {
                executionContextDto.ExternalData = new CheckExternalDataDto(asyncResult.ExternalData);
                await _checkService.StartExecutionAsync(context.CheckId, executionContextDto);
            }
        }

        private async Task HandleErrorResult(CheckRunningContextDto context, Exception exception)
        {
            var executionContextDto = new CheckExecutionContextDto { InputData = context.InputData };
            await _checkService.StartExecutionAsync(context.CheckId, executionContextDto);
            await SaveErrorResultAsync(context.CheckId, exception);
        }

        private Task SaveProcessingResultAsync(Guid checkId, CheckProcessingResult result)
        {
            var processingResultDto = new CheckProcessingResultDto
            {
                Result = result.IsPassed ? CheckResult.Passed : CheckResult.Failed,
                Decision = result.Decision,
                OutputData = _checkOutputDataSerializer.Serialize(result.OutputData)
            };
            return _checkService.SaveProcessingResultAsync(checkId, processingResultDto);
        }

        private Task SaveErrorResultAsync(Guid checkId, Exception exception)
        {
            _logger
                .ForContext(nameof(checkId), checkId)
                .Error(exception, "Error occurred during check execution.");

            var errorResult = MapErrorResult(exception);
            return _checkService.SaveErrorResultAsync(checkId, errorResult);
        }

        private static CheckErrorResultDto MapErrorResult(Exception exception)
        {
            var exceptions = exception is AggregateException aggregateException
                ? aggregateException.Flatten().InnerExceptions
                : Enumerable.Repeat(exception, 1);

            var checkErrors = exceptions.Select(MapCheckError).ToArray();
            return new CheckErrorResultDto { Errors = checkErrors };
        }

        private static CheckErrorDto MapCheckError(Exception exception)
        {
            return exception switch
            {
                CheckInputValidationException exc => new CheckErrorDto
                {
                    Code = Constants.ErrorCodes.MissingRequiredFields,
                    Message = "Check data validation failed.",
                    AdditionalData = new ErrorAdditionalDataDto { [nameof(exc.MissingData)] = exc.MissingData }
                },
                CheckExecutionException exc => new CheckErrorDto
                {
                    Code = exc.Error.Code,
                    Message = exc.Error.Message,
                    AdditionalData = MapAdditionalData(exc.Error.AdditionalData)
                },
                CheckProcessingException exc => new CheckErrorDto
                {
                    Code = exc.Error.Code,
                    Message = exc.Error.Message,
                    AdditionalData = MapAdditionalData(exc.Error.AdditionalData)
                },
                _ => new CheckErrorDto
                {
                    Code = Constants.ErrorCodes.UnknownInternalError,
                    Message = "Something went wrong in check provider."
                }
            };

            static ErrorAdditionalDataDto MapAdditionalData(IDictionary<string, object> additionalData) =>
                additionalData != null ? new ErrorAdditionalDataDto(additionalData) : null;
        }
    }
}
