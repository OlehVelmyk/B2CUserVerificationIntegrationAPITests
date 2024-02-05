using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.Survey.Events;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class CollectionStepEventHandler : BaseEventHandler,
                                                IEventHandler<DocumentSubmittedEvent>,
                                                IEventHandler<UserSurveySubmittedEvent>,
                                                IEventHandler<PersonalDetailsUpdatedEvent>,
                                                IEventHandler<VerificationDetailsUpdatedEvent>
    {
        private readonly ICollectionStepEventObserver _collectionStepObserver;

        public CollectionStepEventHandler(
            ICollectionStepEventObserver collectionStepObserver,
            EventHandlingContext context) : base(context)
        {
            _collectionStepObserver = collectionStepObserver ?? throw new ArgumentNullException(nameof(collectionStepObserver));
        }

        public Task HandleAsync(DocumentSubmittedEvent @event) =>
            Handle(@event, args =>
            {
                var userId = args.UserId;
                var changes = new string[] { new DocumentXPath(args.Category.To<DocumentCategory>(), args.Type) };
                return _collectionStepObserver.OnDocumentSubmitted(userId, changes);
            });

        public Task HandleAsync(UserSurveySubmittedEvent @event) =>
            Handle(@event, args =>
            {
                var userId = args.UserId;
                var changes = new string[] { new SurveyXPath(args.SurveyId) };
                return _collectionStepObserver.OnSurveySubmitted(userId, changes);
            });

        public Task HandleAsync(PersonalDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var userId = args.UserId;
                var changes = GetChangedProperties(args.Changes);
                return changes.Length == 0
                    ? Task.CompletedTask
                    : _collectionStepObserver.OnPersonalDetailsUpdated(userId, changes);
            });

        public Task HandleAsync(VerificationDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var userId = args.UserId;
                var changes = GetChangedProperties(args.Changes);
                return changes.Length == 0
                    ? Task.CompletedTask
                    : _collectionStepObserver.OnVerificationDetailsUpdated(userId, changes);
            });

        private static string[] GetChangedProperties(IEnumerable<PropertyChangeDto> changes) =>
            changes.Where(change => !change.IsReset)
                   .Select(change => change.PropertyName)
                   .ToArray();
    }
}
