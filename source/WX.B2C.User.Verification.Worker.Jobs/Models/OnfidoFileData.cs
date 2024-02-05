using System;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class OnfidoFileData : IJobData
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; }

        public ExternalFileProviderType? Provider { get; set; }

        public string ExternalId { get; set; }
    }
}