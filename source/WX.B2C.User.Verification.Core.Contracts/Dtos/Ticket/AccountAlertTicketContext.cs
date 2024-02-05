using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket
{
    public class AccountAlertTicketContext
    {
        public Guid UserId { get; set; }

        public RiskLevel RiskLevel { get; set; }

        public ApplicationState ApplicationState { get; set; }

        public DateTime LastApprovedDate { get; set; }

        public decimal Turnover { get; set; }
    }
}
