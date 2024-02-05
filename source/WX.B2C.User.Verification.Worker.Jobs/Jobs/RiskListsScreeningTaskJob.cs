using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class RiskListsScreeningTaskJob : BatchJob<ApplicationData, RiskListsScreeningTaskJobSettings>
    {
        private readonly IApplicationService _applicationService;
        private readonly ITaskService _taskService;
        private readonly ITaskStorage _taskStorage;
        private readonly IProfileStorage _profileStorage;

        private readonly Dictionary<Guid, Guid> _mapPolicyAndTaskVariant = new()
        {
            [Guid.Parse("DC658B4F-A0EB-4C20-B296-E0D57E8DA6DB")] = Guid.Parse("9A1C60FB-6F96-431B-ABDB-11B5FC9C5CA5"), //GB
            [Guid.Parse("0EAAE368-8ACB-410B-8EC0-3AE404F49D5E")] = Guid.Parse("FAE04E22-AF37-46A6-875A-C93A2EA8C9A3"), //EEA
            [Guid.Parse("37C6AD01-067C-4B80-976D-30A568E7B0CD")] = Guid.Parse("2772DDB3-1D85-4625-8479-67677D9622BE"), //APAC
            [Guid.Parse("4B6271BD-FDE5-40F7-8701-29AA66865568")] = Guid.Parse("C2BADFF2-B73F-4FB0-B447-A3EB48964B36"), //US
            [Guid.Parse("D5B5997E-FFC1-495D-9E98-60CCBDD6F43B")] = Guid.Parse("1DD5AE17-D87C-4B9F-B61F-A084B29ABB4A"), //ROW
            [Guid.Parse("5DECE2A9-CDD3-4D0D-B1BC-8A164B745051")] = Guid.Parse("AA49652A-74C8-4667-A20F-A92FE59CBF2B"), //GLOBAL
            [Guid.Parse("67A2B2C8-BEAB-4C3E-A772-19CE9380CB0E")] = Guid.Parse("CF3D8AFC-CBB4-4531-BEA5-DDF2EB990CFE")  //RU
        };

        public RiskListsScreeningTaskJob(IApplicationService applicationService,
                                         ITaskService taskService,
                                         ITaskStorage taskStorage,
                                         IProfileStorage profileStorage,
                                         IApplicationDataProvider jobDataProvider,
                                         ILogger logger)
            : base(jobDataProvider, logger)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
        }

        public static string Name => "risk-lists-screening-task";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<RiskListsScreeningTaskJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Backfill RiskListsScreening task from WX.UserVerification.");

        protected override async Task Execute(Batch<ApplicationData> batch, RiskListsScreeningTaskJobSettings settings, CancellationToken cancellationToken)
        {
            var userIds = batch.Items.Select(item => item.UserId).ToArray();
            var allVerificationDetails = await _profileStorage.GetVerificationDetailsAsync(userIds);

            foreach (var item in batch.Items)
            {
                var details = allVerificationDetails.FirstOrDefault(data => data.UserId == item.UserId);
                var taskResult = EstimateTaskResult(item.UserId, details);
                var initiation = InitiationDto.CreateJob(Name);

                var existingTask = await _taskStorage.FindAsync(item.Id, TaskType.RiskListsScreening);
                if (existingTask is not null)
                {
                    await TryCompleteTask(existingTask, taskResult, initiation);
                    continue;
                }

                var taskVariantId = settings.TaskVariantId ?? GetTaskVariantId(item.UserId, item.PolicyId);
                var taskId = await CreateTask(item.UserId, taskVariantId, taskResult, initiation);
                await _applicationService.AddRequiredTasksAsync(item.Id, new[] { taskId }, initiation);
            }
        }

        private async Task<Guid> CreateTask(Guid userId, Guid taskVariantId, TaskResult? taskResult, InitiationDto initiation)
        {
            // Can get first because we don`t support multi-applications functionality
            // Now it looks like one user - one application - one task of specific type
            var userTasks = await _taskStorage.GetAllAsync(userId, TaskType.RiskListsScreening);
            if (userTasks != null && userTasks.Length > 1)
            {
                base.Logger.Warning($"Job {Name}. UserId {userId} has many tasks for TaskType: TaskType.RiskListsScreening");
            }

            var existingTask = userTasks.FirstOrDefault();
            if (existingTask is not null)
            {
                await TryCompleteTask(existingTask, taskResult, initiation);
                return existingTask.Id;
            }

            var newTask = new NewTaskDto
            {
                UserId = userId,
                Type = TaskType.RiskListsScreening,
                VariantId = taskVariantId
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

        private TaskResult? EstimateTaskResult(Guid userId, VerificationDetailsDto verificationDetails)
        {
            if (verificationDetails is null)
                return null;

            var riskIndicators = new (string Key, bool? Value)[]
            {
                (nameof(verificationDetails.IsPep), verificationDetails.IsPep),
                (nameof(verificationDetails.IsSanctioned), verificationDetails.IsSanctioned),
                (nameof(verificationDetails.IsAdverseMedia), verificationDetails.IsAdverseMedia)
            };

            if (riskIndicators.All(indicator => !indicator.Value.HasValue))
                return null;

            var isTaskFailed = false;
            foreach (var indicator in riskIndicators)
            {
                if (!indicator.Value.HasValue)
                    Logger.Information("Parameter {RiskIndicator} is null in user {UserId}.", indicator.Key, userId);

                isTaskFailed |= indicator.Value.HasValue && indicator.Value.Value;
            }
            return isTaskFailed ? TaskResult.Failed : TaskResult.Passed;
        }

        private Guid GetTaskVariantId(Guid userId, Guid policyId)
        {
            if (!_mapPolicyAndTaskVariant.TryGetValue(policyId, out var variantId))
                throw new ArgumentException($"Found unrecognized policy id {policyId} for user {userId}.");

            return variantId;
        }
    }
}
