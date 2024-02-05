using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class AccountAlertInfo : IJobData
    {
        public Guid UserId { get; set; }

        public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;

        public ApplicationState ApplicationState { get; set; }

        public DateTime LastApprovedDate { get; set; }

        public decimal Turnover { get; set; }
    }
}
