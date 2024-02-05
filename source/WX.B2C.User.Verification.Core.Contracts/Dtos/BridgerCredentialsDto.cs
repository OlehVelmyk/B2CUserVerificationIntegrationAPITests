using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class BridgerCredentialsDto
    {
        public string UserId { get; set; }

        public string EncryptedPassword { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
