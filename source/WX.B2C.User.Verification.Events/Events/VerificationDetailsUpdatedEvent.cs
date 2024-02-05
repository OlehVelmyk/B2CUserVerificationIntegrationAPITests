using System;
using WX.B2C.User.Verification.Events.EventArgs;

namespace WX.B2C.User.Verification.Events.Events
{
    public class VerificationDetailsUpdatedEvent : BaseEvent<VerificationDetailsUpdatedEventArgs>
    {
        public VerificationDetailsUpdatedEvent(string key,
                                              VerificationDetailsUpdatedEventArgs args,
                                              Guid causationId,
                                              Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}
