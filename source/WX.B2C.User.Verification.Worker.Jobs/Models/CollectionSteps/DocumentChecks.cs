using System;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models.CollectionSteps
{
    internal class DocumentChecks : IJobData
    {
        public Guid UserId { get; set; }
        
        public VerificationStatus VerificationStatus { get; set; }

        public VerificationStopReason VerificationStopReason { get; set; }

        public ProofOfAddressCheckStatus? PoAStatus { get; set; }

        public ProofOfFundsCheckStatus? PoFStatus { get; set; }
    }
}