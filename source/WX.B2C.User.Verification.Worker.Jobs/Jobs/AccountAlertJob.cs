using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.EventArgs;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Messaging.Publisher;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal class AccountAlertJob : BatchJob<AccountAlertInfo, BatchJobSettings>
    {
        private readonly IEventPublisher _publisher;
        private readonly IOperationContextProvider _operationContextProvider;

        public AccountAlertJob(IEventPublisher publisher,
                               IOperationContextProvider operationContextProvider,
                               IAccountAlertInfoProvider jobDataProvider,
                               ILogger logger) 
            : base(jobDataProvider, logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
        }

        public static string Name => "account-refresh-alert";

        public static JobBuilderFactory Builder =>
            _ => JobBuilder
                 .Create<AccountAlertJob>()
                 .WithIdentity(Name, "Important")
                 .WithDescription("Alert to review user")
                 .StoreDurably()
                 .RequestRecovery();

        protected override async Task Execute(Batch<AccountAlertInfo> batch, BatchJobSettings settings, CancellationToken cancellationToken)
        {
            var operationContext = _operationContextProvider.GetContextOrDefault(); // TODO: Now provide empty operation context, fix it
            foreach (var item in batch.Items)
            {
                var args = AccountAlertCreatedEventArgs.Create(item.UserId,
                                                               item.RiskLevel.To<RiskLevel>(),
                                                               item.ApplicationState.To<ApplicationState>(), 
                                                               item.LastApprovedDate, 
                                                               item.Turnover);
                var @event = new AccountAlertCreatedEvent(item.UserId.ToString(), args, operationContext.OperationId, operationContext.CorrelationId);
                await _publisher.PublishAsync(@event);
            }
        }
    }
}
