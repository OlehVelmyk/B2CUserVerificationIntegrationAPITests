using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class CheckPerformedEvent : BaseEvent<CheckPerformedEventArgs>
    {
        public CheckPerformedEvent(string key, CheckPerformedEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}
