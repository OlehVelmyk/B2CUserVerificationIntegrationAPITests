using System.Collections.Generic;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class Policies
    {
        public static readonly IEnumerable<VerificationPolicy> OnboardingTriggers = new[]
        {
            VerificationPolicy.Gb,
            VerificationPolicy.Eaa,
            VerificationPolicy.Us
        };

        public static readonly IEnumerable<VerificationPolicy> MonitoringTriggers = new[]
        {
            VerificationPolicy.Gb,
            VerificationPolicy.Apac,
            VerificationPolicy.Eaa,
            VerificationPolicy.Row,
            VerificationPolicy.Us,
            VerificationPolicy.Ph
        };
    }
}
