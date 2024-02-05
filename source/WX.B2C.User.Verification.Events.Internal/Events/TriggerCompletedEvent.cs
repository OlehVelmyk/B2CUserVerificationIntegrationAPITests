using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TriggerCompletedEvent : BaseEvent<TriggerCompletedEventArgs>
    {
        public TriggerCompletedEvent(string key, TriggerCompletedEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }

}