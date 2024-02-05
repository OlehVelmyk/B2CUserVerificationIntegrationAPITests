using System;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class UserInfoArbitrary : Arbitrary<UserInfo>
    {
        public static Arbitrary<UserInfo> Create() => new UserInfoArbitrary();

        public override Gen<UserInfo> Generator =>
            from seed in Arb.Generate<Seed>()
            let faker = FakerFactory.Create(seed)
            from userId in Gen.Fresh(Guid.NewGuid)
            from policy in VerificationPolicyGenerators.Supported
            from nationality in CountryCodeGenerators.Supported(policy)
            from address in AddressGenerators.Address(seed, nationality)
            from ipAddress in Arb.Generate<System.Net.IPAddress>()
            select new UserInfo
            {
                UserId = userId,
                Email = faker.Internet.Email(),
                DateOfBirth = faker.Date.Past(100, DateTime.UtcNow.AddDays(-1)),
                Policy = policy,
                Nationality = nationality,
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                Address = address,
                IpAddress = ipAddress.ToString()
            };
    }

    internal class EmptyUserInfoArbitrary : Arbitrary<EmptyUserInfo>
    {
        public static Arbitrary<EmptyUserInfo> Create() => new EmptyUserInfoArbitrary();

        public override Gen<EmptyUserInfo> Generator =>
            from seed in Arb.Generate<Seed>()
            from userId in Gen.Fresh(Guid.NewGuid)
            from policy in VerificationPolicyGenerators.Supported
            from nationality in CountryCodeGenerators.Supported(policy)
            from address in AddressGenerators.Address(seed, nationality)
            from ipAddress in Arb.Generate<System.Net.IPAddress>()
            select new EmptyUserInfo()
            {
                UserId = userId,
                Address = address,
                IpAddress = ipAddress.ToString()
            };
    }

    internal class UnsupportedStateUsUserInfoArbitrary : Arbitrary<UnsupportedStateUsUserInfo>
    {
        public static Arbitrary<UnsupportedStateUsUserInfo> Create() => new UnsupportedStateUsUserInfoArbitrary();

        public override Gen<UnsupportedStateUsUserInfo> Generator =>
            Arb.Generate<UsUserInfo>()
               .Override(VerificationPolicy.Unsupported, ui => ui.Policy)
               .Override(StateGenerators.UnsupportedUs(), ui => ui.Address.State)
               .Select(ui => new UnsupportedStateUsUserInfo(ui));
    }

    internal class UnsupportedUserInfoArbitrary : Arbitrary<UnsupportedUserInfo>
    {
        public static Arbitrary<UnsupportedUserInfo> Create() => new UnsupportedUserInfoArbitrary();

        public override Gen<UnsupportedUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Unsupported)
               .Select(ui => new UnsupportedUserInfo(ui));
    }

    // TODO: Override ip inside 'OverridePolicy' or remove overriding at all because it is redundant now
    internal class GbUserInfoArbitrary : Arbitrary<GbUserInfo>
    {
        public static Arbitrary<GbUserInfo> Create() => new GbUserInfoArbitrary();

        public override Gen<GbUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Gb)
               .Override(IpAddresses.GbIpAddress, ui => ui.IpAddress)
               .Select(ui => new GbUserInfo(ui));
    }
    
    internal class NotGbUserInfoArbitrary : Arbitrary<NotGbUserInfo>
    {
        private readonly Gen<VerificationPolicy> _policyGenerator =
            from policy in VerificationPolicyGenerators.Supported
            where policy is not VerificationPolicy.Gb
            select policy;

        public static Arbitrary<NotGbUserInfo> Create() => new NotGbUserInfoArbitrary();

        public override Gen<NotGbUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(_policyGenerator)
               .Select(ui => new NotGbUserInfo(ui));
    }

    internal class UsUserInfoArbitrary : Arbitrary<UsUserInfo>
    {
        public static Arbitrary<UsUserInfo> Create() => new UsUserInfoArbitrary();

        public override Gen<UsUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Us)
               .Override(IpAddresses.UsIpAddress, ui => ui.IpAddress)
               .Select(ui => new UsUserInfo(ui));
    }

    internal class NotUsUserInfoArbitrary : Arbitrary<NotUsUserInfo>
    {
        private readonly Gen<VerificationPolicy> _policyGenerator =
            from policy in VerificationPolicyGenerators.Supported
            where policy is not VerificationPolicy.Us
            select policy;

        public static Arbitrary<NotUsUserInfo> Create() => new NotUsUserInfoArbitrary();

        public override Gen<NotUsUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(_policyGenerator)
               .Select(ui => new NotUsUserInfo(ui));
    }

    internal class EeaUserInfoArbitrary : Arbitrary<EeaUserInfo>
    {
        public static Arbitrary<EeaUserInfo> Create() => new EeaUserInfoArbitrary();

        public override Gen<EeaUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Eaa)
               .Override(IpAddresses.FrIpAddress, ui => ui.IpAddress)
               .Select(ui => new EeaUserInfo(ui));
    }
    
    internal class ApacUserInfoArbitrary : Arbitrary<ApacUserInfo>
    {
        public static Arbitrary<ApacUserInfo> Create() => new ApacUserInfoArbitrary();

        public override Gen<ApacUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Apac)
               .Override(IpAddresses.AuIpAddress, ui => ui.IpAddress)
               .Select(ui => new ApacUserInfo(ui));
    }
    
    internal class RoWUserInfoArbitrary : Arbitrary<RoWUserInfo>
    {
        public static Arbitrary<RoWUserInfo> Create() => new RoWUserInfoArbitrary();

        public override Gen<RoWUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Row)
               .Override(IpAddresses.UaIpAddress, ui => ui.IpAddress)
               .Select(ui => new RoWUserInfo(ui));
    }

    internal class GlobalUserInfoArbitrary : Arbitrary<GlobalUserInfo>
    {
        public static Arbitrary<GlobalUserInfo> Create() => new GlobalUserInfoArbitrary();

        public override Gen<GlobalUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Global)
               .Override(IpAddresses.DzIpAddress, ui => ui.IpAddress)
               .Select(ui => new GlobalUserInfo(ui));
    }

    internal class NotGlobalUserInfoArbitrary : Arbitrary<NotGlobalUserInfo>
    {
        private readonly Gen<VerificationPolicy> _policyGenerator =
            from policy in VerificationPolicyGenerators.Supported
            where policy is not VerificationPolicy.Global
            select policy;

        public static Arbitrary<NotGlobalUserInfo> Create() => new NotGlobalUserInfoArbitrary();

        public override Gen<NotGlobalUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(_policyGenerator)
               .Select(ui => new NotGlobalUserInfo(ui));
    }

    internal class RuUserInfoArbitrary : Arbitrary<RuUserInfo>
    {
        public static Arbitrary<RuUserInfo> Create() => new RuUserInfoArbitrary();

        public override Gen<RuUserInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicy.Ru)
               .Override(IpAddresses.RuIpAddress, ui => ui.IpAddress)
               .Select(ui => new RuUserInfo(ui));
    }

    internal class UserWithOnboardingTriggerArbitrary : Arbitrary<UserWithOnboardingTriggersInfo>
    {
        public static Arbitrary<UserWithOnboardingTriggersInfo> Create() => new UserWithOnboardingTriggerArbitrary();

        public override Gen<UserWithOnboardingTriggersInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicyGenerators.WithOnboardingTriggers)
               .Select(ui => new UserWithOnboardingTriggersInfo(ui));
    }

    internal class UserWithoutOnboardingTriggerArbitrary : Arbitrary<UserWithoutOnboardingTriggersInfo>
    {
        public static Arbitrary<UserWithoutOnboardingTriggersInfo> Create() => new UserWithoutOnboardingTriggerArbitrary();

        public override Gen<UserWithoutOnboardingTriggersInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicyGenerators.WithoutOnboardingTriggers)
               .Select(ui => new UserWithoutOnboardingTriggersInfo(ui));
    }

    internal class UserWithMonitoringTriggerArbitrary : Arbitrary<UserWithMonitoringTriggersInfo>
    {
        public static Arbitrary<UserWithMonitoringTriggersInfo> Create() => new UserWithMonitoringTriggerArbitrary();

        public override Gen<UserWithMonitoringTriggersInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicyGenerators.WithMonitoringTriggers)
               .Select(ui => new UserWithMonitoringTriggersInfo(ui));
    }

    internal class UserWithoutMonitoringTriggerArbitrary : Arbitrary<UserWithoutMonitoringTriggersInfo>
    {
        public static Arbitrary<UserWithoutMonitoringTriggersInfo> Create() => new UserWithoutMonitoringTriggerArbitrary();

        public override Gen<UserWithoutMonitoringTriggersInfo> Generator =>
            Arb.Generate<UserInfo>()
               .OverridePolicy(VerificationPolicyGenerators.WithoutMonitoringTriggers)
               .Select(ui => new UserWithoutMonitoringTriggersInfo(ui));
    }
}
