using System;
using WX.B2C.User.Verification.Events.EventArgs;

namespace WX.B2C.User.Verification.Events.Events
{
    public class CheckCompletedEvent : BaseEvent<CheckCompletedEventArgs>
    {
        public CheckCompletedEvent(string key, CheckCompletedEventArgs eventArgs, Guid causationId, Guid? correlationId = null) 
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}
