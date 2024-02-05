using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal sealed class FinancialConditionJob : BatchJob<SurveyCheckData, FinancialConditionJobSetting>
    {
        private readonly ITaskService _taskService;
        private readonly ITaskStorage _taskStorage;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IApplicationService _applicationService;

        public FinancialConditionJob(
            IFinancialConditionProvider jobDataProvider,
            IApplicationStorage applicationStorage,
            IApplicationService applicationService,
            ITaskService taskService,
            ITaskStorage taskStorage,
            ILogger logger)
            : base(jobDataProvider, logger)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        public static string Name => "financial-condition-job";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<FinancialConditionJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Create FinancialCondition task in corresponding state without collection steps and without acceptance and performed checks.");

        protected override async Task Execute(Batch<SurveyCheckData> batch, FinancialConditionJobSetting settings, CancellationToken cancellationToken)
        {
            foreach (var survey in batch.Items)
            {
                var application = await _applicationStorage.FindAsync(survey.UserId, ProductType.WirexBasic);
                if (application == null)
                {
                    Logger.Warning($"Job {Name}. ApplicationId not found for userId {survey.UserId}");
                    continue;
                }

                var task = application.Tasks.FirstOrDefault(t => t.Type == TaskType.FinancialCondition)
                           ?? await CreateTaskAsync(survey.UserId, application.Id, settings.TaskVariantId);

                await SetCorrectTaskState(survey.Status, task);
            }
        }

        private async Task<ApplicationTaskDto> CreateTaskAsync(Guid userId, Guid applicationId, Guid taskVariantId)
        {
            var userTasks = await _taskStorage.GetAllAsync(userId, TaskType.FinancialCondition);
            if (userTasks is { Length: > 1 })
                Logger.Warning($"Job {Name}. UserId {userId} has many tasks for TaskType: TaskType.FinancialCondition");

            var applicationTask = Map(userTasks.FirstOrDefault());
            if (applicationTask == null)
            {
                var newTaskDto = new NewTaskDto
                {
                    AcceptanceCheckIds = new Guid[] { },
                    CollectionStepIds = new Guid[] { },
                    Type = TaskType.FinancialCondition,
                    UserId = userId,
                    VariantId = taskVariantId
                };
                var initiation = InitiationDto.CreateJob(Name);

                applicationTask = new ApplicationTaskDto
                {
                    Id = await _taskService.CreateAsync(newTaskDto, initiation),
                };
            }

            var tasks = new[] { applicationTask.Id };
            await _applicationService.AddRequiredTasksAsync(applicationId, tasks, InitiationDto.CreateJob(Name));
            return applicationTask;
        }

        private Task SetCorrectTaskState(SurveyCheckStatus surveyStatus, ApplicationTaskDto task)
        {
            var initiation = InitiationDto.CreateJob(Name);

            if (surveyStatus == SurveyCheckStatus.Completed)
            {
                if (task.State != TaskState.Completed)
                    return _taskService.CompleteAsync(task.Id, TaskResult.Passed, initiation);
            }
            else
            {
                if (task.State == TaskState.Completed)
                    return _taskService.IncompleteAsync(task.Id, initiation);
            }

            return Task.CompletedTask;
        }

        private static ApplicationTaskDto Map(TaskDto taskDto)
        {
            if (taskDto == null)
                return null;

            return new ApplicationTaskDto
            {
                Id = taskDto.Id,
                Type = taskDto.Type,
                State = taskDto.State,
                Result = taskDto.Result,
                VariantId = taskDto.VariantId
            };
        }
    }
}
