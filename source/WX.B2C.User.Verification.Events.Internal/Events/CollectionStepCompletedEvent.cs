using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class CollectionStepCompletedEvent : BaseEvent<CollectionStepCompletedEventArgs>
    {
        public CollectionStepCompletedEvent(string key, CollectionStepCompletedEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}
