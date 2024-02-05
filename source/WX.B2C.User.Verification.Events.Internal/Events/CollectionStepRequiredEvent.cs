using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class CollectionStepRequiredEvent : BaseEvent<CollectionStepRequiredEventArgs>
    {
        public CollectionStepRequiredEvent(string key,
                                           CollectionStepRequiredEventArgs args,
                                           Guid causationId,
                                           Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}