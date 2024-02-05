using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Triggers
{
    public class Trigger : AggregateRoot
    {
        public Trigger(Guid id,
                       Guid variantId,
                       Guid userId,
                       Guid applicationId,
                       TriggerState state,
                       DateTime scheduleDate,
                       DateTime? firingDate = null,
                       DateTime? unscheduleDate = null,
                       IReadOnlyDictionary<string, object> context = null) : base(id)
        {
            VariantId = variantId;
            UserId = userId;
            ApplicationId = applicationId;
            State = state;

            ScheduleDate = scheduleDate;
            UnscheduleDate = unscheduleDate;
            FiringDate = firingDate;

            Context = context;
        }

        public Guid VariantId { get; }

        public Guid UserId { get; }

        public Guid ApplicationId { get; }

        public DateTime ScheduleDate { get; }

        public TriggerState State { private set; get; }

        public DateTime? FiringDate { get; private set; }

        public DateTime? UnscheduleDate { get; private set; }

        public IReadOnlyDictionary<string, object> Context { get; private set; }

        public static Trigger Create(Guid variantId,
                                     Guid userId,
                                     Guid applicationId)
        {
            var trigger = new Trigger(Guid.NewGuid(), variantId, userId, applicationId, TriggerState.Scheduled, DateTime.UtcNow);
            trigger.Apply(TriggerScheduled.Create(trigger));
            return trigger;
        }

        public void Fire(Dictionary<string, object> executionContext)
        {
            if (State == TriggerState.Fired)
                return;

            if (State != TriggerState.Scheduled)
                throw new InvalidOperationException($"Prohibited to fire not schedule trigger. Trigger id {Id}.");

            State = TriggerState.Fired;
            FiringDate = DateTime.UtcNow;
            Context = executionContext;

            Apply(TriggerFired.Create(this));
        }

        public void Schedule()
        {
            if (State == TriggerState.Scheduled)
                return;

            if (State != TriggerState.Unscheduled)
                throw new InvalidOperationException($"Can be rescheduled only unscheduled trigger. Current state of trigger {Id} is {State}");

            State = TriggerState.Scheduled;
            UnscheduleDate = null;

            Apply(TriggerScheduled.Create(this));
        }

        public void Unschedule()
        {
            if (State == TriggerState.Unscheduled)
                return;

            if (State != TriggerState.Scheduled)
                throw new InvalidOperationException($"Can be cancelled only scheduled trigger. Current state of trigger {Id} is {State}");

            State = TriggerState.Unscheduled;
            UnscheduleDate = DateTime.UtcNow;

            Apply(TriggerUnscheduled.Create(this));
        }

        public void Complete()
        {
            if (State != TriggerState.Fired)
                throw new InvalidOperationException($"Can be completed only fired trigger. Current state of trigger {Id} is {State}");

            State = TriggerState.Completed;
            
            Apply(TriggerCompleted.Create(this));
        }
    }
}