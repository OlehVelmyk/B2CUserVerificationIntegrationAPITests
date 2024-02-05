using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Triggers
{
    public class TriggerUnscheduled : DomainEvent
    {
        private TriggerUnscheduled(Guid triggerId,
                                 Guid variantId,
                                 Guid userId,
                                 Guid applicationId,
                                 DateTime unscheduleDate)
        {
            TriggerId = triggerId;
            VariantId = variantId;
            UserId = userId;
            ApplicationId = applicationId;
            UnscheduleDate = unscheduleDate;
        }

        public static TriggerUnscheduled Create(Trigger trigger) =>
            new(trigger.Id, trigger.VariantId, trigger.UserId, trigger.ApplicationId, trigger.UnscheduleDate.Value);

        public Guid TriggerId { get; }

        public Guid VariantId { get; }

        public Guid UserId { get; }

        public Guid ApplicationId { get; }

        public DateTime UnscheduleDate { get; }
    }
}