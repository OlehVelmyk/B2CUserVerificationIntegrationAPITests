using System;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    public class TaxResidenceData : IJobData
    {
        public Guid ProfileInformationId { get; set; }

        public VerificationStopReason VerificationStopReason { get; set; }

        public string TaxResidencies { get; set; }
    }
}