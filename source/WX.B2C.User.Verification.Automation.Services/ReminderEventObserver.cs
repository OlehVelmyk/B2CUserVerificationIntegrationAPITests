using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class ReminderEventObserver : IReminderEventObserver
    {
        private readonly IRegionActionsProvider _regionActionsProvider;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly IReminderManager _reminderManager;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IReminderRepository _reminderRepository;

        public ReminderEventObserver(IRegionActionsProvider regionActionsProvider,
                                     ICollectionStepStorage collectionStepStorage,
                                     IReminderManager reminderManager,
                                     IApplicationStorage applicationStorage,
                                     IReminderRepository reminderRepository)
        {
            _regionActionsProvider = regionActionsProvider ?? throw new ArgumentNullException(nameof(regionActionsProvider));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _reminderManager = reminderManager ?? throw new ArgumentNullException(nameof(reminderManager));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _reminderRepository = reminderRepository ?? throw new ArgumentNullException(nameof(reminderRepository));
        }

        public async Task OnCollectionStepStateChanged(Guid userId, Guid stepId)
        {
            if (!await IsAutomatedAsync(userId))
                return;
            
            var stepToRemind = await FindStepToRemind(userId);
            if (stepToRemind != stepId)
                return;

            await _reminderManager.ScheduleAsync(userId, stepToRemind.Value);
        }

        public async Task OnCollectionStepSubmitted(Guid userId, Guid stepId)
        {
            await _reminderManager.TryCancelReminder(userId, stepId);
            await OnUserActionsChanged(userId);
        }

        public async Task OnJobFinishedAsync(Guid userId, Guid stepId)
        {
            var userReminderDto = new UserReminderDto
            {
                UserId = userId,
                TargetId = stepId,
                SentAt = DateTime.UtcNow
            };
            await _reminderRepository.SaveAsync(userReminderDto);
            await _reminderManager.ScheduleAsync(userId, stepId);
        }

        public async Task OnApplicationRejected(Guid userId)
        {
            var stepToRemind = await FindStepToRemind(userId);
            if (!stepToRemind.HasValue) return;

            await _reminderManager.TryCancelReminder(userId, stepToRemind.Value);
        }

        public Task OnApplicationReverted(Guid userId) => OnUserActionsChanged(userId);

        public async Task OnUserActionsChanged(Guid userId)
        {
            if (!await IsAutomatedAsync(userId))
                return;
            
            var stepToRemind = await FindStepToRemind(userId);
            if (stepToRemind is not null)
                await _reminderManager.ScheduleAsync(userId, stepToRemind.Value);
        }

        private async Task<Guid?> FindStepToRemind(Guid userId)
        {
            var steps = await _collectionStepStorage.FindRequestedAsync(userId);
            if (steps.IsEmpty())
                return null;

            var regionActions = await _regionActionsProvider.GetAsync(userId);
            var step = regionActions.Actions
                                    .Join(steps,
                                          action => action.XPath,
                                          step => step.XPath,
                                          (action, step) => (action, step))
                                    .OrderByDescending(tuple => tuple.step.IsRequired)
                                    .ThenBy(tuple => tuple.action.Priority)
                                    .Select(tuple => (Guid?)tuple.step.Id)
                                    .FirstOrDefault();

            return step;
        }
        
        private Task<bool> IsAutomatedAsync(Guid userId) =>
            _applicationStorage.IsAutomatedAsync(userId, ProductType.WirexBasic);
    }
}