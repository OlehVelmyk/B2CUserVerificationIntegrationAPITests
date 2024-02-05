using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Triggers
{
    public class TriggerScheduled : DomainEvent
    {
        private TriggerScheduled(Guid triggerId,
                                Guid userId,
                                Guid applicationId,
                                Guid variantId,
                                DateTime scheduleDate)
        {
            TriggerId = triggerId;
            UserId = userId;
            ApplicationId = applicationId;
            ScheduleDate = scheduleDate;
            VariantId = variantId;
        }

        public static TriggerScheduled Create(Trigger trigger) =>
            new(trigger.Id, trigger.UserId, trigger.ApplicationId, trigger.VariantId, trigger.ScheduleDate);

        public Guid TriggerId { get; }

        public Guid UserId { get; }

        public Guid ApplicationId { get; }

        public Guid VariantId { get; }

        public DateTime ScheduleDate { get; }
    }
}