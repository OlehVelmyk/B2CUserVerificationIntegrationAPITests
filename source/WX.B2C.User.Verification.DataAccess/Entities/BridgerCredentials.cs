using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class BridgerCredentials : AuditableEntity
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string EncryptedPassword { get; set; }
    }
}
