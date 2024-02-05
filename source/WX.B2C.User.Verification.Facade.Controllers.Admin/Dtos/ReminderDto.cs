using System;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class ReminderDto
    {
        public Guid TargetId { get; set; }

        public Guid UserId { get; set; }

        public DateTime SentAt { get; set; }
    }
    
    public class ActiveReminderDto
    {
        public Guid TargetId { get; set; }

        public Guid UserId { get; set; }

        public DateTime SendAt { get; set; }
    }
}