using System;
using WX.B2C.User.Verification.Events.EventArgs;

namespace WX.B2C.User.Verification.Events.Events
{
    public class CollectionStepReadyForReviewEvent : BaseEvent<CollectionStepReadyForReviewEventArgs>
    {
        public CollectionStepReadyForReviewEvent(string key,
                                                 CollectionStepReadyForReviewEventArgs eventArgs,
                                                 Guid causationId,
                                                 Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId) { }
    }
}