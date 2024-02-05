using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class UserTriggersActionRequiredEvent : BaseEvent<UserTriggersActionRequiredEventArgs>
    {
        public UserTriggersActionRequiredEvent(string key,
                                               UserTriggersActionRequiredEventArgs eventEventArgs,
                                               Guid causationId,
                                               Guid? correlationId = null)
            : base(key, eventEventArgs, causationId, correlationId) { }
    }
}