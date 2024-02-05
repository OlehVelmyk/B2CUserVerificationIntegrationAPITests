using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TaskCreatedEvent : BaseEvent<TaskCreatedEventArgs>
    {
        public TaskCreatedEvent(string key,
                                TaskCreatedEventArgs args,
                                Guid causationId,
                                Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}