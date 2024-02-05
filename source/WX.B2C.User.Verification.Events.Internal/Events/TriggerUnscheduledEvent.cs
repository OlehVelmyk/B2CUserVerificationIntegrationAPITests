using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TriggerUnscheduledEvent : BaseEvent<TriggerUnscheduledEventArgs>
    {
        public TriggerUnscheduledEvent(string key, TriggerUnscheduledEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }

}