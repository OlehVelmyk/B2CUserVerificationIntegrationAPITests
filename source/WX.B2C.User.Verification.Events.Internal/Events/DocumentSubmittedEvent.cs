using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class DocumentSubmittedEvent : BaseEvent<DocumentSubmittedEventArgs>
    {
        public DocumentSubmittedEvent(string key,
                                       DocumentSubmittedEventArgs args,
                                       Guid causationId,
                                       Guid correlationId)
            :base(key,args,causationId,correlationId) { }
    }
}