using System;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class ProofOfFundsChecksData : IJobData
    {
        public Guid UserId { get; set; }

        public bool HasOngoing { get; set; }
    }
}