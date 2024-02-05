using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class TriggerTests : BaseComponentTest
    {
        private TriggerFixture _triggerFixture;
        private ApplicationFixture _applicationFixture;
        private ProfileFixture _profileFixture;
        private EventsFixture _eventsFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;
        private PublicApiClientFactory _publicApiClientFactory;
        private ITriggerProvider _triggerProvider;
        private TurnoverFixture _turnoverFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _triggerFixture = Resolve<TriggerFixture>();
            _applicationFixture = Resolve<ApplicationFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _turnoverFixture = Resolve<TurnoverFixture>();

            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();
            _triggerProvider = Resolve<ITriggerProvider>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<GbUserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<UserWithOnboardingTriggerArbitrary>();
            Arb.Register<UserWithoutOnboardingTriggerArbitrary>();
            Arb.Register<UserWithMonitoringTriggerArbitrary>();
            Arb.Register<UserWithoutMonitoringTriggerArbitrary>();
        }

        /// <summary>
        /// Scenario: Schedule onboarding triggers
        /// Given user
        /// And user verification policy has onboarding triggers
        /// When he starts verification
        /// Then onboarding triggers are scheduled
        /// </summary>
        [Theory]
        public async Task ShouldScheduleOnboardingTriggers(UserWithOnboardingTriggersInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var client = _publicApiClientFactory.Create(userId);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);

            // Act
            var scheduleAfter = DateTime.UtcNow;
            await client.Applications.RegisterAsync();

            // Assert
            var application = await adminApiClient.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            var triggersVariantIds = _triggerProvider.GetOnbording(userInfo.Policy);
            foreach (var variantId in triggersVariantIds)
            {
                var @event = _eventsFixture.ShouldExistSingle<TriggerScheduledEvent>(e => e.EventArgs.UserId == userId &&
                                                                                          e.EventArgs.VariantId == variantId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.ScheduleDate.Should().BeAfter(scheduleAfter);
                @event.EventArgs.TriggerId.Should().NotBeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Do not schedule onboarding triggers
        /// Given user
        /// And user verification policy does not have onboarding triggers
        /// When he starts verification
        /// Then onboarding triggers are not scheduled
        /// </summary>
        [Theory]
        public async Task ShouldNotScheduleOnboardingTriggers(UserWithoutOnboardingTriggersInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);

            // Act
            await client.Applications.RegisterAsync();

            // Assert
            _eventsFixture.ShouldNotExist<TriggerScheduledEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Schedule monitoring triggers
        /// Given user
        /// And user verification policy has monitoring triggers
        /// When his application becomes approved
        /// Then monitoring triggers are scheduled
        /// </summary>
        [Theory]
        public async Task ShouldScheduleMonitoringTriggers(UserWithMonitoringTriggersInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            // Act
            var scheduleAfter = DateTime.UtcNow;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Assert
            var triggersVariantIds = _triggerProvider.GetMonitoring(userInfo.Policy);
            foreach (var variantId in triggersVariantIds)
            {
                var @event = _eventsFixture.ShouldExistSingle<TriggerScheduledEvent>(e => e.EventArgs.UserId == userId &&
                                                                                          e.EventArgs.VariantId == variantId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.ScheduleDate.Should().BeAfter(scheduleAfter);
                @event.EventArgs.TriggerId.Should().NotBeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Schedule monitoring triggers
        /// Given user from GB
        /// And user have approved application
        /// When user becomes PEP
        /// Then pep user review reminder trigger scheduled
        /// </summary>
        [Theory]
        public async Task ShouldScheduleTriggerWithPrecondition(GbUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Act
            var scheduleAfter = DateTime.UtcNow;
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.WithIsPep(true));

            // Assert
            var variantId = new Guid("1F50ACCD-F4F2-4744-8D89-9F4A27DF17B1");
            var @event = _eventsFixture.ShouldExistSingle<TriggerScheduledEvent>(e => e.EventArgs.UserId == userId &&
                                                                                      e.EventArgs.VariantId == variantId);
            @event.EventArgs.ApplicationId.Should().Be(applicationId);
            @event.EventArgs.ScheduleDate.Should().BeAfter(scheduleAfter);
            @event.EventArgs.TriggerId.Should().NotBeEmpty();
        }

        /// <summary>
        /// Scenario: Unschedule monitoring triggers
        /// Given user from GB
        /// And user have approved application
        /// And user is PEP
        /// When user becomes not PEP
        /// Then pep user review reminder trigger unscheduled
        /// </summary>
        [Theory]
        public async Task ShouldUnscheduleTriggerWithPrecondition(GbUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.WithIsPep(true));

            var variantId = new Guid("1F50ACCD-F4F2-4744-8D89-9F4A27DF17B1");
            _eventsFixture.ShouldExistSingle<TriggerScheduledEvent>(
            e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == variantId);

            // Act
            var unscheduleAfter = DateTime.UtcNow;
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.WithIsPep(false));

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<TriggerUnscheduledEvent>(
            e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == variantId);

            @event.EventArgs.ApplicationId.Should().Be(applicationId);
            @event.EventArgs.UnscheduleDate.Should().BeAfter(unscheduleAfter);
            @event.EventArgs.TriggerId.Should().NotBeEmpty();
        }

        /// <summary>
        /// Scenario: Schedule monitoring triggers
        /// Given user
        /// And user verification policy does not have monitoring triggers
        /// When his application becomes approved
        /// Then monitoring triggers are not scheduled
        /// </summary>
        [Theory]
        public async Task ShouldNotScheduleMonitoringTriggers(UserWithoutMonitoringTriggersInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            // Act 
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Assert
            _eventsFixture.ShouldNotExist<TriggerScheduledEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Unschedule onboarding triggers
        /// Given user with applied application
        /// And user has scheduled onboarding triggers
        /// When his application becomes approved 
        /// Then onboarding triggers are unscheduled
        /// </summary>
        [TestCaseSource(nameof(PoliciesWithOnboardingTriggers))] // TODO: WRXB-10812
        public async Task ShouldUnscheduleOnboardingTriggers(VerificationPolicy policy)
        {
            const string Replay = "";
            var fsSeed = GenerateFsSeed(Replay);
            var seed = Arb.Generate<Seed>().Sample(fsSeed);
            var userInfo = Arb.Generate<UserInfo>().OverridePolicy(policy).Sample(fsSeed);

            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicationId = (await client.Applications.GetDefaultAsync(userId)).Id;
            _triggerFixture.ScheduleOnboardingTriggers(applicationId, policy, seed);

            // Arrange
            var unscheduleAfter = DateTime.UtcNow;

            // Act
            await _applicationFixture.RejectAsync(userId, applicationId);

            // Assert
            var triggersVariantIds = _triggerProvider.GetOnbording(userInfo.Policy);
            foreach (var variantId in triggersVariantIds)
            {
                var @event = _eventsFixture.ShouldExistSingle<TriggerUnscheduledEvent>(e => e.EventArgs.UserId == userId &&
                                                                                            e.EventArgs.VariantId == variantId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UnscheduleDate.Should().BeAfter(unscheduleAfter);
                @event.EventArgs.TriggerId.Should().NotBeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Unschedule monitoring triggers
        /// Given user with approved application
        /// And user has scheduled monitoring triggers
        /// When his application requires review 
        /// Then monitoring triggers are unscheduled
        /// </summary>
        [TestCaseSource(nameof(PoliciesWithMonitoringTriggers))] // TODO: WRXB-10812
        public async Task ShouldUnscheduleMonitoringTriggers(VerificationPolicy policy)
        {
            const string Replay = "";
            var fsSeed = GenerateFsSeed(Replay);
            var seed = Arb.Generate<Seed>().Sample(fsSeed);
            var userInfo = Arb.Generate<UserInfo>().OverridePolicy(policy).Sample(fsSeed);

            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicationId = (await client.Applications.GetDefaultAsync(userId)).Id;
            await _triggerFixture.ScheduleMonitoringTriggersAsync(userId, applicationId, policy, seed);

            // Arrange
            var unscheduleAfter = DateTime.UtcNow;

            // Act
            await _applicationFixture.RequestReviewAsync(userId, applicationId);

            // Assert
            var triggersVariantIds = _triggerProvider.GetMonitoring(userInfo.Policy);
            foreach (var variantId in triggersVariantIds)
            {
                var @event = _eventsFixture.ShouldExistSingle<TriggerUnscheduledEvent>(e => e.EventArgs.UserId == userId &&
                                                                                            e.EventArgs.VariantId == variantId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UnscheduleDate.Should().BeAfter(unscheduleAfter);
                @event.EventArgs.TriggerId.Should().NotBeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Schedule monitoring trigger for US
        /// Given user from US
        /// And user have approved application
        /// When user exceeded turnover (400)
        /// Then new pep check must be instructed
        /// </summary>
        [Theory]
        public async Task ShouldSchedulePepFamilyMemberCheck(UsUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Act
            await _turnoverFixture.UpdateAsync(userId, 400);

            // Assert
            var triggerVariantId = new Guid("50283357-6227-4793-B104-5331C80F86E2");
            var checkVariantId = new Guid("3D410715-DFC6-4952-82CA-9FD1AD53CBA5");

            _eventsFixture.ShouldExistSingle<TriggerScheduledEvent>(e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == triggerVariantId);
            _eventsFixture.ShouldExistSingle<TriggerFiredEvent>(e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == triggerVariantId);
            _eventsFixture.ShouldExistSingle<TriggerCompletedEvent>(e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == triggerVariantId);
            _eventsFixture.ShouldExistSingle<CheckCreatedEvent>(e => e.EventArgs.VariantId == checkVariantId);
            _eventsFixture.ShouldExistSingle<CheckCompletedEvent>(e => e.EventArgs.VariantId == checkVariantId);

            var task = (await client.Tasks.GetAllAsync(userId)).Single(dto => dto.Type == TaskType.RiskListsScreening);
            task.Checks.Should().HaveCount(2);
            task.Checks.Should().ContainSingle(dto => dto.Variant.Id == checkVariantId);
            task.Checks.Should().OnlyContain(dto => dto.State == CheckState.Complete);
        }

        /// <summary>
        /// Scenario: Trigger repeating threshold several times
        /// Given user with approved application from GB
        /// And user has scheduled monitoring triggers
        /// And user has reached first trigger threshold
        /// When user turnover reached threshold + trigger step
        /// Then repeating threshold trigger must be fired/completed.
        /// </summary>
        [Test]
        [Ignore("TODO: Need to implement")]
        public async Task ShouldFireIterativeTriggerSeveralTimes() { }

        private static FsCheckSeed GenerateFsSeed(string replay)
        {
            var fsSeed = string.IsNullOrEmpty(replay)
                ? FsCheckSeed.Create(Arb.Generate<Rnd>().Sample())
                : FsCheckSeed.Parse(replay);

            return fsSeed.Dump();
        }

        private static IEnumerable<TestCaseData> PoliciesWithOnboardingTriggers =>
            Policies.OnboardingTriggers.Select(policy => new TestCaseData(policy));

        private static IEnumerable<TestCaseData> PoliciesWithMonitoringTriggers =>
            Policies.MonitoringTriggers.Select(policy => new TestCaseData(policy));
    }
}