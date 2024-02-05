using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket
{
    public class TicketContext
    {
        public Guid UserId { get; set; }

        public string Reason { get; set; }
    }
}