namespace WX.B2C.User.Verification.Domain.Models
{
    public class VerificationPolicySelectionContext
    {
        private VerificationPolicySelectionContext() { }

        public string Region { get; private set; }

        public string Country { get; private set; }

        public string State { get; private set; }

        public static VerificationPolicySelectionContext Create(string country, string region, string state = null)
        {
            return new VerificationPolicySelectionContext
            {
                Country = country,
                Region = region,
                State = state
            };
        }
    }
}