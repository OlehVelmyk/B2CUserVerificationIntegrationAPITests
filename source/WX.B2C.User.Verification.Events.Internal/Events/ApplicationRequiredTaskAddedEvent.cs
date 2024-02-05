using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class ApplicationRequiredTaskAddedEvent : BaseEvent<ApplicationRequiredTaskAddedEventArgs>
    {
        public ApplicationRequiredTaskAddedEvent(string key,
                                                 ApplicationRequiredTaskAddedEventArgs args,
                                                 Guid causationId,
                                                 Guid correlationId)
            :base(key,args,causationId,correlationId) { }
    }
}