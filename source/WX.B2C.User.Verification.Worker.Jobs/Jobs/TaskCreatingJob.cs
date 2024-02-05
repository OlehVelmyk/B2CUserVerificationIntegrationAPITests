using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using TaskVariantData = WX.B2C.User.Verification.Worker.Jobs.Models.TasksCreatingData.TaskVariantData;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class TaskCreatingJob : BatchJob<TasksCreatingData, TaskCreatingJobSettings>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ITaskRepository _taskRepository;

        public TaskCreatingJob(IApplicationRepository applicationService,
                               ITaskRepository taskService,
                               ITaskCreatingDataProvider jobDataProvider,
                               ILogger logger) 
            : base(jobDataProvider, logger)
        {
            _applicationRepository = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _taskRepository = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        public static string Name => "task-creating";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<TaskCreatingJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Create task and add to application");

        protected override async Task Execute(Batch<TasksCreatingData> batch, TaskCreatingJobSettings settings, CancellationToken cancellationToken)
        {
            foreach (var item in batch.Items)
            {
                var taskVariants = item.TaskVariants.Where(tv => !tv.AlreadyCreated);
                if (taskVariants.IsEmpty())
                    continue;

                var taskIds = await taskVariants.Foreach(tv => CreateTask(item.UserId, tv));
                await _applicationRepository.LinkTasksAsync(item.ApplicationId, taskIds);
            }
        }

        private Task<Guid> CreateTask(Guid userId, TaskVariantData taskVariant)
        {
            var newTaskDto = new TaskTemplate
            {
                UserId = userId,
                Type = taskVariant.Type,
                VariantId = taskVariant.Id
            };

            return _taskRepository.CreateAsync(newTaskDto);
        }
    }
}
