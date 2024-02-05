using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class TaskEventObserver : ITaskEventObserver
    {
        private readonly ITaskStorage _taskStorage;
        private readonly ITaskManager _taskManager;

        public TaskEventObserver(ITaskStorage taskStorage, ITaskManager taskManager)
        {
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
        }

        public async Task OnCheckPassed(Guid checkId)
        {
            var reason = InitiationReasons.CompleteTaskWhenCheckCompleted(checkId);
            var tasks = await _taskStorage.FindByCheckIdAsync(checkId);
            await _taskManager.TryPassAsync(tasks, reason);
        }

        public async Task OnCheckFailed(Guid checkId)
        {
            var reason = InitiationReasons.IncompleteTaskWhenCheckFailed(checkId);
            var tasks = await _taskStorage.FindByCheckIdAsync(checkId);
            await _taskManager.TryIncompleteAsync(tasks, reason);
        }

        public async Task OnCollectionStepCompleted(Guid collectionStepId)
        {
            var reason = InitiationReasons.CompleteTaskWhenCollectionStepCompleted(collectionStepId);
            var tasks = await _taskStorage.FindByStepIdAsync(collectionStepId);
            await _taskManager.TryPassAsync(tasks, reason);
        }

        public async Task OnCollectionStepRequired(Guid collectionStepId)
        {
            var reason = InitiationReasons.IncompleteTaskWhenCollectionStepRequired(collectionStepId);
            var tasks = await _taskStorage.FindByStepIdAsync(collectionStepId);
            await _taskManager.TryIncompleteAsync(tasks, reason);
        }
    }
}
