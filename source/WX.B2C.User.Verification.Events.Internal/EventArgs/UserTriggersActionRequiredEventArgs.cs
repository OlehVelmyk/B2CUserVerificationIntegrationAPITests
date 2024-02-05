using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class UserTriggersActionRequiredEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public string[] Actions { get; set; }

        public Guid TriggerPolicyId { get; set; }
    }
}