using System;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class UserReminderDto
    {
        public Guid UserId { get; set; }
        
        public Guid TargetId { get; set; }

        public DateTime SentAt { get; set; }
    }
}
