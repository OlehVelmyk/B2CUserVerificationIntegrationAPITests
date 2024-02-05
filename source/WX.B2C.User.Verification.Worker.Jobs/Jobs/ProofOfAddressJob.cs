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
    internal sealed class ProofOfAddressJob : BatchJob<ProofOfAddressData, ProofOfAddressJobSetting>
    {
        private readonly ITaskService _taskService;
        private readonly ITaskStorage _taskStorage;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IApplicationService _applicationService;

        public ProofOfAddressJob(
            IProofOfAddressProvider jobDataProvider,
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

        public static string Name => "proof-of-address-job";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<ProofOfAddressJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Create ProofOfAddress task in corresponding state without collection steps and without performed checks.");

        protected override async Task Execute(Batch<ProofOfAddressData> batch, ProofOfAddressJobSetting settings, CancellationToken cancellationToken)
        {
            foreach (var proofOfAddress in batch.Items)
            {
                if (proofOfAddress.Status == null && proofOfAddress.IsCountryMatchedByIp == null)
                {
                    Logger.Warning($"Job {Name}. ProofOfAddress not found for userId {proofOfAddress.UserId}");
                    continue;
                }

                var application = await _applicationStorage.FindAsync(proofOfAddress.UserId, ProductType.WirexBasic);
                if (application == null)
                {
                    Logger.Warning($"Job {Name}. ApplicationId not found for userId {proofOfAddress.UserId}.");
                    continue;
                }

                var task = application.Tasks.FirstOrDefault(t => t.Type == TaskType.Address)
                           ?? await CreateTaskAsync(proofOfAddress.UserId, application.Id, settings.TaskVariantId);

                await SetCorrectTaskState(proofOfAddress.Status, proofOfAddress.IsCountryMatchedByIp, task);
            }
        }

        private async Task<ApplicationTaskDto> CreateTaskAsync(Guid userId, Guid applicationId, Guid taskVariantId)
        {
            var userTasks = await _taskStorage.GetAllAsync(userId, TaskType.Address);
            if (userTasks is { Length: > 1 })
                Logger.Warning($"Job {Name}. UserId {userId} has many tasks for TaskType: TaskType.Address");

            var initiation = InitiationDto.CreateJob(Name);

            var applicationTask = Map(userTasks.FirstOrDefault());
            if (applicationTask == null)
            {
                var newTaskDto = new NewTaskDto
                {
                    AcceptanceCheckIds = new Guid[] { },
                    CollectionStepIds = new Guid[] { },
                    Type = TaskType.Address,
                    UserId = userId,
                    VariantId = taskVariantId
                };

                applicationTask = new ApplicationTaskDto
                {
                    Id = await _taskService.CreateAsync(newTaskDto, initiation),
                };
            }

            var tasks = new[] { applicationTask.Id };
            await _applicationService.AddRequiredTasksAsync(applicationId, tasks, initiation);
            return applicationTask;
        }

        private async Task SetCorrectTaskState(ProofOfAddressCheckStatus? status, bool? isCountryMatchedByIp, ApplicationTaskDto task)
        {
            var initiation = InitiationDto.CreateJob(Name);
            if (status.HasValue && status.Value == ProofOfAddressCheckStatus.Completed || !status.HasValue && isCountryMatchedByIp.HasValue && isCountryMatchedByIp.Value)
            {
                await _taskService.CompleteAsync(task.Id, TaskResult.Passed, initiation);
            }
            else
            {
                await _taskService.IncompleteAsync(task.Id, initiation);
            }
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
