using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class ApplicationAutomatedEvent : BaseEvent<ApplicationAutomatedEventArgs>
    {
        public ApplicationAutomatedEvent(string key,
                                         ApplicationAutomatedEventArgs args,
                                         Guid causationId,
                                         Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}