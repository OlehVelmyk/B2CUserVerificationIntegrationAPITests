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
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class InstructChecksJob : BatchJob<UserInstructChecksData, InstructChecksJobSettings>
    {
        private readonly ICheckService _checkService;
        private readonly ITaskRepository _taskRepository;
        private readonly ICheckRepository _checkRepository;
        private readonly AsyncLazy<CheckVariantInfo[]> _checksVariants;

        public InstructChecksJob(ICheckService checkService,
                                 ITaskRepository taskRepository,
                                 ICheckRepository checkRepository,
                                 IVerificationPolicyStorage policyStorage,
                                 IInstructChecksDataProvider jobDataProvider,
                                 ILogger logger)
            : base(jobDataProvider, logger)
        {
            if (policyStorage == null)
                throw new ArgumentNullException(nameof(policyStorage));

            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _checkRepository = checkRepository ?? throw new ArgumentNullException(nameof(checkRepository));
            _checksVariants = new AsyncLazy<CheckVariantInfo[]>(() => policyStorage.GetChecksInfoAsync());
        }

        public static string Name => "instruct-checks";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<InstructChecksJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Instruct checks for users");

        protected override Task Execute(Batch<UserInstructChecksData> batch,
                                              InstructChecksJobSettings settings,
                                              CancellationToken cancellationToken) =>
            batch.Items.Select(data => InstructUserChecks(settings, data)).WhenAll();

        private Task InstructUserChecks(InstructChecksJobSettings settings, UserInstructChecksData item)
        {
            var logger = Logger.ForContext(nameof(item.UserId), item.UserId);

            if (!item.ApplicationId.HasValue)
            {
                logger.Information("Instructing checks skipped:No application created for User");
                return Task.CompletedTask;
            }

            return item.TaskAcceptanceChecks
                       .Select(checks => InstructTaskChecks(settings, item, checks, logger))
                       .WhenAll();
        }

        private async Task InstructTaskChecks(InstructChecksJobSettings settings,
                                              UserInstructChecksData item,
                                              TaskAcceptanceChecksData data,
                                              ILogger parentLogger)
        {
            var logger = parentLogger.ForContext(nameof(data.TaskType), data.TaskType);

            var tasks = data.Tasks;
            var task = FindValidTask(tasks, logger);
            if (task == null)
            {
                logger.Information("Skipped instructing checks for task type:{TaskType}", data.TaskType);
                return;
            }

            var checkVariants = await FindChecksToInstructAsync(task, data, item, logger);
            if (checkVariants == null)
                return;

            await SaveChecksAsync(item.UserId, task.Id, checkVariants, settings.DirectUpdate);
        }

        private static TaskAcceptanceChecksData.TaskData FindValidTask(TaskAcceptanceChecksData.TaskData[] tasks,
                                                                       ILogger logger)
        {
            if (tasks.IsEmpty())
            {
                logger.Warning("Not found task with required type");
                return null;
            }

            if (tasks.Length != 1)
            {
                logger.Error("Multiple tasks {TasksNumber} are found with same type", tasks.Length);
                return null;
            }

            return tasks[0];
        }

        private async Task<CheckVariantInfo[]> FindChecksToInstructAsync(TaskAcceptanceChecksData.TaskData task,
                                                                         TaskAcceptanceChecksData data,
                                                                         UserInstructChecksData item,
                                                                         ILogger logger)
        {
            var existingChecks = task.ExitingChecks;
            var checksVariantsIdsToInstruct = data.NewChecks.Except(existingChecks).ToArray();
            if (checksVariantsIdsToInstruct.Length == 0)
                return null;

            var checkVariants = (await _checksVariants.Value).Where(info => info.Id.In(checksVariantsIdsToInstruct)).ToArray();
            if (checkVariants.Length < checksVariantsIdsToInstruct.Length)
            {
                logger.Error("Check variants not found {@MissedChecks}. UserId: {UserId}",
                             checksVariantsIdsToInstruct.Except(checkVariants.Select(info => info.Id)),
                             item.UserId);

                return null;
            }

            return checkVariants;
        }

        private async Task SaveChecksAsync(Guid userId,
                                           Guid taskId,
                                           CheckVariantInfo[] checkVariants,
                                           bool directUpdate)
        {
            var initiationDto = InitiationDto.CreateJob(Name);

            if (directUpdate)
            {
                var instructedChecks = await _checkRepository.InstructAsync(userId, checkVariants);
                await _taskRepository.AddTaskChecksAsync(taskId, instructedChecks.Select(tuple => tuple.CheckId).ToArray());
            }
            else
            {
                await checkVariants.Foreach(async info =>
                {
                    var newCheckDto = new NewCheckDto
                    {
                        Provider = info.Provider,
                        CheckType = info.Type,
                        VariantId = info.Id,
                        RelatedTasks = new[] { taskId }
                    };

                    await _checkService.RequestAsync(userId, newCheckDto, initiationDto);
                });
            }
        }
    }
}