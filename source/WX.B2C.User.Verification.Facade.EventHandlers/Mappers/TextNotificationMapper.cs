using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    internal interface ITextNotificationMapper
    {
        TextNotificationDto Map(ApplicationStateChangedEvent @event, string template, string[] parameters = null);

        TextNotificationDto Map(CollectionStepRequestedEvent @event, string template, string[] parameters = null);

        TextNotificationDto Map(UserReminderJobFinishedEvent @event, string template, string[] parameters = null);
    }

    internal class TextNotificationMapper : ITextNotificationMapper
    {
        public TextNotificationDto Map(ApplicationStateChangedEvent @event, string template, string[] parameters = null)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new()
            {
                UserId = @event.EventArgs.UserId,
                CorrelationId = @event.CorrelationId,
                Template = template,
                TemplateParameters = parameters
            };
        }

        public TextNotificationDto Map(CollectionStepRequestedEvent @event, string template, string[] parameters = null)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new()
            {
                UserId = @event.EventArgs.UserId,
                CorrelationId = @event.CorrelationId,
                Template = template,
                TemplateParameters = parameters
            };
        }

        public TextNotificationDto Map(UserReminderJobFinishedEvent @event, string template, string[] parameters = null)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (@event.EventArgs == null)
                throw new ArgumentNullException(nameof(@event.EventArgs));

            return new()
            {
                UserId = @event.EventArgs.UserId,
                CorrelationId = @event.CorrelationId,
                Template = template,
                TemplateParameters = parameters
            };
        }
    }
}