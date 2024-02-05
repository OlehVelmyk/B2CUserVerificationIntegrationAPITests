using System;

namespace WX.B2C.User.Verification.Commands
{
    public class CreateCollectionStepCommand : VerificationCommand
    {
        public CreateCollectionStepCommand(Guid userId, 
                                           Guid stepId, 
                                           string xPath, 
                                           bool isRequired, 
                                           bool isReviewNeeded, 
                                           bool isSubmitted, 
                                           string reason)
        {
            CommandId = stepId;
            CommandChainId = userId;
            UserId = userId;
            XPath = xPath;
            IsRequired = isRequired;
            IsReviewNeeded = isReviewNeeded;
            IsSubmitted = isSubmitted;
            Reason = reason;
        }

        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public bool IsSubmitted { get; set; }

        public string Reason { get; set; }
    }
}
