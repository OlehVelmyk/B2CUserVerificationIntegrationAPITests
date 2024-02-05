using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class ApplicationEventHandler : BaseEventHandler,
                                             IEventHandler<TaskCompletedEvent>,
                                             IEventHandler<TaskIncompleteEvent>,
                                             IEventHandler<CheckCompletedEvent>,
                                             IEventHandler<VerificationDetailsUpdatedEvent>,
                                             IEventHandler<TriggerCompletedEvent>
    {

        private readonly IApplicationEventObserver _applicationObserver;
        private readonly ITriggerEventObserver _triggerEventObserver;

        public ApplicationEventHandler(
            IApplicationEventObserver applicationObserver,
            ITriggerEventObserver triggerEventObserver,
            EventHandlingContext context) : base(context)
        {
            _applicationObserver = applicationObserver ?? throw new ArgumentNullException(nameof(applicationObserver));
            _triggerEventObserver = triggerEventObserver ?? throw new ArgumentNullException(nameof(triggerEventObserver));
        }

        public Task HandleAsync(TaskCompletedEvent @event) =>
            Handle(@event, args => args.Result switch
            {
                TaskResult.Passed => _applicationObserver.OnTaskPassed(args.Id),
                TaskResult.Failed => _applicationObserver.OnTaskFailed(args.Id),
                _ => throw new ArgumentOutOfRangeException(nameof(args.Result), args.Result, "Unexpected task result type.")
            });

        public Task HandleAsync(TaskIncompleteEvent @event) =>
            Handle(@event, args => _applicationObserver.OnTaskIncomplete(args.Id));

        public Task HandleAsync(CheckCompletedEvent @event) =>
            Handle(@event, args => args.Result switch
            {
                CheckResult.Passed => Task.CompletedTask,
                CheckResult.Failed => _applicationObserver.OnCheckFailed(args.UserId, args.CheckId),
                _ => throw new ArgumentOutOfRangeException(nameof(args.Result), args.Result, "Unexpected check result type.")
            });

        public Task HandleAsync(VerificationDetailsUpdatedEvent @event) =>
            Handle(@event, async args =>
            {
                var userId = args.UserId;

                var poiIssuingCountry = args.Changes.Find<string>(XPathes.PoiIssuingCountry);
                if (poiIssuingCountry is { NewValue: not null })
                    await _applicationObserver.OnPoiIssuingCountryChanged(userId, poiIssuingCountry.NewValue);

                //Triggers must be scheduled and fired before we try to change application state
                await _triggerEventObserver.OnDetailsChanged(userId);

                var changes = args.Changes.Select(change => change.PropertyName).ToArray();
                await _applicationObserver.OnDetailsChanged(userId, changes);
            });

        public Task HandleAsync(TriggerCompletedEvent @event) =>
            Handle(@event, args => _applicationObserver.OnTriggerCompleted(args.ApplicationId, args.TriggerId));
    }
}
