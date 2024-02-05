using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class TriggerScheduledEventArgs : System.EventArgs
    {
        public Guid TriggerId { get; set; }
        
        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public DateTime ScheduleDate { get; set; }
    }
}