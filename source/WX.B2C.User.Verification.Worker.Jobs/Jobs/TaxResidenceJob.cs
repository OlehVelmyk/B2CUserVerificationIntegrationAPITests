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
    internal sealed class TaxResidenceJob : BatchJob<TaxResidenceData, TaxResidenceJobSetting>
    {
        private readonly ITaskService _taskService;
        private readonly ITaskStorage _taskStorage;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IApplicationService _applicationService;
        private readonly ITaxResidenceAddressProvider _taxResidenceAddressProvider;

        public TaxResidenceJob(
            ITaxResidenceProvider jobDataProvider,
            ITaxResidenceAddressProvider taxResidenceAddressProvider,
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
            _taxResidenceAddressProvider = taxResidenceAddressProvider ?? throw new ArgumentNullException(nameof(taxResidenceAddressProvider));
        }

        public static string Name => "tax-residence-job";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<TaxResidenceJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription($"Create TaxResidence task in corresponding state without collection steps and without performed checks.");

        protected override async Task Execute(Batch<TaxResidenceData> batch, TaxResidenceJobSetting settings, CancellationToken cancellationToken)
        {
            var userIds = batch.Items.Select(i => i.ProfileInformationId).ToArray();
            var users = await _taxResidenceAddressProvider.GetAsync(settings, userIds);

            foreach (var taxResidence in batch.Items)
            {
                var userId = taxResidence.ProfileInformationId;

                var application = await _applicationStorage.FindAsync(userId, ProductType.WirexBasic);
                if (application == null)
                {
                    Logger.Warning($"Job {Name}. ApplicationId not found for userId {userId}.");
                    continue;
                }

                var task = application.Tasks.FirstOrDefault(t => t.Type == TaskType.TaxResidence)
                           ?? await CreateTaskAsync(userId, application.Id, settings.TaskVariantId);

                var taxResidences = taxResidence.TaxResidencies;
                var verificationStopReason = taxResidence.VerificationStopReason;
                var residenceCountry = users.FirstOrDefault(u => u.UserId == userId)?.Country;

                await SetCorrectTaskState(userId, taxResidences, verificationStopReason, residenceCountry, task);
            }
        }

        private async Task<ApplicationTaskDto> CreateTaskAsync(Guid userId, Guid applicationId, Guid taskVariantId)
        {
            var userTasks = await _taskStorage.GetAllAsync(userId, TaskType.TaxResidence);
            if (userTasks is { Length: > 1 })
                Logger.Warning($"Job {Name}. UserId {userId} has many tasks for TaskType: TaskType.TaxResidence");

            var initiation = InitiationDto.CreateJob(Name);

            var applicationTask = Map(userTasks.FirstOrDefault());
            if (applicationTask == null)
            {
                var newTaskDto = new NewTaskDto
                {
                    AcceptanceCheckIds = new Guid[] { },
                    CollectionStepIds = new Guid[] { },
                    Type = TaskType.TaxResidence,
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

        private async Task SetCorrectTaskState(Guid userId, string taxResidences, VerificationStopReason verificationStopReason, string residenceCountry, ApplicationTaskDto task)
        {
            var initiation = InitiationDto.CreateJob(Name);

            if (string.IsNullOrWhiteSpace(taxResidences))
            {
                await _taskService.IncompleteAsync(task.Id, initiation);
                Logger.Warning($"Job {Name}. UserId {userId} didn't have TaxResidence.");
                return;
            }

            var isUsTaxResidence = taxResidences.Contains("us", StringComparison.InvariantCultureIgnoreCase);
            var isUsResidenceCountry = residenceCountry.Contains("us", StringComparison.InvariantCultureIgnoreCase);

            if (isUsTaxResidence)
            {
                if (isUsResidenceCountry)
                {
                    await _taskService.CompleteAsync(task.Id, TaskResult.Passed, initiation);
                }
                else
                {
                    switch (verificationStopReason)
                    {
                        case VerificationStopReason.UsaCitizenFormSubmission:
                            await _taskService.IncompleteAsync(task.Id, initiation);
                            break;

                        case VerificationStopReason.UsaCitizenFormDone:
                            await _taskService.CompleteAsync(task.Id, TaskResult.Passed, initiation);
                            Logger.Warning($"Job {Name}. UserId {userId}. TaxResidence task completed. User need review VerificationStopReason: UsaCitizenFormDone.");
                            break;

                        case VerificationStopReason.UsaCitizenFormReviewed:
                            await _taskService.CompleteAsync(task.Id, TaskResult.Passed, initiation);
                            break;
                    }
                }
            }
            else
            {
                await _taskService.CompleteAsync(task.Id, TaskResult.Passed, initiation);
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
