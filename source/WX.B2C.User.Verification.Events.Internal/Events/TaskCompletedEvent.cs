using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TaskCompletedEvent : BaseEvent<TaskCompletedEventArgs>
    {
        public TaskCompletedEvent(string key, TaskCompletedEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}
