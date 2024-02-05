namespace WX.B2C.User.Verification.Domain.Models
{
    public class MonitoringPolicySelectionContext
    {
        private MonitoringPolicySelectionContext() { }

        public string Country { get; private set; }

        public string Region { get; private set; }

        public static MonitoringPolicySelectionContext Create(string country, string region)
        {
            return new MonitoringPolicySelectionContext
            {
                Country = country,
                Region = region
            };
        }

        public override string ToString() => $"{nameof(Country)}: {Country}";
    }
}