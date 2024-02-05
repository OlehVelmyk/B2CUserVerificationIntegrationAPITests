using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket
{
    public class ReviewCollectionStepTicketContext
    {
        public Guid UserId { get; set; }

        public string XPath { get; set; }
    }
}