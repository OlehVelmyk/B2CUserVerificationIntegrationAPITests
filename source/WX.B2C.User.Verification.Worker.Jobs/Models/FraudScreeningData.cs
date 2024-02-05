using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class FraudScreeningData
    {
        public Guid UserId { get; set; }

        public int? ComprehensiveIndex { get; set; }

        public string RiskCodes { get; set; }
    }
}
