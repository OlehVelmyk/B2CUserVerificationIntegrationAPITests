using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Autofac;
using FsCheck;
using FluentAssertions;
using NUnit.Framework;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using System.Collections.Generic;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs.Providers
{
    internal class AccountAlertInfoProviderTests : BaseIntegrationTest
    {
        private AccountAlertInfoProvider _sut;
        private BatchJobSettings _jobSettings;
        private AccountAlertFixture _alertFixture;

        private List<AccountAlertSpecimen> _savedAlerts = new List<AccountAlertSpecimen>();

        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterDbQueryFactory();
            containerBuilder.RegisterType<AccountAlertInfoProvider>().AsSelf();
            containerBuilder.Register(c => new AccountAlertJobConfig
            {
                ExcludedCountries = new[] { "US", "AU", "HK", "MY", "SG", "KR", "TW", "PH", "TH" },
                ApplicationState = ApplicationState.Approved,
                Periods = new[]
                {
                    new AlertPeriod { RiskLevel = RiskLevel.ExtraHigh, AccountAge = 1, OverallTurnover = 50 },
                    new AlertPeriod { RiskLevel = RiskLevel.High, AccountAge = 1, OverallTurnover = 50 },
                    new AlertPeriod { RiskLevel = RiskLevel.Medium, AccountAge = 2, OverallTurnover = 250 },
                    new AlertPeriod { RiskLevel = RiskLevel.Low, AccountAge = 3, OverallTurnover = 750 }
                }
            }).AsSelf();

            containerBuilder.RegisterType<AccountAlertFixture>().AsSelf();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = Resolve<AccountAlertInfoProvider>();

            _jobSettings = new BatchJobSettings
            {
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0,
                ProcessBatchSize = 100,
                ReadingBatchSize = 3
            };

            _alertFixture = Resolve<AccountAlertFixture>();

            Arb.Register<ApplicationArbitrary>();
            Arb.Register<VerificationTaskArbitrary>();
            Arb.Register<ApplicationChangelogArbitrary>();
            Arb.Register<VerificationDetailsArbitrary>();
            Arb.Register<PersonalDetailsArbitrary>();
            Arb.Register<LowAccountAlertArbitrary>();
            Arb.Register<MediumAccountAlertArbitrary>();
            Arb.Register<HighAccountAlertArbitrary>();
            Arb.Register<ExtraHighAccountAlertArbitrary>();
            Arb.Register<AccountAlertsArbitrary>();
        }

        [TearDown]
        public Task TearDown() => _alertFixture.RemoveAsync(_savedAlerts);

        [Theory]
        public async Task ShouldFindAlerts(AccountAlertsSpecimen alertsSpecimen)
        {
            // Given
            await _alertFixture.SaveAsync(alertsSpecimen);
            _savedAlerts.AddRange(alertsSpecimen);

            // Act
            var totalCount = await _sut.GetTotalCountAsync(_jobSettings, CancellationToken.None);
            var result = await _sut.GetAsync(_jobSettings, CancellationToken.None).ToEnumerableAsync();
            var alerts = result.Flatten().ToArray();

            // Assert
            alerts.Should().HaveCount(totalCount, "Should be same amount as from 'GetTotalCountAsync' method.");
                                
            foreach (var actual in alerts)
            {
                var expected = alertsSpecimen.SingleOrDefault(expected => expected.UserId == actual.UserId);

                expected.Should().NotBeNull();
                actual.RiskLevel.Should().Be(expected.RiskLevel);
                actual.Turnover.Should().Be(expected.Turnover);
                actual.ApplicationState.Should().Be(expected.ApplicationState);
                actual.LastApprovedDate.Should().Be(expected.LastApprovedDate);
            }
        }

        [Theory]
        public async Task ShouldFindAlertWithLowRiskLevel_WhenRiskLevelIsNull(LowAccountAlertSpecimen low, MediumAccountAlertSpecimen medium)
        {
            // Given
            low.RiskLevel = null;
            medium.RiskLevel = null;
            var alerts = new AccountAlertSpecimen[] { low, medium };
            await _alertFixture.SaveAsync(alerts);
            _savedAlerts.AddRange(alerts);

            var expectedAlert = low;

            // Act
            var totalCount = await _sut.GetTotalCountAsync(_jobSettings, CancellationToken.None);
            var result = await _sut.GetAsync(_jobSettings, CancellationToken.None).ToEnumerableAsync();

            // Assert
            var actual = result.Should().HaveCount(1).And.Subject.First()
                               .Should().HaveCount(totalCount, "Should be same amount as from 'GetTotalCountAsync' method.").And.Subject
                               .Should().HaveCount(1).And.Subject.First();

            actual.Should().NotBeNull();
            actual.UserId.Should().Be(expectedAlert.UserId);
            actual.RiskLevel.Should().Be(RiskLevel.Low);
            actual.Turnover.Should().Be(expectedAlert.Turnover);
            actual.ApplicationState.Should().Be(expectedAlert.ApplicationState);
            actual.LastApprovedDate.Should().Be(expectedAlert.LastApprovedDate);
        }

        /// <remark>
        /// Test should find only alerts that satisfy conditions
        /// </remark>
        [Theory]
        public async Task ShouldNotFindAlerts_WhenExcludedCountryAndForbiddenState(AccountAlertsSpecimen alertsSpecimen)
        {
            // Arrange and Given
            var (low, medium, high, extraHigh) = alertsSpecimen;
            low.Country = "US";
            medium.ApplicationState = ApplicationState.Applied;

            await _alertFixture.SaveAsync(alertsSpecimen);
            _savedAlerts.AddRange(alertsSpecimen);

            // Arrange
            var expectedAlerts = new[] { high, extraHigh };

            // Act
            var totalCount = await _sut.GetTotalCountAsync(_jobSettings, CancellationToken.None);
            var result = await _sut.GetAsync(_jobSettings, CancellationToken.None).ToEnumerableAsync();

            // Assert
            var alerts = result.Should().HaveCount(1).And.Subject.First()
                               .Should().HaveCount(totalCount, "Should be same amount as from 'GetTotalCountAsync' method.").And.Subject
                               .Should().HaveCount(2).And.Subject;
            foreach (var actual in alerts)
            {
                var expected = expectedAlerts.SingleOrDefault(expected => expected.UserId == actual.UserId);

                expected.Should().NotBeNull();
                actual.RiskLevel.Should().Be(expected.RiskLevel);
                actual.Turnover.Should().Be(expected.Turnover);
                actual.ApplicationState.Should().Be(expected.ApplicationState);
                actual.LastApprovedDate.Should().Be(expected.LastApprovedDate);
            }
        }

        /// <remark>
        /// Test should find only alerts that satisfy conditions
        /// </remark>
        [Theory]
        public async Task ShouldNotFindAlerts_WhenNotExceededTurnoverAndInappropriateRiskLevel(AccountAlertsSpecimen alertsSpecimen)
        {
            // Given
            var (low, medium, high, extraHigh) = alertsSpecimen;
            (low.RiskLevel, medium.Turnover, high.Turnover) = (RiskLevel.ExtraHigh, 1, null);

            await _alertFixture.SaveAsync(alertsSpecimen);
            _savedAlerts.AddRange(alertsSpecimen);

            // Arrange
            var expectedAlert = extraHigh;

            // Act
            var totalCount = await _sut.GetTotalCountAsync(_jobSettings, CancellationToken.None);
            var result = await _sut.GetAsync(_jobSettings, CancellationToken.None).ToEnumerableAsync();

            // Assert
            var actual = result.Should().HaveCount(1).And.Subject.First()
                               .Should().HaveCount(totalCount, "Should be same amount as from 'GetTotalCountAsync' method.").And.Subject
                               .Should().HaveCount(1).And.Subject.First();

            actual.Should().NotBeNull();
            actual.UserId.Should().Be(expectedAlert.UserId);
            actual.RiskLevel.Should().Be(expectedAlert.RiskLevel);
            actual.Turnover.Should().Be(expectedAlert.Turnover);
            actual.ApplicationState.Should().Be(expectedAlert.ApplicationState);
            actual.LastApprovedDate.Should().Be(expectedAlert.LastApprovedDate);
        }

        /// <remark>
        /// Test should find only alerts that satisfy conditions
        /// </remark>
        [Theory]
        public async Task ShouldNotFindAlerts_WhenInappropriateAccountAge(AccountAlertsSpecimen alertsSpecimen)
        {
            // Given
            var (low, medium, high, extraHigh) = alertsSpecimen;
            (low.LastApprovedDate, medium.LastApprovedDate, high.LastApprovedDate) = (DateTime.Now, DateTime.Now.AddYears(2), null);

            await _alertFixture.SaveAsync(alertsSpecimen);
            _savedAlerts.AddRange(alertsSpecimen);

            // Arrange
            var expectedAlert = extraHigh;

            // Act
            var totalCount = await _sut.GetTotalCountAsync(_jobSettings, CancellationToken.None);
            var result = await _sut.GetAsync(_jobSettings, CancellationToken.None).ToEnumerableAsync();

            // Assert
            var actual = result.Should().HaveCount(1).And.Subject.First()
                               .Should().HaveCount(totalCount, "Should be same amount as from 'GetTotalCountAsync' method.").And.Subject
                               .Should().HaveCount(1).And.Subject.First();

            actual.Should().NotBeNull();
            actual.UserId.Should().Be(expectedAlert.UserId);
            actual.RiskLevel.Should().Be(expectedAlert.RiskLevel);
            actual.Turnover.Should().Be(expectedAlert.Turnover);
            actual.ApplicationState.Should().Be(expectedAlert.ApplicationState);
            actual.LastApprovedDate.Should().Be(expectedAlert.LastApprovedDate);
        }
    }
}
