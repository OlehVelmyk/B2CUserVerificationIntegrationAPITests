using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    public class AuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}