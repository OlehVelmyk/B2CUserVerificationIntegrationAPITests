using System;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class AccountAlertCreatedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public RiskLevel RiskLevel { get; set; }

        public ApplicationState ApplicationState { get; set; }

        public DateTime LastApprovedDate { get; set; }

        public decimal Turnover { get; set; }

        public static AccountAlertCreatedEventArgs Create(Guid userId,
                                                          RiskLevel riskLevel,
                                                          ApplicationState applicationState,
                                                          DateTime lastApprovedDate,
                                                          decimal turnover) =>
            new AccountAlertCreatedEventArgs
            {
                UserId = userId,
                RiskLevel = riskLevel,
                ApplicationState = applicationState,
                LastApprovedDate = lastApprovedDate,
                Turnover = turnover
            };
    }
}
