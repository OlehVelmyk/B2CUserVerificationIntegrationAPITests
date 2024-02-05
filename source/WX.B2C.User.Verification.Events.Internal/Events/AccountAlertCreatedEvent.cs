using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class AccountAlertCreatedEvent : BaseEvent<AccountAlertCreatedEventArgs>
    {
        public AccountAlertCreatedEvent(string key, AccountAlertCreatedEventArgs eventArgs, Guid causationId, Guid? correlationId = null) 
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }
}
