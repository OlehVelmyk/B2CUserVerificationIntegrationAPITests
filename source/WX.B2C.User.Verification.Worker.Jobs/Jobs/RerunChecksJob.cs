using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Events.Internal.EventArgs;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Messaging.Publisher;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class RerunCheckJob : BatchJob<ReRunCheckJobData, RerunChecksJobSettings>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICheckRepository _checkRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IOperationContextProvider _contextProvider;

        public RerunCheckJob(ITaskRepository taskRepository,
                             ICheckRepository checkRepository,
                             IChecksDataProvider jobDataProvider,
                             IEventPublisher eventPublisher,
                             IOperationContextProvider contextProvider,
                             ILogger logger)
            : base(jobDataProvider, logger)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _checkRepository = checkRepository ?? throw new ArgumentNullException(nameof(checkRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public static string Name => "rerun-checks";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<RerunCheckJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Re-run existing checks for users");

        protected override Task Execute(Batch<ReRunCheckJobData> batch,
                                        RerunChecksJobSettings settings,
                                        CancellationToken cancellationToken) =>
            batch.Items.Select(data => RerunUserChecks(settings, data)).WhenAll();

        private async Task RerunUserChecks(RerunChecksJobSettings settings, ReRunCheckJobData userChecks)
        {
            var logger = Logger.ForContext(nameof(userChecks.UserId), userChecks.UserId);
            
            var checksToRerun = await GetChecksToRerun(settings, userChecks, logger);
            if (checksToRerun.Count == 0)
                return;

            var instructedChecks = await InstructChecksAsync(userChecks, checksToRerun);
            await LinkChecksToTasksAsync(instructedChecks, checksToRerun);
            await RunChecksAsync(userChecks.UserId, instructedChecks);
        }

        private async Task<(Guid CheckId, Guid VariantId)[]> InstructChecksAsync(ReRunCheckJobData userChecks,
                                                                                 IReadOnlyCollection<CheckData> checksToRerun)
        {
            var checkVariants = checksToRerun.Select(check => new CheckVariantInfo
                                             {
                                                 Id = check.VariantId,
                                                 Provider = check.Provider,
                                                 Type = check.Type,
                                             })
                                             .ToArray();

            var instructedChecks = await _checkRepository.InstructAsync(userChecks.UserId, checkVariants);
            return instructedChecks;
        }

        private async Task LinkChecksToTasksAsync((Guid CheckId, Guid VariantId)[] instructedChecks,
                                                  IReadOnlyCollection<CheckData> checksToRerun)
        {
            var checksTasks = instructedChecks.GroupJoin(checksToRerun,
                                                         tuple => tuple.VariantId,
                                                         data => data.VariantId,
                                                         Map)
                                              .Flatten();
            await _taskRepository.AddTasksChecksAsync(checksTasks);

            IEnumerable<(Guid TaskId, Guid CheckId)> Map((Guid CheckId, Guid VariantId) newCheck,
                                                         IEnumerable<CheckData> existingCheck) =>
                existingCheck.SelectMany(data => data.RelatedTasks)
                             .Distinct()
                             .Select(taskId => (TaskId: taskId,
                                                newCheck.CheckId));
        }

        private async Task RunChecksAsync(Guid userId, (Guid CheckId, Guid VariantId)[] instructedChecks)
        {
            var eventArgs = ChecksCreatedEventArgs.Create(userId, instructedChecks.Select(tuple => tuple.CheckId).ToArray());
            var context = _contextProvider.GetContextOrDefault();
            var @event = new ChecksCreatedEvent(Guid.NewGuid().ToString(), eventArgs, context.OperationId, context.CorrelationId);
            await _eventPublisher.PublishAsync(@event);
        }

        private async Task<IReadOnlyCollection<CheckData>> GetChecksToRerun(RerunChecksJobSettings settings,
                                                                            ReRunCheckJobData userChecks,
                                                                            ILogger logger)
        {
            // TODO Potential improvements:
            // 1) Check that there are no already checks in pending or running state with the same variant but different check id
            // 2) Check that there are no already completed checks with the same variant but different check id AND performed later than provided check to rerun.

            var checksWithSameCheckVariant = userChecks.ChecksToRerun.ToLookup(data => data.VariantId)
                                                       .Where(checks => checks.Count() > 1)
                                                       .ToDictionary(checks => checks.Key, checks => checks.ToArray())
                                                       .ToArray();
            if (checksWithSameCheckVariant.Length > 0)
            {
                logger.Warning("Provided checks with the same check variant. Re-running checks is skipped for user. Checks:{@Checks}",
                               checksWithSameCheckVariant);
                return Array.Empty<CheckData>();
            }

            var checksByState = userChecks.ChecksToRerun.ToLookup(check => check.State);
            var checksToRerun = checksByState[CheckState.Error].Concat(checksByState[CheckState.Complete]).ToList();
            if (checksByState[CheckState.Pending].Any())
            {
                logger.Warning("Provided checks in pending state. Re-run is skipped. Pending checks {@PendingChecks}",
                               checksByState[CheckState.Pending]);
            }

            if (!checksByState[CheckState.Running].Any())
                return checksToRerun;

            if (settings.InstructionToCancel == null)
            {
                logger.Warning(
                "Provided checks in running state, but instruction to cancel is skipped. Re-run is skipped. Pending checks {@PendingChecks}",
                checksByState[CheckState.Pending]);
            }
            else
            {
                await _checkRepository.CancelChecksAsync(checksByState[CheckState.Running].Select(data => data.CheckId),
                                                         settings.InstructionToCancel);
                checksToRerun.AddRange(checksByState[CheckState.Running]);
            }
            return checksToRerun;
        }
    }
}