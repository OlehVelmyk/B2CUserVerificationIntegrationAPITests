using System;
using WX.B2C.User.Verification.Events.EventArgs;

namespace WX.B2C.User.Verification.Events.Events
{
    public class ApplicationRegisteredEvent : BaseEvent<VerificationApplicationRegisteredEventArgs>
    {
        public ApplicationRegisteredEvent(string key,
                                          VerificationApplicationRegisteredEventArgs args,
                                          Guid causationId,
                                          Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}