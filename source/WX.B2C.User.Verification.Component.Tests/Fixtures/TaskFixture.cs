using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class TaskFixture
    {
        private readonly Guid[] _manualTasks = Array.Empty<Guid>();

        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly CollectionStepsFixture _stepsFixture;
        private readonly ChecksFixture _checksFixture;
        private readonly EventsFixture _eventsFixture;

        public TaskFixture(AdminApiClientFactory adminApiClientFactory,
                           AdministratorFactory adminFactory,
                           CollectionStepsFixture stepsFixture,
                           ChecksFixture checksFixture,
                           EventsFixture eventsFixture)
        {
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _stepsFixture = stepsFixture ?? throw new ArgumentNullException(nameof(stepsFixture));
            _checksFixture = checksFixture ?? throw new ArgumentNullException(nameof(checksFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public async Task CompleteAsync(Guid userId, Guid taskId, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var task = await adminClient.Tasks.GetAsync(userId, taskId);
            if (task.State is TaskState.Completed)
                return;

            var previousEventCorrelationIds = _eventsFixture.GetAllEvents<TaskCompletedEvent>(IsCompleted)
                                                            .Select(e => e.CorrelationId)
                                                            .ToArray();

            if (task.Priority > 1)
                await CompletePriorityGroupAsync(userId, task.Priority - 1, seed);

            await _stepsFixture.CompleteAllAsync(userId, task.CollectionSteps.Select(step => step.Id), false, seed);
            await task.Checks.ForeachConsistently(check => _checksFixture.CompleteAsync(userId, check.Id, seed));

            if (IsManualTask(task.Variant.Id))
            {
                var request = new CompleteTaskRequest(TaskResult.Passed, $"{nameof(TaskFixture)}.{nameof(CompleteAsync)}");
                await adminClient.Tasks.CompleteAsync(request, userId, taskId);
            }

            Console.WriteLine($"Waiting for TaskCompletedEvent. TaskId: {taskId}, UserId: {userId}. TaskType: {task.Type}");
            _eventsFixture.ShouldExistSingle<TaskCompletedEvent>(e => IsCompleted(e) && !e.CorrelationId.In(previousEventCorrelationIds));

            bool IsCompleted(TaskCompletedEvent e) => e.EventArgs.Id == taskId;
        }

        public async Task IncompleteAsync(Guid userId, Guid taskId)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var reason = new ReasonDto($"{nameof(TaskFixture)}.{nameof(IncompleteAsync)}");
            await adminClient.Tasks.IncompleteAsync(reason, userId, taskId);

            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(adminClient.CorrelationId);
        }

        public async Task RemoveCollectionStepAsync(Guid userId, CollectionStepDto collectionStep)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            foreach (var relatedTask in collectionStep.RelatedTasks)
            {
                await adminClient.Tasks.RemoveCollectionStepAsync(userId, relatedTask, collectionStep.Id);
            }
        }

        private async Task CompletePriorityGroupAsync(Guid userId, int priority, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var tasks = (await adminClient.Tasks.GetAllAsync(userId))
                .Where(task => task.Priority == priority)
                .Where(task => task.State is not TaskState.Completed);

            await tasks.Foreach(task => CompleteAsync(userId, task.Id, seed));
        }

        private bool IsManualTask(Guid taskVariantId) => _manualTasks.Contains(taskVariantId);
    }
}
