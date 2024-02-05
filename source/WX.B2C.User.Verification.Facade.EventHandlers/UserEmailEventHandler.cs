using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.UserEmails;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.Messaging.Core;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class UserEmailEventHandler : BaseEventHandler,
                                           IEventHandler<ApplicationStateChangedEvent>,
                                           IEventHandler<CollectionStepRequestedEvent>,
                                           IEventHandler<UserReminderJobFinishedEvent>
    {
        /// <summary>
        /// TODO STAGE 2 Later, instead of ignoring reasons by initiators need to be
        /// TODO provided correct reason or reason code in policy.
        /// TODO For now we just ignore automatic reasons and use fallback.  
        /// </summary>
        private static readonly string[] IgnoredInitiators =
        {
            Initiators.Job,
            Initiators.System
        };

        private readonly IUserEmailService _userEmailService;

        public UserEmailEventHandler(
            IUserEmailService userEmailService, 
            EventHandlingContext context) : base(context)
        {
            _userEmailService = userEmailService ?? throw new ArgumentNullException(nameof(userEmailService));
        }

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args =>
            {
                var context = new ApplicationStateChangedEmailContext
                {
                    UserId = args.UserId,
                    NewState = args.NewState.To<Domain.Models.ApplicationState>()
                };

                return _userEmailService.SendAsync(context);
            });

        public Task HandleAsync(CollectionStepRequestedEvent @event) =>
            Handle(@event, args =>
            {
                var context = new CollectionStepRequestedEmailContext
                {
                    UserId = args.UserId,
                    XPath = args.XPath
                };

                if (!args.Initiation.Initiator.In(IgnoredInitiators))
                    context.Reason = args.Initiation.Reason;

                return _userEmailService.SendAsync(context);
            });

        public Task HandleAsync(UserReminderJobFinishedEvent @event) =>
            Handle(@event, args =>
            {
                var context = new ReminderSentEmailContext
                {
                    UserId = args.UserId,
                };            
                return _userEmailService.SendAsync(context);
            });
    }
}