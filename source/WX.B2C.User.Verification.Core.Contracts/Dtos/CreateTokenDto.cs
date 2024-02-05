using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CreateTokenDto
    {
        public TokenProvider Provider { get; set; }

        public TokenType Type { get; set; }

        public string ApplicationId { get; set; }
    }
}