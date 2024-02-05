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
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class ProofOfFundsTaskJob : BatchJob<ProofOfFundsChecksData, UserBatchJobSettings>
    {
        private readonly Guid _taskVariantId = new("C1B826B7-6E0A-4AB3-813D-4393E0C0E095");

        private readonly IApplicationService _applicationService;
        private readonly ITaskService _taskService;
        private readonly ITaskStorage _taskStorage;
        private readonly IApplicationStorage _applicationStorage;

        public ProofOfFundsTaskJob(IApplicationService applicationService,
                                   ITaskService taskService,
                                   ITaskStorage taskStorage,
                                   IApplicationStorage applicationStorage,
                                   IProofOfFundsCheckDataProvider jobDataProvider,
                                   ILogger logger)
            : base(jobDataProvider, logger)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
        }

        public static string Name => "proof-of-funds-task";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<ProofOfFundsTaskJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Backfill ProofOfFunds task from WX.UserVerification.");

        protected override async Task Execute(Batch<ProofOfFundsChecksData> batch, UserBatchJobSettings settings, CancellationToken cancellationToken)
        {
            foreach (var item in batch.Items)
            {
                var initiation = InitiationDto.CreateJob(Name);
                var taskState = EstimateTaskResult(item.HasOngoing);

                var application = await _applicationStorage.FindAsync(item.UserId, ProductType.WirexBasic);
                if (application == null)
                {
                    Logger.Warning($"Job {Name}. ApplicationId not found for userId {item.UserId}.");
                    continue;
                }

                var taskId = application.Tasks.FirstOrDefault(t => t.Type == TaskType.ProofOfFunds)?.Id
                           ?? await CreateTaskAsync(item.UserId, initiation);

                await SetTaskState(taskId, taskState, initiation);
                await _applicationService.AddRequiredTasksAsync(application.Id, new[] { taskId }, initiation);
            }

            static TaskState EstimateTaskResult(bool hasOngoingCheck) => hasOngoingCheck ? TaskState.Incomplete : TaskState.Completed;
        }

        private Task<Guid> CreateTaskAsync(Guid userId, InitiationDto initiation)
        {
            var newTask = new NewTaskDto
            {
                UserId = userId,
                Type = TaskType.ProofOfFunds,
                VariantId = _taskVariantId
            };
            return _taskService.CreateAsync(newTask, initiation);
        }

        private Task SetTaskState(Guid taskId, TaskState taskState, InitiationDto initiation)
        {
            return taskState switch
            {
                TaskState.Incomplete => _taskService.IncompleteAsync(taskId, initiation),
                TaskState.Completed => _taskService.CompleteAsync(taskId, TaskResult.Passed, initiation),
                _ => throw new ArgumentOutOfRangeException(nameof(taskState), taskState, "Unsupported task state.")
            };
        }
    }
}