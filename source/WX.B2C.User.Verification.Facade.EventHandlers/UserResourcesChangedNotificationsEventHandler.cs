using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Facade.EventHandlers.Mappers;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class UserResourcesChangedNotificationsEventHandler : BaseEventHandler,
                                                                   IEventHandler<VerificationDetailsUpdatedEvent>,
                                                                   IEventHandler<ApplicationStateChangedEvent>,
                                                                   IEventHandler<CollectionStepRequestedEvent>,
                                                                   IEventHandler<CollectionStepReadyForReviewEvent>,
                                                                   IEventHandler<CollectionStepCompletedEvent>,
                                                                   IEventHandler<ApplicationAutomatedEvent>
    {
        private readonly IUserNotificationService _notificationsService;
        private readonly IResourceNotificationMapper _resourceNotificationMapper;
        private readonly IApplicationStorage _applicationStorage;

        public UserResourcesChangedNotificationsEventHandler(
            IUserNotificationService notificationsService,
            IResourceNotificationMapper resourceNotificationMapper,
            IApplicationStorage applicationStorage,
            EventHandlingContext context) : base(context)
        {
            _notificationsService = notificationsService ?? throw new ArgumentNullException(nameof(notificationsService));
            _resourceNotificationMapper = resourceNotificationMapper ?? throw new ArgumentException(nameof(resourceNotificationMapper));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
        }

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args =>
            {
                var notification = _resourceNotificationMapper.Map(@event);
                return _notificationsService.SendAsync(notification);
            });

        public Task HandleAsync(CollectionStepRequestedEvent @event) =>
            Handle(@event, async args =>
            {
                var isApplicationAutomated = await IsAutomatedAsync(args.UserId);
                if (!isApplicationAutomated)
                    return;

                var notification = _resourceNotificationMapper.Map(@event);
                await _notificationsService.SendAsync(notification);
            });

        public Task HandleAsync(CollectionStepReadyForReviewEvent @event) =>
            Handle(@event, async args =>
            {
                var isApplicationAutomated = await IsAutomatedAsync(args.UserId);
                if (!isApplicationAutomated)
                    return;

                var notification = _resourceNotificationMapper.Map(@event);
                await _notificationsService.SendAsync(notification);
            });

        public Task HandleAsync(CollectionStepCompletedEvent @event) =>
            Handle(@event, async args =>
            {
                var isApplicationAutomated = await IsAutomatedAsync(args.UserId);
                if (!isApplicationAutomated)
                    return;

                var notification = _resourceNotificationMapper.Map(@event);
                await _notificationsService.SendAsync(notification);
            });

        public Task HandleAsync(VerificationDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var notification = _resourceNotificationMapper.Map(@event);
                return _notificationsService.SendAsync(notification);
            });

        public Task HandleAsync(ApplicationAutomatedEvent @event) =>
            Handle(@event, args =>
            {
                var notification = _resourceNotificationMapper.Map(@event);
                return _notificationsService.SendAsync(notification);
            });

        /// <summary>
        /// TODO ugly solution as
        /// 1) hidden dependency between IsAutomating and Actions resource
        /// 2) is not considered that user can have several products.
        /// </summary>
        private Task<bool> IsAutomatedAsync(Guid userId) =>
            _applicationStorage.IsAutomatedAsync(userId, ProductType.WirexBasic);
    }
}