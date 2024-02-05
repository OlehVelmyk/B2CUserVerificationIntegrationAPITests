using System;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class UserReminderJob : IJob
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger _logger;

        public UserReminderJob(IEventPublisher eventPublisher, ILogger logger)
        {
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _logger = logger?.ForContext<UserReminderJob>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public static string Name => JobConstants.UserReminderJob;

        public static JobBuilderFactory Builder =>
            parameters => JobBuilder
                          .Create<UserReminderJob>()
                          .WithIdentity(JobKeyFactory(parameters))
                          .WithDescription("User reminder execution.")
                          .StoreDurably()
                          .RequestRecovery();

        public static JobKeyFactory JobKeyFactory => parameters =>
            new JobKey(Name, parameters.Get<Guid>(JobConstants.UserId).ToString());

        public static TriggerKeyFactory TriggerKeyFactory => parameters =>
            new TriggerKey(parameters.Get<Guid>(JobConstants.ReminderId).ToString(), Name);

        public async Task Execute(IJobExecutionContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            try
            {
                var settings = context.GetSettings<UserReminderJobSetting>();
                var domainEvent = UserReminderJobFinished.Create(settings.ReminderId, settings.UserId);
                await _eventPublisher.PublishAsync(domainEvent);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Could not execute user reminder.");
                throw new JobExecutionException(exc);
            }
        }

        private class UserReminderJobSetting
        {
            public Guid ReminderId { get; set; }

            public Guid UserId { get; set; }
        }
    }
}
