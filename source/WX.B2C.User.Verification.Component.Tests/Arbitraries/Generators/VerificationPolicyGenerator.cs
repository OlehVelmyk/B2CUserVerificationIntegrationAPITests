using System.Collections.Generic;
using System.Linq;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class VerificationPolicyGenerators
    {
        public static Gen<VerificationPolicy> Supported =>
            Arb.Generate<VerificationPolicy>()
               .Where(policy => policy is not VerificationPolicy.Unsupported);

        public static Gen<VerificationPolicy> WithOnboardingTriggers =>
            Gen.Elements(Policies.OnboardingTriggers);

        public static Gen<VerificationPolicy> WithoutOnboardingTriggers =>
            Supported.Where(policy => !policy.In(Policies.OnboardingTriggers));

        public static Gen<VerificationPolicy> WithMonitoringTriggers =>
            Gen.Elements(Policies.MonitoringTriggers);

        public static Gen<VerificationPolicy> WithoutMonitoringTriggers =>
            Supported.Where(policy => !policy.In(Policies.MonitoringTriggers));
    }
}
