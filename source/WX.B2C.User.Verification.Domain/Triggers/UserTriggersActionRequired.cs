using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Triggers
{
    public class UserTriggersActionRequired : DomainEvent
    {
        private UserTriggersActionRequired(Guid userId, HashSet<string> triggerActions, Guid triggerPolicyId)
        {
            if (triggerActions == null)
                throw new ArgumentNullException(nameof(triggerActions));
            if (triggerActions.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(triggerActions));

            UserId = userId;
            TriggerActions = triggerActions;
            TriggerPolicyId = triggerPolicyId;
        }

        public static UserTriggersActionRequired Create(Guid userId, HashSet<string> triggerAction, Guid triggerPolicyId) =>
            new(userId, triggerAction, triggerPolicyId);

        public Guid UserId { get; }

        public HashSet<string> TriggerActions { get; }

        public Guid TriggerPolicyId { get; }
    }
}