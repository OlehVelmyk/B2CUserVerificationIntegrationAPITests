using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class CheckStartedEvent : BaseEvent<CheckStartedEventArgs>
    {
        public CheckStartedEvent(string key, CheckStartedEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}