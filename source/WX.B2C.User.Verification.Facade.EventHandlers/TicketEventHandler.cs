using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.EventHandlers.Mappers;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class TicketEventHandler : BaseEventHandler,
                                        IEventHandler<CheckCompletedEvent>,
                                        IEventHandler<CollectionStepReadyForReviewEvent>,
                                        IEventHandler<AccountAlertCreatedEvent>
    {
        private readonly ITicketService _ticketService;
        private readonly ICompletedCheckMapper _completedCheckMapper;

        public TicketEventHandler(
            ITicketService ticketService,
            ICompletedCheckMapper completedCheckMapper,
            EventHandlingContext context) : base(context)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _completedCheckMapper = completedCheckMapper ?? throw new ArgumentNullException(nameof(completedCheckMapper));
        }

        public Task HandleAsync(CheckCompletedEvent @event) =>
            Handle(@event, args => _ticketService.SendAsync(_completedCheckMapper.Map(args)));

        public Task HandleAsync(CollectionStepReadyForReviewEvent @event) =>
            Handle(@event, args =>
            {
                var context = new ReviewCollectionStepTicketContext { XPath = args.XPath, UserId = args.UserId };
                return _ticketService.SendAsync(context);
            });

        public Task HandleAsync(AccountAlertCreatedEvent @event) =>
            Handle(@event, args =>
            {
                var context = new AccountAlertTicketContext
                {
                    UserId = args.UserId,
                    ApplicationState = args.ApplicationState.To<ApplicationState>(),
                    LastApprovedDate = args.LastApprovedDate,
                    RiskLevel = args.RiskLevel.To<RiskLevel>(),
                    Turnover = args.Turnover,
                };
                return _ticketService.SendAsync(context);
            });
    }
}