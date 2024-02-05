using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class AccountAlertJobConfig
    {
        public AlertPeriod[] Periods { get; set; }

        public ApplicationState ApplicationState { get; set; }

        public string[] ExcludedCountries { get; set; }
    }

    internal class AlertPeriod
    {
        public int AccountAge { get; set; }

        public decimal OverallTurnover { get; set; }

        public RiskLevel RiskLevel { get; set; }
    }
}
