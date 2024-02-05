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
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class CancelCheckJob : BatchJob<EntityIdentifier, CancelChecksJobSettings>
    {
        private readonly ICheckRepository _checkRepository;

        public CancelCheckJob(ICheckRepository checkRepository,
                              ICsvEntityProvider jobDataProvider,
                              ILogger logger)
            : base(jobDataProvider, logger)
        {
            _checkRepository = checkRepository ?? throw new ArgumentNullException(nameof(checkRepository));
        }

        public static string Name => "cancel-checks";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<CancelCheckJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Cancel existing checks for users");

        protected override Task Execute(Batch<EntityIdentifier> batch,
                                        CancelChecksJobSettings settings,
                                        CancellationToken cancellationToken)
        {
            if (settings.InstructionToCancel == null)
                throw new ArgumentNullException(nameof(settings.InstructionToCancel));

            return _checkRepository.CancelChecksAsync(batch.Items.Select(check => check.Id), settings.InstructionToCancel);
        }
    }
}