using System;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class ScheduledTriggerJob : IJob
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;

        public ScheduledTriggerJob(IEventPublisher eventPublisher, ILogger logger)
        {
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _logger = logger?.ForContext<ScheduledTriggerJob>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public static string Name => JobConstants.ScheduledTriggerJobName;

        public static JobBuilderFactory Builder =>
            parameters => JobBuilder
                          .Create<ScheduledTriggerJob>()
                          .WithIdentity(Name, parameters.Get<Guid>(JobConstants.UserId).ToString())
                          .WithDescription("Scheduled trigger execution")
                          .StoreDurably()
                          .RequestRecovery();

        public static TriggerKeyFactory TriggerKeyFactory => parameters =>
            new TriggerKey(parameters.Get<Guid>(JobConstants.TriggerId).ToString());

        public async Task Execute(IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            try
            {
                var settings = context.GetSettings<ScheduledTriggerJobSetting>();
                var domainEvent = ScheduledTriggerJobFinished.Create(settings.TriggerId, settings.UserId, DateTime.UtcNow);
                await _eventPublisher.PublishAsync(domainEvent);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Could not execute scheduled trigger.");
                throw new JobExecutionException(exc);
            }
        }
    }

    public class ScheduledTriggerJobSetting
    {
        public Guid TriggerId { get; set; }

        public Guid UserId { get; set; }
    }
}