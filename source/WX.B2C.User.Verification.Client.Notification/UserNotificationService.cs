using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.Client.Notification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Client.Notification
{
    internal class UserNotificationService : IUserNotificationService
    {
        private readonly INotificationClientCommandsPublisher _publisher;

        public UserNotificationService(INotificationClientCommandsPublisher publisher)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public Task SendAsync(UserResourcesChangedNotificationDto notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            var parameters = new NotifyUserResourcesChangedParameters
            {
                UserId = notification.UserId,
                CorrelationId = notification.CorrelationId.ToString(),
                ChangeId = Guid.NewGuid(),
                Created = ToLower(notification.Created),
                Updated = ToLower(notification.Updated),
                Deleted = ToLower(notification.Deleted)
            };

            return _publisher.Publish(parameters, CancellationToken.None);
        }

        public Task SendAsync(TextNotificationDto notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            var builder = new SendTextNotificationParametersBuilder()
                  .WithUserId(notification.UserId)
                  .WithCorrelationId(notification.CorrelationId.ToString())
                  .WithTemplate(notification.Template)
                  .WithTemplateParameters(notification.TemplateParameters)
                  .WithNotificationImportance(NotificationImportance.Information);

            
            return _publisher.Publish(builder.Build(), CancellationToken.None);
        }

        private static string[] ToLower(IEnumerable<string> array) =>
            array?.Select(x => x.ToLowerInvariant()).ToArray();
    }
}