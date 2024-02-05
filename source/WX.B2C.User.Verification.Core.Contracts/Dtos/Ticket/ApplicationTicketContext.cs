using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket
{
    public class ApplicationTicketContext
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public ApplicationTicketReason Reason { get; set; }
    }
    
    public enum ApplicationTicketReason
    {
        ReadyForReview = 1,
    }
}