using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.LexisNexis.Rdp.Client.Models;

namespace WX.B2C.User.Verification.LexisNexis.Processors
{
    internal static class RiskIndicatorExtensions
    {
        private static readonly string[] IgnoredRiskIndicators =
        {
            Constants.RiskIndicators.MissingPhoneRiskCode,
            Constants.RiskIndicators.MissingOrIncompleteSsnRiskCode
        };

        public static bool HasPartialRiskIndicators(this IEnumerable<RiskIndicator> riskIndicators)
        {
            if (riskIndicators == null)
                return false;

            return !riskIndicators.All(CanIgnoreRiskIndicator);
        }

        private static bool CanIgnoreRiskIndicator(RiskIndicator riskIndicator) =>
            IgnoredRiskIndicators.Contains(riskIndicator.RiskCode);
    }
}