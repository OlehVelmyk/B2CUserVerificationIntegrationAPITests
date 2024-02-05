using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class UserReminderJobFinishedEvent : BaseEvent<UserReminderJobFinishedEventArgs>
    {
        public UserReminderJobFinishedEvent(string key, 
                                            UserReminderJobFinishedEventArgs eventArgs, 
                                            Guid causationId, 
                                            Guid? correlationId = null) 
            : base(key, eventArgs, causationId, correlationId) { }
    }
}
