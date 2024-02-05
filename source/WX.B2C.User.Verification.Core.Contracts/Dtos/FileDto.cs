using System;
using Optional;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class FileDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; }

        public FileStatus Status { get; set; }

        public ExternalFileProviderType? Provider { get; set; }

        public string ExternalId { get; set; }

        public Option<uint> Crc32Checksum { get; set; }
    }
}
