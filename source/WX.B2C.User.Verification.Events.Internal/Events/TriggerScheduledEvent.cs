using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TriggerScheduledEvent : BaseEvent<TriggerScheduledEventArgs>
    {
        public TriggerScheduledEvent(string key, TriggerScheduledEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }

}