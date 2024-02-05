using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class ScheduledTriggerJobFinishedEvent : BaseEvent<ScheduledTriggerJobFinishedArgs>
    {
        public ScheduledTriggerJobFinishedEvent(string key, ScheduledTriggerJobFinishedArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }

}