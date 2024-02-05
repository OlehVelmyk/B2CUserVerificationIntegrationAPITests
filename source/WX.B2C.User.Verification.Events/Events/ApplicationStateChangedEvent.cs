using System;
using WX.B2C.User.Verification.Events.EventArgs;

namespace WX.B2C.User.Verification.Events.Events
{
    public class ApplicationStateChangedEvent : BaseEvent<ApplicationStateChangedEventArgs>
    {
        public ApplicationStateChangedEvent(string key,
                                            ApplicationStateChangedEventArgs args,
                                            Guid causationId,
                                            Guid correlationId)
            :base(key, args, causationId, correlationId) { }
    }
}
