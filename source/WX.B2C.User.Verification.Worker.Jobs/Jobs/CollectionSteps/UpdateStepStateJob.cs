using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class UpdateStepStateJob : BatchJob<EntityIdentifier, UpdateStepStateJobSettings>
    {
        private readonly ICollectionStepRepository _stepRepository;

        public UpdateStepStateJob(ICsvEntityProvider jobDataProvider,
                                  ICollectionStepRepository stepRepository,
                                  ILogger logger) : base(jobDataProvider, logger)
        {
            _stepRepository = stepRepository ?? throw new ArgumentNullException(nameof(stepRepository));
        }

        internal static string Name => "update-step";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<UpdateStepStateJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Change collection steps state")
                 .StoreDurably();

        protected override Task Execute(Batch<EntityIdentifier> batch,
                                        UpdateStepStateJobSettings settings,
                                        CancellationToken cancellationToken)
        {
            // TODO implement later validation mechanism:
            // TODO register validators for each setting where needed and call corresponding in ScheduleAsync.
            // TODO potential inconsistency: isReviewNeeded + completed, but no result
            // TODO potential inconsistency: !isReviewNeeded , but has result
            // TODO potential inconsistency: !completed, but has result

            return _stepRepository.UpdateAsync(batch.Items.Select(identifier => identifier.Id).ToArray(), settings.Patch);
        }
    }
}