using System;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class AccountAlertsArbitrary : Arbitrary<AccountAlertsSpecimen>
    {
        public static Arbitrary<AccountAlertsSpecimen> Create() => new AccountAlertsArbitrary();

        public override Gen<AccountAlertsSpecimen> Generator =>
            from low in Arb.Generate<LowAccountAlertSpecimen>()
            from medium in Arb.Generate<MediumAccountAlertSpecimen>()
            from high in Arb.Generate<HighAccountAlertSpecimen>()
            from extraHigh in Arb.Generate<ExtraHighAccountAlertSpecimen>()
            select new AccountAlertsSpecimen
            {
                LowAccountAlert = low,
                MediumAccountAlert = medium,
                HighAccountAlert = high,
                ExtraHighAccountAlert = extraHigh
            };
    }

    internal class LowAccountAlertArbitrary : Arbitrary<LowAccountAlertSpecimen>
    {
        public static Arbitrary<LowAccountAlertSpecimen> Create() => new LowAccountAlertArbitrary();

        public override Gen<LowAccountAlertSpecimen> Generator =>
            from application in Arb.Generate<ApplicationSpecimen>()
            from verificationDetails in Arb.Generate<VerificationDetailsDto>()
            from personalDetails in Arb.Generate<PersonalDetailsDto>()
            from applicationChangelog in Arb.Generate<ApplicationStateChangelogDto>()
            let alert = LowAccountAlertSpecimen.Create(application, verificationDetails, personalDetails, applicationChangelog)
            let _ = FillAlert(alert)
            select alert;

        private AccountAlertSpecimen FillAlert(LowAccountAlertSpecimen alert)
        {
            alert.RiskLevel = RiskLevel.Low;
            alert.Turnover = 1000;
            alert.ApplicationState = ApplicationState.Approved;
            alert.LastApprovedDate = DateTime.UtcNow.AddYears(-3);
            alert.Country = "GB";
            return alert;
        }
    }

    internal class MediumAccountAlertArbitrary : Arbitrary<MediumAccountAlertSpecimen>
    {
        public static Arbitrary<MediumAccountAlertSpecimen> Create() => new MediumAccountAlertArbitrary();

        public override Gen<MediumAccountAlertSpecimen> Generator =>
            from application in Arb.Generate<ApplicationSpecimen>()
            from verificationDetails in Arb.Generate<VerificationDetailsDto>()
            from personalDetails in Arb.Generate<PersonalDetailsDto>()
            from applicationChangelog in Arb.Generate<ApplicationStateChangelogDto>()
            let alert = MediumAccountAlertSpecimen.Create(application, verificationDetails, personalDetails, applicationChangelog)
            let _ = FillAlert(alert)
            select alert;

        private AccountAlertSpecimen FillAlert(AccountAlertSpecimen alert)
        {
            alert.RiskLevel = RiskLevel.Medium;
            alert.Turnover = 500;
            alert.ApplicationState = ApplicationState.Approved;
            alert.LastApprovedDate = DateTime.UtcNow.AddYears(-2);
            alert.Country = "GB";
            return alert;
        }
    }

    internal class HighAccountAlertArbitrary : Arbitrary<HighAccountAlertSpecimen>
    {
        public static Arbitrary<HighAccountAlertSpecimen> Create() => new HighAccountAlertArbitrary();

        public override Gen<HighAccountAlertSpecimen> Generator =>
            from application in Arb.Generate<ApplicationSpecimen>()
            from verificationDetails in Arb.Generate<VerificationDetailsDto>()
            from personalDetails in Arb.Generate<PersonalDetailsDto>()
            from applicationChangelog in Arb.Generate<ApplicationStateChangelogDto>()
            let alert = HighAccountAlertSpecimen.Create(application, verificationDetails, personalDetails, applicationChangelog)
            let _ = FillAlert(alert)
            select alert;

        private AccountAlertSpecimen FillAlert(AccountAlertSpecimen alert)
        {
            alert.RiskLevel = RiskLevel.High;
            alert.Turnover = 100;
            alert.ApplicationState = ApplicationState.Approved;
            alert.LastApprovedDate = DateTime.UtcNow.AddYears(-1);
            alert.Country = "GB";
            return alert;
        }
    }

    internal class ExtraHighAccountAlertArbitrary : Arbitrary<ExtraHighAccountAlertSpecimen>
    {
        public static Arbitrary<ExtraHighAccountAlertSpecimen> Create() => new ExtraHighAccountAlertArbitrary();

        public override Gen<ExtraHighAccountAlertSpecimen> Generator =>
            from application in Arb.Generate<ApplicationSpecimen>()
            from verificationDetails in Arb.Generate<VerificationDetailsDto>()
            from personalDetails in Arb.Generate<PersonalDetailsDto>()
            from applicationChangelog in Arb.Generate<ApplicationStateChangelogDto>()
            let alert = ExtraHighAccountAlertSpecimen.Create(application, verificationDetails, personalDetails, applicationChangelog)
            let _ = FillAlert(alert)
            select alert;

        private AccountAlertSpecimen FillAlert(AccountAlertSpecimen alert)
        {
            alert.RiskLevel = RiskLevel.ExtraHigh;
            alert.Turnover = 100;
            alert.ApplicationState = ApplicationState.Approved;
            alert.LastApprovedDate = DateTime.UtcNow.AddYears(-1);
            alert.Country = "GB";
            return alert;
        }
    }
}
