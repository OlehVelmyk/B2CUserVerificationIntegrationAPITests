using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TaskCollectionStepAddedEvent : BaseEvent<TaskCollectionStepAddedEventArgs>
    {
        public TaskCollectionStepAddedEvent(string key,
                                            TaskCollectionStepAddedEventArgs args,
                                            Guid causationId,
                                            Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}