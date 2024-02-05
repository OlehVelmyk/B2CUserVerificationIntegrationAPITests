using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class VerificationDetailsUpdatedEvent : BaseEvent<VerificationDetailsUpdatedEventArgs>
    {
        public VerificationDetailsUpdatedEvent(string key,
                                               VerificationDetailsUpdatedEventArgs args,
                                               Guid causationId,
                                               Guid correlationId)
            :base(key,args,causationId,correlationId) { }
    }
}