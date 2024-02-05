using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket
{
    public class ExceededCheckResubmitAttemptsTicketContext
    {
        public Guid UserId { get; set; }

        public Guid CheckId { get; set; }

        public CheckType Type { get; set; }

        public CheckProviderType Provider { get; set; }
    }
}
