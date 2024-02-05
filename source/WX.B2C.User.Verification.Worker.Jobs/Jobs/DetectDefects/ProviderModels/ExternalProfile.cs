using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels
{
    internal class ExternalProfile
    {
        public Guid UserId { get; set; }

        public ExternalProviderType Provider { get; set; }

        public string ExternalId { get; set; }
    }
}