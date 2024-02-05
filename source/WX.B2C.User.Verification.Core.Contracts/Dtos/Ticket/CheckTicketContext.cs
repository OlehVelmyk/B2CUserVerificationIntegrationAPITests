using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket
{
    public class CheckTicketContext
    {
        public Guid UserId { get; set; }

        public Guid CheckId { get; set; }

        public CheckType Type { get; set; }
        
        public CheckState State { get; set; }

        public CheckResult Result { get; set; }

        public CheckProviderType Provider { get; set; }

        public string Decision { get; set; }
    }
}