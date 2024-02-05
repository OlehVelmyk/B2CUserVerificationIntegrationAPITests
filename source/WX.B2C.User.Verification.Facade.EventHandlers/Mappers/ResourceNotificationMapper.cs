using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    internal static class ResourceNames
    {
        public const string VerificationActions = nameof(VerificationActions);
        public const string VerificationApplications = nameof(VerificationApplications);
        public const string VerificationDetails = nameof(VerificationDetails);

        public static string GlobalResource(string resource) => resource;
        public static string ObjectResource(string resource, Guid objectId) => $"{resource}:{objectId}";
    }

    internal interface IResourceNotificationMapper
    {
        UserResourcesChangedNotificationDto Map(VerificationDetailsUpdatedEvent @event);

        UserResourcesChangedNotificationDto Map(ApplicationStateChangedEvent @event);

        UserResourcesChangedNotificationDto Map(CollectionStepRequestedEvent @event);

        UserResourcesChangedNotificationDto Map(CollectionStepReadyForReviewEvent @event);

        UserResourcesChangedNotificationDto Map(CollectionStepCompletedEvent @event);

        UserResourcesChangedNotificationDto Map(ApplicationAutomatedEvent @event);
    }

    internal class ResourceNotificationMapper : IResourceNotificationMapper
    {
        private static readonly ApplicationState[] StatesWithoutActions = { ApplicationState.Cancelled, ApplicationState.Rejected };
        
        public UserResourcesChangedNotificationDto Map(VerificationDetailsUpdatedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new UserResourcesChangedNotificationDto
            {
                CorrelationId = @event.CorrelationId,
                Updated = new[] { ResourceNames.GlobalResource(ResourceNames.VerificationDetails) },
                UserId = @event.EventArgs.UserId
            };
        }

        public UserResourcesChangedNotificationDto Map(ApplicationStateChangedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            var updated = new List<string>
                { ResourceNames.ObjectResource(ResourceNames.VerificationApplications, @event.EventArgs.ApplicationId) };
            
            // NOTE: could be useless push when application without open actions moved from/to StatesWithoutActions.
            // Agreed that we can ignore such case until performance issues detected.
            if (@event.EventArgs.NewState.In(StatesWithoutActions) || @event.EventArgs.PreviousState.In(StatesWithoutActions))
                updated.Add(ResourceNames.GlobalResource(ResourceNames.VerificationActions));
            
            return new UserResourcesChangedNotificationDto
            {
                CorrelationId = @event.CorrelationId,
                Updated = updated.ToArray(),
                UserId = @event.EventArgs.UserId
            };
        }

        public UserResourcesChangedNotificationDto Map(CollectionStepRequestedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new UserResourcesChangedNotificationDto
            {
                CorrelationId = @event.CorrelationId,
                Updated = new[] { ResourceNames.GlobalResource(ResourceNames.VerificationActions) },
                UserId = @event.EventArgs.UserId
            };
        }

        public UserResourcesChangedNotificationDto Map(CollectionStepReadyForReviewEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new UserResourcesChangedNotificationDto
            {
                CorrelationId = @event.CorrelationId,
                Updated = new[] { ResourceNames.GlobalResource(ResourceNames.VerificationActions) },
                UserId = @event.EventArgs.UserId
            };
        }

        public UserResourcesChangedNotificationDto Map(CollectionStepCompletedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new UserResourcesChangedNotificationDto
            {
                CorrelationId = @event.CorrelationId,
                Updated = new[] { ResourceNames.GlobalResource(ResourceNames.VerificationActions) },
                UserId = @event.EventArgs.UserId
            };
        }

        public UserResourcesChangedNotificationDto Map(ApplicationAutomatedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new UserResourcesChangedNotificationDto
            {
                CorrelationId = @event.CorrelationId,
                Updated = new[] { ResourceNames.GlobalResource(ResourceNames.VerificationActions) },
                UserId = @event.EventArgs.UserId
            };
        }
    }
}