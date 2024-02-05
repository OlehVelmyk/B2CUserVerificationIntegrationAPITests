using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class ExternalProfileCreatedEvent : BaseEvent<ExternalProfileCreatedEventArgs>
    {
        public ExternalProfileCreatedEvent(string key,
                                           ExternalProfileCreatedEventArgs eventArgs,
                                           Guid causationId,
                                           Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId) { }
    }
}