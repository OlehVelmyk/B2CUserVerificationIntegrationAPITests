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
    internal class DeleteStepStateJob : BatchJob<EntityIdentifier, DeleteStepStateJobSettings>
    {
        private readonly ICollectionStepRepository _stepRepository;

        public DeleteStepStateJob(ICsvEntityProvider jobDataProvider,
                                  ICollectionStepRepository stepRepository,
                                  ILogger logger) : base(jobDataProvider, logger)
        {
            _stepRepository = stepRepository ?? throw new ArgumentNullException(nameof(stepRepository));
        }

        internal static string Name => "delete-step";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<DeleteStepStateJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Delete collection steps")
                 .StoreDurably();

        protected override Task Execute(Batch<EntityIdentifier> batch,
                                        DeleteStepStateJobSettings settings,
                                        CancellationToken cancellationToken) =>
            _stepRepository.DeleteAsync(batch.Items.Select(identifier => identifier.Id).ToArray());
    }
}