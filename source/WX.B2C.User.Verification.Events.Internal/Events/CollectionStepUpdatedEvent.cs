using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class CollectionStepUpdatedEvent : BaseEvent<CollectionStepUpdatedEventArgs>
    {
        public CollectionStepUpdatedEvent(string key,
                                          CollectionStepUpdatedEventArgs eventArgs,
                                          Guid causationId,
                                          Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId) { }
    }
}
