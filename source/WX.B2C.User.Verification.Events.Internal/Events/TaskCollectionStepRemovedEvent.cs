using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TaskCollectionStepRemovedEvent : BaseEvent<TaskCollectionStepRemovedEventArgs>
    {
        public TaskCollectionStepRemovedEvent(string key,
                                              TaskCollectionStepRemovedEventArgs args,
                                              Guid causationId,
                                              Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}