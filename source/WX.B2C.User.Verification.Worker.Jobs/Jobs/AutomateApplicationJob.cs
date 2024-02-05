using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class AutomateApplicationJob : BatchJob<EntityIdentifier, ApplicationJobSettings>
    {
        private readonly IApplicationService _applicationService;

        public AutomateApplicationJob(ICsvEntityProvider jobDataProvider,
                                      IApplicationService applicationService,
                                      ILogger logger) : base(jobDataProvider, logger)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        internal static string Name => "automate-application";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<AutomateApplicationJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Automate user application after backfill")
                 .StoreDurably();

        protected override Task Execute(Batch<EntityIdentifier> batch,
                                        ApplicationJobSettings settings,
                                        CancellationToken cancellationToken)
        {
            var initiation = InitiationDto.CreateJob(Name);
            return batch.Items.Foreach(app => _applicationService.AutomateAsync(app.Id, initiation));
        }
    }
}