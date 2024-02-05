using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class File : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; }

        public uint? Crc32Checksum { get; set; }

        public FileStatus Status { get; set; }

        public string ExternalId { get; set; }

        public ExternalFileProviderType? Provider { get; set; }
    }
}
