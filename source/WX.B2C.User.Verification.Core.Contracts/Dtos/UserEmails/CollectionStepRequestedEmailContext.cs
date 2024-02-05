using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.UserEmails
{
    public class CollectionStepRequestedEmailContext
    {
        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public string Reason { get; set; }
    }
}