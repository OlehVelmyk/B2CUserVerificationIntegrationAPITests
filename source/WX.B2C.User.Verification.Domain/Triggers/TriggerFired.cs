using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Triggers
{
    public class TriggerFired : DomainEvent
    {
        private TriggerFired(Guid triggerId,
                                Guid variantId,
                                Guid userId,
                                Guid applicationId,
                                DateTime firingDate)
        {
            TriggerId = triggerId;
            VariantId = variantId;
            UserId = userId;
            ApplicationId = applicationId;
            FiringDate = firingDate;
        }

        public static TriggerFired Create(Trigger trigger) =>
            new(trigger.Id, trigger.VariantId, trigger.UserId, trigger.ApplicationId, trigger.FiringDate.Value);

        public Guid TriggerId { get; }

        public Guid VariantId { get; }

        public Guid UserId { get; }

        public Guid ApplicationId { get; }

        public DateTime FiringDate { get; }
    }
}