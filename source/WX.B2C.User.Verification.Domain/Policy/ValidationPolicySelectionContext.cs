namespace WX.B2C.User.Verification.Domain.Models
{
    public class ValidationPolicySelectionContext
    {
        private ValidationPolicySelectionContext() { }

        public string Country { get; private set; }

        public string Region { get; private set; }

        public static ValidationPolicySelectionContext Create(string country, string region)
        {
            return new ValidationPolicySelectionContext
            {
                Country = country,
                Region = region
            };
        }
    }
}