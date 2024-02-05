using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class BlobFileDto
    {
        public Guid FileId { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; }
    }
}
