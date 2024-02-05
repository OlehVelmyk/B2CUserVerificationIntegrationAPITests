using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class ChecksCreatedEvent : BaseEvent<ChecksCreatedEventArgs>
    {
        public ChecksCreatedEvent(string key, ChecksCreatedEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}
