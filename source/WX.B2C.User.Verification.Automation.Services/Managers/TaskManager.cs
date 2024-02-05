using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface ITaskManager
    {
        Task TryPassAsync(IReadOnlyCollection<TaskDto> tasks, string reason);

        Task TryIncompleteAsync(IReadOnlyCollection<TaskDto> tasks, string reason);
    }

    internal class TaskManager : ITaskManager
    {
        private readonly ITaskCompletionService _taskCompletionService;
        private readonly IBatchCommandPublisher _commandsPublisher;

        public TaskManager(
            ITaskCompletionService taskCompletionService,
            IBatchCommandPublisher commandsPublisher)
        {
            _taskCompletionService = taskCompletionService ?? throw new ArgumentNullException(nameof(taskCompletionService));
            _commandsPublisher = commandsPublisher ?? throw new ArgumentNullException(nameof(commandsPublisher));
        }

        public async Task TryPassAsync(IReadOnlyCollection<TaskDto> tasks, string reason)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException(nameof(reason));

            var tasksToPass = await FindReadyToPassTasksAsync(tasks).ToArrayAsync();

            var commands = tasksToPass
                           .Select(task => new PassTaskCommand(task.Id, task.UserId, reason))
                           .ToArray();

            await _commandsPublisher.PublishAsync(commands);
        }

        public Task TryIncompleteAsync(IReadOnlyCollection<TaskDto> tasks, string reason)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException(nameof(reason));

            var commands = tasks
                           .Where(task => task.CanBeIncomplete())
                           .Select(task => new IncompleteTaskCommand(task.Id, task.UserId, reason))
                           .ToArray();

            return _commandsPublisher.PublishAsync(commands);
        }

        private async IAsyncEnumerable<TaskDto> FindReadyToPassTasksAsync(IEnumerable<TaskDto> tasks)
        {
            foreach (var task in tasks)
            {
                var canBePassed = await _taskCompletionService.CanBePassedAsync(task);
                if (canBePassed) yield return task;
            }
        }
    }
}