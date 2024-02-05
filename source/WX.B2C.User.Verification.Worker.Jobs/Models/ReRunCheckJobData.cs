using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class ReRunCheckJobData : IJobData
    {
        public Guid UserId { get; set; }

        public CheckData[] ChecksToRerun { get; set; }
    }

    internal class CheckData
    {
        public Guid CheckId { get; set; }
        
        public Guid VariantId { get; set; }
        
        public CheckState State { get; set; }
        
        public Guid[] RelatedTasks { get; set; }

        public CheckProviderType Provider { get; set; }

        public CheckType Type { get; set; }
    }
}
