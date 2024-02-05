using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface ICheckManager
    {
        Task TryInstructAsync(Guid userId, IEnumerable<AcceptanceCheck> checks, string reason);

        Task TryRunAsync(Guid userId, IEnumerable<PendingCheck> checks);

        Task TryProcessAsync(CheckDto checkDto);
    }

    internal class CheckManager : ICheckManager
    {
        private readonly ICheckFilteringService _checkFilteringService;
        private readonly ICheckProviderService _checkProviderService;
        private readonly IBatchCommandPublisher _commandPublisher;
        private readonly ILogger _logger;

        public CheckManager(
            ICheckFilteringService checkFilteringService,
            ICheckProviderService checkProviderService,
            IBatchCommandPublisher commandPublisher,
            ILogger logger)
        {

            _checkProviderService = checkProviderService ?? throw new ArgumentNullException(nameof(checkProviderService));
            _commandPublisher = commandPublisher ?? throw new ArgumentNullException(nameof(commandPublisher));
            _checkFilteringService = checkFilteringService ?? throw new ArgumentNullException(nameof(checkFilteringService));
            _logger = logger?.ForContext<CheckManager>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task TryInstructAsync(Guid userId, IEnumerable<AcceptanceCheck> checks, string reason)
        {
            var (checksToInstruct, skippedChecks) = await _checkFilteringService.FilterAsync(userId, checks);

            if (skippedChecks.Length > 0) LogSkippedChecks(skippedChecks);

            var commands = checksToInstruct.Select(Map).ToArray();

            await _commandPublisher.PublishAsync(commands);

            RequestCheckCommand Map(CheckToInstructContext context) =>
                new(userId,
                    Guid.NewGuid(),
                    context.Check.VariantId,
                    context.Variant.Type.ToString(),
                    context.Variant.Provider.ToString(),
                    context.Check.RelatedTasks.Select(x => x.Id).ToArray(),
                    reason);
        }

        public async Task TryRunAsync(Guid userId, IEnumerable<PendingCheck> checks)
        {
            var (checksToRun, skippedChecks) = await _checkFilteringService.FilterAsync(userId, checks);

            if (skippedChecks.Length > 0) LogSkippedChecks(skippedChecks);

            if (checksToRun.Length == 0) return;

            var requests = checksToRun.Select(MapContext).ToArray();

            await _checkProviderService.RunAsync(requests);

            CheckRunningContextDto MapContext(CheckToRunContext context) => new()
            {
                UserId = userId,
                CheckId = context.Check.Id,
                VariantId = context.Check.VariantId,
                InputData = new CheckInputDataDto(context.InputData),
                ExternalProfile = context.ExternalProfile
            };
        }

        public Task TryProcessAsync(CheckDto checkDto)
        {
            var context = new CheckProcessingContextDto
            {
                CheckId = checkDto.Id,
                UserId = checkDto.UserId,
                VariantId = checkDto.Variant.Id,
                ExternalData = checkDto.ExternalData
            };
            return _checkProviderService.ProcessAsync(context);
        }

        private void LogSkippedChecks<T>(SkippedCheck<T>[] skippedChecks)
        {
            var checkType = typeof(T).Name;
            var command = checkType switch
            {
                nameof(AcceptanceCheck) => "instruct",
                nameof(PendingCheck) => "run",
                _ => throw new ArgumentOutOfRangeException(nameof(checkType), checkType, "Unknown check type.")
            };

            _logger.Information("Prevent checks {@Checks} to {Command}.", skippedChecks, command);
        }
    }
}