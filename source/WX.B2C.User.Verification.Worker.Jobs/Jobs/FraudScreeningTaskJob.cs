using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using SqlKata;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class FraudScreeningTaskJob : BatchJob<ApplicationData, FraudScreeningTaskJobSettings>
    {
        // use constant because FraudScreening task exists only in US policy
        private readonly Guid _defaultTaskVariantId = Guid.Parse("886609CB-ED63-4AEF-AEE9-B51B75C2A829");

        private readonly IApplicationService _applicationService;
        private readonly ITaskService _taskService;
        private readonly ITaskStorage _taskStorage;
        private readonly IQueryFactory _queryFactory;

        private readonly string[] _ignoredRiskCodes =
        {
            "79", // MissingOrIncompleteSsnRiskCode
            "80" // MissingPhoneRiskCode
        };

        public FraudScreeningTaskJob(IApplicationService applicationService,
                                     ITaskService taskService,
                                     ITaskStorage taskStorage,
                                     IUsaApplicationDataProvider jobDataProvider,
                                     IUserVerificationQueryFactory queryFactory,
                                     ILogger logger)
            : base(jobDataProvider, logger)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));
        }

        public static string Name => "fraud-screening-task";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<FraudScreeningTaskJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Backfill FraudScreening task from WX.UserVerification.");

        protected override async Task Execute(Batch<ApplicationData> batch,
                                              FraudScreeningTaskJobSettings settings,
                                              CancellationToken cancellationToken)
        {
            var userIds = batch.Items.Select(item => item.UserId).ToArray();
            var allFraudScreeningData = await FindFraudScreeningData(userIds);

            foreach (var item in batch.Items)
            {
                var data = allFraudScreeningData.FirstOrDefault(data => data.UserId == item.UserId);
                var taskResult = EstimateTaskResult(item.UserId, data);
                var initiation = InitiationDto.CreateJob(Name);

                var existingTask = await _taskStorage.FindAsync(item.Id, TaskType.FraudScreening);
                if (existingTask is not null)
                {
                    await TryCompleteTask(existingTask, taskResult, initiation);
                    continue;
                }

                var taskId = await CreateTask(item.UserId, settings.TaskVariantId, taskResult, initiation);
                await _applicationService.AddRequiredTasksAsync(item.Id, new[] { taskId }, initiation);
            }
        }

        private async Task<Guid> CreateTask(Guid userId,
                                            Guid? taskVariantId,
                                            TaskResult? taskResult,
                                            InitiationDto initiation)
        {
            // Can get first because we don`t support multi-applications functionality
            // Now it looks like one user - one application - one task of specific type
            var userTasks = await _taskStorage.GetAllAsync(userId, TaskType.FraudScreening);
            if (userTasks != null && userTasks.Length > 1)
                Logger.Warning($"Job {Name}. UserId {userId} has many tasks for TaskType: TaskType.FraudScreening");

            var existingTask = userTasks.FirstOrDefault();
            if (existingTask is not null)
            {
                await TryCompleteTask(existingTask, taskResult, initiation);
                return existingTask.Id;
            }

            var newTask = new NewTaskDto
            {
                UserId = userId,
                Type = TaskType.FraudScreening,
                VariantId = taskVariantId ?? _defaultTaskVariantId
            };
            var taskId = await _taskService.CreateAsync(newTask, initiation);

            if (taskResult.HasValue)
                await _taskService.CompleteAsync(taskId, taskResult.Value, initiation);

            return taskId;
        }

        private async Task TryCompleteTask(TaskDto task, TaskResult? expectedResult, InitiationDto initiation)
        {
            if (task.State == TaskState.Incomplete && expectedResult.HasValue)
                await _taskService.CompleteAsync(task.Id, expectedResult.Value, initiation);
            else if (task.State == TaskState.Completed && !expectedResult.HasValue)
                await _taskService.IncompleteAsync(task.Id, initiation);
            else if (task.State == TaskState.Completed && expectedResult.HasValue && task.Result != expectedResult)
            {
                // Cannot move completed task between different task results
                await _taskService.IncompleteAsync(task.Id, initiation);
                await _taskService.CompleteAsync(task.Id, expectedResult.Value, initiation);
            }
        }

        private TaskResult? EstimateTaskResult(Guid userId, FraudScreeningData data)
        {
            if (data is null)
                return null;

            var cvi = data.ComprehensiveIndex.GetValueOrDefault(0);
            if (cvi >= 30 && cvi < 40 || HasPartialRiskCodes(data.RiskCodes))
                Logger.Information("Found suspicious user with id {UserId}.", userId);

            return cvi < 30 ? TaskResult.Failed : TaskResult.Passed;

            bool HasPartialRiskCodes(string riskCodes)
                => riskCodes?.Split(',').Any(riskCode => !riskCode.In(_ignoredRiskCodes)) ?? false;
        }

        private async Task<FraudScreeningData[]> FindFraudScreeningData(Guid[] userIds)
        {
            var query = new Query("InstantIdSearches").
                        Select("UserId", "ComprehensiveVerificationIndex AS ComprehensiveIndex", "RiskCodes").
                        WhereUserIdIn(userIds);

            using var db = _queryFactory.Create();
            var fraudScreeningData = await db.GetAsync<FraudScreeningData>(query);
            return fraudScreeningData.ToArray();
        }
    }
}