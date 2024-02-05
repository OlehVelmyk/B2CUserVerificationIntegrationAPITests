using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class CollectionStepReadyForReviewEvent : BaseEvent<CollectionStepReadyForReviewEventArgs>
    {
        public CollectionStepReadyForReviewEvent(string key, CollectionStepReadyForReviewEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId) { }
    }
}
