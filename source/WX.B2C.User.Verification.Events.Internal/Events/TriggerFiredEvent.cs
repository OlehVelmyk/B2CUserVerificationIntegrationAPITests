using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class TriggerFiredEvent : BaseEvent<TriggerFiredEventArgs>
    {
        public TriggerFiredEvent(string key, TriggerFiredEventArgs eventArgs, Guid causationId, Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId)
        {
        }
    }

}