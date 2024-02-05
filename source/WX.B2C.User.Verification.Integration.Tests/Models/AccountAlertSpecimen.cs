using System;
using System.Collections;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    internal class AccountAlertsSpecimen : IEnumerable<AccountAlertSpecimen>
    {
        public AccountAlertSpecimen LowAccountAlert { get; set; }

        public AccountAlertSpecimen MediumAccountAlert { get; set; }

        public AccountAlertSpecimen HighAccountAlert { get; set; }

        public AccountAlertSpecimen ExtraHighAccountAlert { get; set; }

        public void Deconstruct(out AccountAlertSpecimen lowAccountAlert,
                                out AccountAlertSpecimen mediumAccountAlert,
                                out AccountAlertSpecimen highAccountAlert,
                                out AccountAlertSpecimen extraHighAccountAlert)
        {
            lowAccountAlert = LowAccountAlert;
            mediumAccountAlert = MediumAccountAlert;
            highAccountAlert = HighAccountAlert;
            extraHighAccountAlert = ExtraHighAccountAlert;
        }

        public IEnumerator<AccountAlertSpecimen> GetEnumerator()
        {
            yield return LowAccountAlert;
            yield return MediumAccountAlert;
            yield return HighAccountAlert;
            yield return ExtraHighAccountAlert;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class LowAccountAlertSpecimen : AccountAlertSpecimen
    {
        public static LowAccountAlertSpecimen Create(ApplicationSpecimen applicationSpecimen,
                                                     VerificationDetailsDto verificationDetails,
                                                     PersonalDetailsDto personalDetails,
                                                     ApplicationStateChangelogDto applicationChangelog) =>
            Create<LowAccountAlertSpecimen>(applicationSpecimen, verificationDetails, personalDetails, applicationChangelog);
    }

    internal class MediumAccountAlertSpecimen : AccountAlertSpecimen
    {
        public static MediumAccountAlertSpecimen Create(ApplicationSpecimen applicationSpecimen,
                                                        VerificationDetailsDto verificationDetails,
                                                        PersonalDetailsDto personalDetails,
                                                        ApplicationStateChangelogDto applicationChangelog) =>
            Create<MediumAccountAlertSpecimen>(applicationSpecimen, verificationDetails, personalDetails, applicationChangelog);
    }

    internal class HighAccountAlertSpecimen : AccountAlertSpecimen
    {
        public static HighAccountAlertSpecimen Create(ApplicationSpecimen applicationSpecimen,
                                                      VerificationDetailsDto verificationDetails,
                                                      PersonalDetailsDto personalDetails,
                                                      ApplicationStateChangelogDto applicationChangelog) =>
            Create<HighAccountAlertSpecimen>(applicationSpecimen, verificationDetails, personalDetails, applicationChangelog);
    }

    internal class ExtraHighAccountAlertSpecimen : AccountAlertSpecimen
    {
        public static ExtraHighAccountAlertSpecimen Create(ApplicationSpecimen applicationSpecimen,
                                                           VerificationDetailsDto verificationDetails,
                                                           PersonalDetailsDto personalDetails,
                                                           ApplicationStateChangelogDto applicationChangelog) =>
            Create<ExtraHighAccountAlertSpecimen>(applicationSpecimen, verificationDetails, personalDetails, applicationChangelog);
    }

    internal abstract class AccountAlertSpecimen
    {
        protected ApplicationSpecimen _applicationSpecimen;
        protected VerificationDetailsDto _verificationDetails;
        protected PersonalDetailsDto _personalDetails;
        protected ApplicationStateChangelogDto _applicationChangelog;

        public Guid UserId 
        { 
            get => _applicationSpecimen.UserId;
            set => _applicationSpecimen.UserId = _verificationDetails.UserId = _personalDetails.UserId = value;
        }

        public RiskLevel? RiskLevel
        {
            get => _verificationDetails.RiskLevel;
            set => _verificationDetails.RiskLevel = value;
        }

        public decimal? Turnover
        {
            get => _verificationDetails.Turnover;
            set => _verificationDetails.Turnover = value;
        }

        public Guid ApplicationId
        {
            get => _applicationSpecimen.Id;
            set => _applicationSpecimen.Id = _applicationChangelog.ApplicationId = value;
        }

        public ApplicationState ApplicationState
        {
            get => _applicationSpecimen.State;
            set => _applicationSpecimen.State = value;
        }

        public DateTime? LastApprovedDate
        {
            get => _applicationChangelog.LastApprovedDate;
            set => _applicationChangelog.LastApprovedDate = value;
        }

        public string Country
        {
            get => _personalDetails.ResidenceAddress.Country;
            set => _personalDetails.ResidenceAddress.Country = value;
        }

        public void Deconstruct(out ApplicationSpecimen applicationSpecimen, 
                                out VerificationDetailsDto verificationDetails, 
                                out PersonalDetailsDto personalDetails,
                                out ApplicationStateChangelogDto applicationChangelog)
        {
            applicationSpecimen = _applicationSpecimen;
            verificationDetails = _verificationDetails;
            personalDetails = _personalDetails;
            applicationChangelog = _applicationChangelog;
        }

        protected static TAlertSpecimen Create<TAlertSpecimen>(ApplicationSpecimen applicationSpecimen,
                                                               VerificationDetailsDto verificationDetails,
                                                               PersonalDetailsDto personalDetails,
                                                               ApplicationStateChangelogDto applicationChangelog)
            where TAlertSpecimen : AccountAlertSpecimen, new()
        {
            var alertSpecimen = new TAlertSpecimen
            {
                _applicationSpecimen = applicationSpecimen,
                _verificationDetails = verificationDetails,
                _personalDetails = personalDetails,
                _applicationChangelog = applicationChangelog
            };

            alertSpecimen.UserId = applicationSpecimen.UserId;
            alertSpecimen.ApplicationId = applicationSpecimen.Id;
            return alertSpecimen;
        }
    }
}
