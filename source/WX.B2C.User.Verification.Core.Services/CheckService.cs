using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Core.Services.Utilities;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class CheckService : ICheckService
    {
        private readonly ICheckRepository _checkRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IInitiationMapper _initiationMapper;
        private readonly ISystemClock _systemClock;

        public CheckService(
            ICheckRepository checkRepository,
            IEventPublisher eventPublisher,
            IInitiationMapper initiationMapper,
            ISystemClock systemClock)
        {
            _checkRepository = checkRepository ?? throw new ArgumentNullException(nameof(checkRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public Task RequestAsync(Guid userId, NewCheckDto newCheckDto, InitiationDto initiationDto)
        {
            var checkId = newCheckDto.Id ?? Guid.NewGuid();
            var checkVariant = CheckVariant.Create(newCheckDto.VariantId, newCheckDto.Provider);
            var initiation = _initiationMapper.Map(initiationDto);

            var check = Check.Create(
                checkId,
                userId,
                newCheckDto.CheckType,
                checkVariant,
                newCheckDto.RelatedTasks,
                initiation);

            return SaveAndPublishAsync(check);
        }

        public Task StartExecutionAsync(Guid checkId, CheckExecutionContextDto executionContextDto) =>
            UpdateAsync(checkId, check =>
            {
                var startedAt = _systemClock.GetDate();
                var executionContext = CheckExecutionContext.Create(
                    executionContextDto.InputData, 
                    executionContextDto.ExternalData);
                check.Start(executionContext, startedAt);
            });

        public Task FinishExecutionAsync(Guid checkId, CheckExecutionResultDto executionResultDto) =>
            UpdateAsync(checkId, check =>
            {
                var performedAt = _systemClock.GetDate();
                check.Performed(performedAt);
            });

        public Task SaveProcessingResultAsync(Guid checkId, CheckProcessingResultDto processingResultDto) =>
            UpdateAsync(checkId, check =>
            {
                var completedAt = _systemClock.GetDate();
                var processingResult = CheckProcessingResult.Create(
                    processingResultDto.Result, 
                    processingResultDto.Decision, 
                    processingResultDto.OutputData);
                check.Complete(processingResult, completedAt);
            });

        public Task SaveErrorResultAsync(Guid checkId, CheckErrorResultDto errorResult) =>
            UpdateAsync(checkId, check =>
            {
                var completedAt = _systemClock.GetDate();
                var checkErrors = errorResult.Errors.Select(Map).ToArray();
                check.Error(checkErrors, completedAt);

                static CheckError Map(CheckErrorDto error) => CheckError.Create(error.Code, error.Message, error.AdditionalData);
            });

        public Task CancelAsync(Guid checkId) =>
            UpdateAsync(checkId, check =>
            {
                var cancelledAt = _systemClock.GetDate();
                check.Cancel(cancelledAt);
            });

        private Task<Check> UpdateAsync(Guid checkId, Action<Check> update) =>
            UpdateAsync(checkId, update?.ToAsync());

        private Task<Check> UpdateAsync(Guid checkId, Func<Check, Task> update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            return AppCore.ApplyChangesAsync(
                () => _checkRepository.GetAsync(checkId),
                update,
                SaveAndPublishAsync);
        }

        private async Task SaveAndPublishAsync(Check check)
        {
            await _checkRepository.SaveAsync(check);
            await _eventPublisher.PublishAsync(check.CommitEvents());
        }
    }
}