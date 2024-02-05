using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class TaskEventHandler : BaseEventHandler,
                                      IEventHandler<CollectionStepCompletedEvent>,
                                      IEventHandler<CheckCompletedEvent>,
                                      IEventHandler<CollectionStepRequiredEvent>
    {
        private readonly ITaskEventObserver _taskEventObserver;

        public TaskEventHandler(
            ITaskEventObserver taskEventObserver,
            EventHandlingContext context) : base(context)
        {
            _taskEventObserver = taskEventObserver ?? throw new ArgumentNullException(nameof(taskEventObserver));
        }

        public Task HandleAsync(CollectionStepCompletedEvent @event) =>
            Handle(@event, args =>
            {
                var collectionStepId = args.CollectionStepId;
                var reviewResult = args.ReviewResult;
                return reviewResult switch
                {
                    CollectionStepReviewResult.Rejected => Task.CompletedTask,
                    CollectionStepReviewResult.Approved => _taskEventObserver.OnCollectionStepCompleted(collectionStepId),
                    null => _taskEventObserver.OnCollectionStepCompleted(collectionStepId),
                    _ => throw new ArgumentOutOfRangeException(nameof(reviewResult), reviewResult, "Unsupported review result.")
                };
            });

        public Task HandleAsync(CheckCompletedEvent @event) =>
            Handle(@event, args =>
            {
                // TODO open question: theoretically CheckCompletedEvent can be received before CheckCreatedEvent
                // TODO this is unlikely to happen, but later we need to prepare guard for such potential issue 

                var checkResult = args.Result;
                return checkResult switch
                {
                    CheckResult.Passed => _taskEventObserver.OnCheckPassed(args.CheckId),
                    CheckResult.Failed => _taskEventObserver.OnCheckFailed(args.CheckId),
                    _ => throw new ArgumentOutOfRangeException(nameof(checkResult), checkResult, "Unsupported check result.")
                };
            });

        public Task HandleAsync(CollectionStepRequiredEvent @event) =>
            Handle(@event, args => _taskEventObserver.OnCollectionStepRequired(args.CollectionStepId));
    }
}