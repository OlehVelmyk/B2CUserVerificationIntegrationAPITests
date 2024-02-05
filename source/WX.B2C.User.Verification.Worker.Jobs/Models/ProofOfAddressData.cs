using System;
using WX.B2C.User.Verification.Worker.Jobs.Models.Verification;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class ProofOfAddressData : IJobData
    {
        public Guid UserId { get; set; }

        public ProofOfAddressCheckStatus? Status { get; set; }

        public bool? IsCountryMatchedByIp { get; set; }
    }
}