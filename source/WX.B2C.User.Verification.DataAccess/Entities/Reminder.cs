using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class Reminder
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }

        public Guid TargetId { get; set; }
        
        public DateTime SentAt { get; set; }
    }
}