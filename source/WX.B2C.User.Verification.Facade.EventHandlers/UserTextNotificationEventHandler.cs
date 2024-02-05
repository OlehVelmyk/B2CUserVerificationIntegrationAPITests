using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.EventHandlers.Mappers;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class UserTextNotificationEventHandler : BaseEventHandler,
                                                      IEventHandler<ApplicationStateChangedEvent>,
                                                      IEventHandler<CollectionStepRequestedEvent>,
                                                      IEventHandler<UserReminderJobFinishedEvent>
    {
        private readonly IUserNotificationService _notificationsService;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly ITextNotificationMapper _textNotificationMapper;

        public UserTextNotificationEventHandler(
            IUserNotificationService notificationsService,
            ITextNotificationMapper textNotificationMapper,
            ICollectionStepStorage collectionStepStorage,
            EventHandlingContext context) : base(context)
        {
            _notificationsService = notificationsService ?? throw new ArgumentNullException(nameof(notificationsService));
            _textNotificationMapper = textNotificationMapper ?? throw new ArgumentNullException(nameof(textNotificationMapper));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
        }

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args =>
            {
                var applicationState = args.NewState;

                string template;
                if (applicationState is ApplicationState.Approved)
                    template = TextNotificationTemplates.VerificationSuccessful;
                else if (applicationState is ApplicationState.Rejected or ApplicationState.Cancelled)
                    template = TextNotificationTemplates.VerificationRejected;
                else
                    return Task.CompletedTask;

                var textNotification = _textNotificationMapper.Map(@event, template);
                return _notificationsService.SendAsync(textNotification);
            });

        public Task HandleAsync(CollectionStepRequestedEvent @event) =>
            Handle(@event, async args =>
            {
                var xPathes = new string[]
                {
                    XPathes.ProofOfAddressDocument,
                    XPathes.ProofOfFundsDocument,
                    XPathes.W9Form
                };

                var xpath = args.XPath;
                if (!xpath.In(xPathes) || !args.IsRequired)
                    return;

                var steps = await _collectionStepStorage.GetAllAsync(args.UserId, xpath);
                var template = TextNotificationTemplates.CreateDocsRequiredTemplate(steps.Length == 1);
                var textNotification = _textNotificationMapper.Map(@event, template);
                await _notificationsService.SendAsync(textNotification);
            });

        public Task HandleAsync(UserReminderJobFinishedEvent @event) =>
            Handle(@event, _ =>
            {
                var template = TextNotificationTemplates.DataRequiredTemplate;
                var textNotification = _textNotificationMapper.Map(@event, template);
                return _notificationsService.SendAsync(textNotification);
            });
    }
}