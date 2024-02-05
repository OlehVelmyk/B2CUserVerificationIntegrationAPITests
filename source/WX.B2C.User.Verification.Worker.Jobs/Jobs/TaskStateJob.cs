using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class TaskStateJob : BatchJob<UserTaskData, TaskStateJobSettings>
    {
        private readonly ITaskService _taskService;
        private readonly ITaskRepository _taskRepository;

        public TaskStateJob(ITaskService taskService,
                            ITaskRepository taskRepository,
                            ITaskDataProvider jobDataProvider,
                            ILogger logger)
            : base(jobDataProvider, logger)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        }

        public static string Name => "task-state";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<TaskStateJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Moves task to appropriate state");

        protected override async Task Execute(Batch<UserTaskData> batch, TaskStateJobSettings settings, CancellationToken cancellationToken)
        {
            var newTaskState = settings.State;
            TaskResult? newTaskResult = newTaskState == TaskState.Incomplete ? null : settings.Result ?? TaskResult.Passed;

            //TODO Ideally have a factory, but no make sense as for direct are needed only for initial release
            Func<TaskStateInfo, Task> SaveState = settings.UseActors
                ? task => SaveByActorsAsync(task, newTaskState, newTaskResult)
                : task => SaveDirectlyAsync(task, newTaskState, newTaskResult);

            foreach (var item in batch.Items)
            {
                var userId = item.UserId;
                var taskType = item.TaskType;
                var logger = Logger.ForContext(nameof(userId), userId);

                if (item.Tasks.IsNullOrEmpty())
                {
                    logger.Warning("No task with type {Type}", taskType);

                    //For now this is not error. Later when we will use it not for data migration will be an error.
                    ////IncrementErrorCount();
                    continue;
                }
                if (item.Tasks.Length != 1)
                {
                    logger.Warning("Multiple tasks {TasksNumber} are found with type {Type}", item.Tasks.Length, taskType);
                    IncrementErrorCount();
                    continue;
                }

                var task = item.Tasks.First();

                if (task.State == newTaskState && task.Result == newTaskResult)
                {
                    logger.Information(
                    "Skipped updating task state for {TaskType}. Current task state: {CurrentTaskState}, Current task result {TaskResult}",
                    taskType,
                    task.State,
                    task.Result);

                    continue;
                }

                await SaveState(task);
            }
        }

        private Task SaveDirectlyAsync(TaskStateInfo task,
                                       TaskState newTaskState,
                                       TaskResult? newTaskResult)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return newTaskState switch
            {
                TaskState.Incomplete => _taskRepository.IncompleteAsync(task.Id),
                TaskState.Completed  => _taskRepository.CompleteAsync(task.Id, newTaskResult.Value),
                _                    => throw new ArgumentOutOfRangeException(nameof(newTaskState), newTaskState, null)
            };
        }

        private async Task SaveByActorsAsync(TaskStateInfo task,
                                             TaskState newTaskState,
                                             TaskResult? newTaskResult)
        {
            var initiation = InitiationDto.CreateJob(Name);

            switch (newTaskState)
            {
                case TaskState.Incomplete when task.State == TaskState.Completed:
                    await _taskService.IncompleteAsync(task.Id, initiation);
                    break;
                case TaskState.Completed when task.State == TaskState.Incomplete:
                    await _taskService.CompleteAsync(task.Id, newTaskResult.Value, initiation);
                    break;
                case TaskState.Completed when task.State == TaskState.Completed && task.Result != newTaskResult:
                    await _taskService.IncompleteAsync(task.Id, initiation);
                    await _taskService.CompleteAsync(task.Id, newTaskResult.Value, initiation);
                    break;
            }
        }
    }
}