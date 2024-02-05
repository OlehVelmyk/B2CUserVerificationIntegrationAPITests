namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Policy
{
    public class PolicyCollectionStep
    {
        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }
    }
}