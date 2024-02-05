using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TaskIncompleteEvent : BaseEvent<TaskIncompleteEventArgs>
    {
        public TaskIncompleteEvent(string key, TaskIncompleteEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }

}