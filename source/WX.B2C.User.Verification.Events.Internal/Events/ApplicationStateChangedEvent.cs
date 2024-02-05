using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class ApplicationStateChangedEvent : BaseEvent<ApplicationStateChangedEventArgs>
    {
        public ApplicationStateChangedEvent(string key,
                                            ApplicationStateChangedEventArgs args,
                                            Guid causationId,
                                            Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}