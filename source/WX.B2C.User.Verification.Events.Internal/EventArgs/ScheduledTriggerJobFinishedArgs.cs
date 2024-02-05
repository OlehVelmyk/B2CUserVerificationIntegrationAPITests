using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class ScheduledTriggerJobFinishedArgs : System.EventArgs
    {
        public Guid TriggerId { get; set; }

        public Guid UserId { get; set; }

        public DateTime FinishingDate { get; set; }
    }
}