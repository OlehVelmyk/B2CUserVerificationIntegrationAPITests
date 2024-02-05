using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class CheckErrorOccuredEvent : BaseEvent<CheckErrorOccuredEventArgs>
    {
        public CheckErrorOccuredEvent(string key, CheckErrorOccuredEventArgs eventArgs, Guid causationId, Guid? correlationId = null) 
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}
