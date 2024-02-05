using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Automation.Services.Mappers;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Extensions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface IReminderManager
    {
        Task ScheduleAsync(Guid userId, Guid collectionStepId);

        Task<bool> TryCancelReminder(Guid userId, Guid stepId);
    }

    internal class ReminderManager : IReminderManager
    {
        private readonly IOptionProvider<UserReminderOption> _optionsProvider;
        private readonly IReminderStorage _reminderStorage;
        private readonly ISystemClock _systemClock;
        private readonly IJobService _jobService;
        private readonly IUserReminderMapper _reminderMapper;
        private readonly ILogger _logger;

        public ReminderManager(IOptionProvider<UserReminderOption> optionsProvider,
                               IReminderStorage reminderStorage,
                               ISystemClock systemClock,
                               IJobService jobService,
                               IUserReminderMapper reminderMapper,
                               ILogger logger)
        {
            _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
            _reminderStorage = reminderStorage ?? throw new ArgumentNullException(nameof(reminderStorage));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _reminderMapper = reminderMapper ?? throw new ArgumentNullException(nameof(reminderMapper));
            _logger = logger?.ForContext<ReminderManager>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ScheduleAsync(Guid userId, Guid collectionStepId)
        {
            var fireAt = await EvaluateFiringDateAsync(userId, collectionStepId);
            if (!fireAt.HasValue)
                return;

            var triggerParameters = _reminderMapper.Map(userId, collectionStepId);
            if (await _jobService.ExistsTriggerAsync(triggerParameters))
            {
                _logger.Information("Trigger already exists for User: {UserId} and CollectionStep: {CollectionStepId}", userId, collectionStepId);
                return;
            }
                
            var newTrigger = _reminderMapper.Map(userId, collectionStepId, fireAt.Value);
            
            // To avoid two reminders at the same time, unschedule all previous reminders firstly
            await _jobService.UnscheduleJobAsync(triggerParameters);
            
            await _jobService.ScheduleAsync(newTrigger, CancellationToken.None);
        }

        public Task<bool> TryCancelReminder(Guid userId, Guid stepId)
        {
            var triggerParameters = _reminderMapper.Map(userId, stepId);
            return _jobService.UnscheduleAsync(triggerParameters);
        }

        private async Task<DateTime?> EvaluateFiringDateAsync(Guid userId, Guid collectionStepId)
        {
            var options = await _optionsProvider.GetAsync();

            var remindersSent = await _reminderStorage.CountAsync(userId, collectionStepId);
            if (remindersSent >= options.Spans.Length)
            {
                _logger.Warning("Application rejection is recommended. Inactive user {UserId}", userId);
                return null;
            }
            var span = options.Spans[remindersSent];
            return _systemClock.GetDate().AddInterval(span.Unit, span.Value);
        }
    }
}