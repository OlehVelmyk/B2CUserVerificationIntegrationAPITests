using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class TriggerActionsRequestingJob : BatchJob<Models.User, TriggerManagingSettings>
    {
        private readonly IEventPublisher _eventPublisher;

        public TriggerActionsRequestingJob(ICsvBlobDataProvider<Models.User> jobDataProvider, IEventPublisher eventPublisher, ILogger logger)
            : base(jobDataProvider, logger)
        {
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public static string Name => "trigger-actions-requesting-job";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<TriggerActionsRequestingJob>()
                 .WithIdentity(Name, "Backfill")
                 .WithDescription("Requires performing actions with user triggers");

        protected override async Task Execute(Batch<Models.User> batch, TriggerManagingSettings settings, CancellationToken cancellationToken)
        {
            var actions = settings.Actions.ToHashSet();
            foreach (var user in batch.Items)
            {
                var @event = UserTriggersActionRequired.Create(user.UserId, actions, settings.TriggerPolicyId);
                await _eventPublisher.PublishAsync(@event);
            }
        }
    }
}