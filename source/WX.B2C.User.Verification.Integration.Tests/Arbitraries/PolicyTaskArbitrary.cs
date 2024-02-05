using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class PolicyTaskArbitrary : Arbitrary<PolicyTaskSpecimen>
    {
        public static Arbitrary<PolicyTaskSpecimen> Create() => new PolicyTaskArbitrary();

        public override Gen<PolicyTaskSpecimen> Generator =>
            PolicyTaskGenerator.Any();
    }

    internal class UsPolicyTaskArbitrary : Arbitrary<UsPolicyTaskSpecimen>
    {
        public static Arbitrary<UsPolicyTaskSpecimen> Create() => new UsPolicyTaskArbitrary();

        public override Gen<UsPolicyTaskSpecimen> Generator =>
            PolicyTaskGenerator.Us();
    }

    internal class UsPolicyTasksArbitrary : Arbitrary<PolicyTasksSpecimen<UsPolicyTaskSpecimen>>
    {
        public static Arbitrary<PolicyTasksSpecimen<UsPolicyTaskSpecimen>> Create() => new UsPolicyTasksArbitrary();

        public override Gen<PolicyTasksSpecimen<UsPolicyTaskSpecimen>> Generator =>
            PolicyTaskGenerator.UsAll();
    }

    internal class GbPolicyTaskArbitrary : Arbitrary<GbPolicyTaskSpecimen>
    {
        public static Arbitrary<GbPolicyTaskSpecimen> Create() => new GbPolicyTaskArbitrary();

        public override Gen<GbPolicyTaskSpecimen> Generator =>
            PolicyTaskGenerator.Gb();
    }

    internal class GbPoFPolicyTaskArbitrary : Arbitrary<GbPoFPolicyTaskSpecimen>
    {
        public static Arbitrary<GbPoFPolicyTaskSpecimen> Create() => new GbPoFPolicyTaskArbitrary();

        public override Gen<GbPoFPolicyTaskSpecimen> Generator =>
            PolicyTaskGenerator.GbPoF();
    }

    internal class IdentityPolicyTaskArbitrary : Arbitrary<PolicyTasksSpecimen<IdentityPolicyTaskSpecimen>>
    {
        public static Arbitrary<PolicyTasksSpecimen<IdentityPolicyTaskSpecimen>> Create() => new IdentityPolicyTaskArbitrary();

        public override Gen<PolicyTasksSpecimen<IdentityPolicyTaskSpecimen>> Generator =>
            PolicyTaskGenerator.IdentityAll();
    }
}
