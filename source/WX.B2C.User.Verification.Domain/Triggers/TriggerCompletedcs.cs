using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Triggers
{
    public class TriggerCompleted : DomainEvent
    {
        private TriggerCompleted(Guid triggerId,
                                 Guid variantId,
                                 Guid userId,
                                 Guid applicationId)
        {
            TriggerId = triggerId;
            VariantId = variantId;
            UserId = userId;
            ApplicationId = applicationId;
        }

        public static TriggerCompleted Create(Trigger trigger) =>
            new(trigger.Id, trigger.VariantId, trigger.UserId, trigger.ApplicationId);

        public Guid TriggerId { get; }

        public Guid VariantId { get; }

        public Guid UserId { get; }

        public Guid ApplicationId { get; }
    }
}