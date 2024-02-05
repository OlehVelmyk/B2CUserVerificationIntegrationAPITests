using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Facade.EventHandlers.Mappers;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class JobEventHandler : BaseEventHandler,
                                     IEventHandler<TriggerUnscheduledEvent>,
                                     IEventHandler<TriggerScheduledEvent>,
                                     IEventHandler<TriggerFiredEvent>
    {
        private readonly IJobService _jobService;
        private readonly IJobRequestMapper _jobRequestMapper;
        private readonly ITriggerVariantStorage _triggerVariantStorage;

        public JobEventHandler(
            IJobService jobService,
            IJobRequestMapper jobRequestMapper,
            ITriggerVariantStorage triggerVariantStorage,
            EventHandlingContext context) : base(context)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _jobRequestMapper = jobRequestMapper ?? throw new ArgumentNullException(nameof(jobRequestMapper));
            _triggerVariantStorage = triggerVariantStorage ?? throw new ArgumentNullException(nameof(triggerVariantStorage));
        }

        public Task HandleAsync(TriggerUnscheduledEvent @event) =>
            Handle(@event, args =>
            {
                var request = _jobRequestMapper.ToStopJobRequest(args.TriggerId, args.UserId);
                return _jobService.UnscheduleAsync(request);
            });

        public Task HandleAsync(TriggerScheduledEvent @event) =>
            Handle(@event, async args =>
            {
                var triggerVariant = await _triggerVariantStorage.GetAsync(args.VariantId);
                if (triggerVariant.IsScheduled)
                {
                    var request = _jobRequestMapper.ToScheduleJobRequest(triggerVariant, args.TriggerId, args.UserId);
                    await _jobService.ScheduleAsync(request);
                }
            });

        public Task HandleAsync(TriggerFiredEvent @event) =>
            Handle(@event, async args =>
            {
                var triggerVariant = await _triggerVariantStorage.GetAsync(args.VariantId);
                if (triggerVariant.IsScheduled)
                {
                    var request = _jobRequestMapper.ToStopJobRequest(args.TriggerId, args.UserId);
                    await _jobService.UnscheduleAsync(request);
                }
            });
    }
}