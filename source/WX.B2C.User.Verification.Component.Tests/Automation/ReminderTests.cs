using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Helpers;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class ReminderTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private CollectionStepsFixture _stepsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;

        private ReminderFixture _reminderFixture;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _stepsFixture = Resolve<CollectionStepsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();
            _reminderFixture = Resolve<ReminderFixture>();

            Arb.Register<UserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Get all reminders (Absent)
        /// Given user without reminders
        /// When admin requests reminders assigned to user
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyArray_WhenUserHasNoReminders(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.RegisterApplicationAsync(userInfo);

            // Act
            var activeReminders = await adminClient.Reminders.GetActiveRemindersAsync(userId);
            var sentReminders = await adminClient.Reminders.GetSentRemindersAsync(userId);

            // Assert
            activeReminders.Should().BeEmpty();
            sentReminders.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Reminder for action with highest priority must be scheduled
        /// Given user register application
        /// When application is built
        /// Then reminder must scheduled
        /// And reminder must be connected to highest priority collection step
        /// </summary>
        [Theory]
        public async Task ShouldScheduleReminder_WhenApplicationAutomated(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var expectedTimeToRaise = DateTime.UtcNow.AddMinutes(2);

            // Act
            var reminder = await _reminderFixture.GetActiveReminderAsync(userId);

            reminder.Should().NotBeNull();
            reminder.UserId.Should().Be(userId);
            reminder.SendAt.Should().BeCloseTo(expectedTimeToRaise, TimeSpan.FromSeconds(10));

            var step = await adminClient.CollectionStep.GetAsync(userId, reminder.TargetId);
            step.State.Should().Be(CollectionStepState.Requested);
            step.Variant.Should()
                .BeOfType<DocumentCollectionStepVariantDto>()
                .Which.DocumentCategory.Should()
                .Be(DocumentCategory.ProofOfIdentity);
        }

        /// <summary>
        /// Scenario: Create second reminder when first reminder sent
        /// Given user built application
        /// When first reminder sent
        /// Then next reminder should be created
        ///  </summary>
        [Theory]
        public async Task ShouldScheduleNewReminder_WhenPreviousReminderWasSent(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var firstReminder = await _reminderFixture.GetActiveReminderAsync(userId);

            // Act
            await _reminderFixture.FireReminderJob(firstReminder);

            // Assert
            var reminder = await _reminderFixture.GetActiveReminderAsync(userId);
            reminder.Should().NotBeNull();
            reminder.TargetId.Should().Be(firstReminder.TargetId);
            reminder.UserId.Should().Be(userId);

            var step = await adminClient.CollectionStep.GetAsync(userId, reminder.TargetId);
            step.State.Should().Be(CollectionStepState.Requested);
            step.Variant.Should()
                .BeOfType<DocumentCollectionStepVariantDto>()
                .Which.DocumentCategory.Should()
                .Be(DocumentCategory.ProofOfIdentity);

            var sentReminder = await _reminderFixture.GetLastSentReminderAsync(userId);
            sentReminder.Should().NotBeNull();
            sentReminder.TargetId.Should().Be(firstReminder.TargetId);
            sentReminder.UserId.Should().Be(userId);
        }

        /// <summary>
        /// Scenario: Exceed rescheduling attempts 
        /// Given user with scheduled reminder
        /// When max reschedulling attempts are exceeded
        /// Then new reminder is not scheduled
        /// </summary>
        [Theory]
        public async Task ShouldExceedReschedulingAttempt(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            const int expectedCountOfReminders = 5;
            var actualCountOfReminders = 0;

            // Act
            while (await _reminderFixture.GetActiveReminderAsync(userId) is { } reminder &&
                   actualCountOfReminders < 10 /* check for prevent infinite loop*/)
            {
                await _reminderFixture.FireReminderJob(reminder);
                actualCountOfReminders++;
            }

            // Assert
            actualCountOfReminders.Should().Be(expectedCountOfReminders);

            var sentReminders = await adminClient.Reminders.GetSentRemindersAsync(userId);
            sentReminders.Should().HaveCount(expectedCountOfReminders);

            var activeReminders = await adminClient.Reminders.GetActiveRemindersAsync(userId);
            activeReminders.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Create reminder for new action, when previous action completed
        /// Given user built application
        /// When user complete first action
        /// Then reminder for next action must be created
        /// </summary>
        [Theory]
        public async Task ShouldScheduleNewReminder_WhenPreviousActionCompleted(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var firstReminder = await _reminderFixture.GetActiveReminderAsync(userId);

            // Act
            await _stepsFixture.CompleteAsync(userId, firstReminder.TargetId, seed);

            // !!!TODO no events or data in db when reminder cancelled and new one requested
            await Task.Delay(TimeSpan.FromMilliseconds(300));

            // Assert
            var reminder = await _reminderFixture.GetActiveReminderAsync(userId);
            reminder.Should().NotBeNull();
            reminder.TargetId.Should().NotBe(firstReminder.TargetId);
            reminder.UserId.Should().Be(userId);

            var step = await adminClient.CollectionStep.GetAsync(userId, reminder.TargetId);
            step.State.Should().Be(CollectionStepState.Requested);
            step.Variant.Should()
                .BeOfType<DocumentCollectionStepVariantDto>()
                .Which.DocumentCategory.Should()
                .Be(DocumentCategory.Selfie);
        }

        /// <summary>
        /// Scenario: Should not sent any reminders when user complete all actions
        /// Given user built application
        /// When user complete all actions
        /// Then all created reminders must be cancelled
        /// And no sent reminders must exists
        /// </summary>
        [Theory]
        public async Task ShouldNotSendAnyReminder_WhenUserCompleteAllActionsBeforeSending(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            while (await _reminderFixture.GetActiveReminderAsync(userId) is { } activeReminder)
            {
                await _stepsFixture.CompleteAsync(userId, activeReminder.TargetId, seed);
                // !!!TODO no events or data in db when reminder cancelled and new one requested
                await Task.Delay(TimeSpan.FromMilliseconds(300));
            }

            // Assert
            var reminders = await adminClient.Reminders.GetActiveRemindersAsync(userId);
            reminders.Should().BeEmpty();

            var sentReminders = await adminClient.Reminders.GetSentRemindersAsync(userId);
            sentReminders.Should().BeEmpty();
        }
        
        /// <summary>
        /// Scenario: Should sent reminders several times until user complete actions
        /// Given user built application
        /// When reminder sent twice
        /// And user complete action
        /// Then next reminder must created
        /// And after all actions completed no more reminders must be created
        /// </summary>
        [Theory]
        public async Task ShouldSendRemindersTwice(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var actionNumbers = 0;

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            while (await _reminderFixture.GetActiveReminderAsync(userId) is { } activeReminder)
            {
                await _reminderFixture.FireReminderJob(activeReminder);
                await _reminderFixture.FireReminderJob(activeReminder);
                await _stepsFixture.CompleteAsync(userId, activeReminder.TargetId, seed);
                // !!!TODO no events or data in db when reminder cancelled and new one requested
                await Task.Delay(TimeSpan.FromMilliseconds(300));
                actionNumbers++;
            }

            // Assert
            var reminders = await adminClient.Reminders.GetActiveRemindersAsync(userId);
            reminders.Should().BeEmpty();

            var sentReminders = await adminClient.Reminders.GetSentRemindersAsync(userId);
            sentReminders.Should().HaveCount(actionNumbers * 2);
        }


        /// <summary>
        /// Scenario: Cancel reminder
        /// Given user with scheduled reminder
        /// When required data is submitted
        /// And collection step is completed
        /// Then reminder is cancelled
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldCancelReminder_WhenCollectionStepCompleted(UserInfo userInfo, Seed seed)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Cancel reminder
        /// Given user with scheduled reminder
        /// When required data is submitted
        /// And collection step is moved in review
        /// Then reminder is cancelled
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldCancelReminder_WhenCollectionStepMovedInReview(UserInfo userInfo, Seed seed)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Schedule reminder (Previous optional)
        /// Given user with scheduled reminder
        /// When action becomes optional
        /// Then reminder is cancelled
        /// And new one is scheduled
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldScheduleRequiredReminder_WhenPreviousBecameOptional(UserInfo userInfo)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Schedule reminder (More priority step required)
        /// Given user with scheduled reminder
        /// When more priority action becomes required
        /// Then reminder is cancelled
        /// And new one is scheduled
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldScheduleReminder_WhenMorePriorityStepBecameRequired(UserInfo userInfo)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Schedule reminder (No required)
        /// Given user with scheduled reminder
        /// And all actions are optional
        /// When reminder is cancelled
        /// Then new reminder is cancelled
        /// And it is associated with optional action
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldScheduleOptionalReminder_WhenNoRequired(UserInfo userInfo, Seed seed)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Schedule reminder (Previous cancelled)
        /// Given user with scheduled reminder
        /// When reminder is cancelled
        /// Then new reminder is scheduled
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldScheduleReminder_WhenPreviousCancelled(UserInfo userInfo, Seed seed)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Schedule reminder (More priority step)
        /// Given user with scheduled reminder
        /// When collection step is requested
        /// And step is associated with more priority action
        /// Then reminder is cancelled
        /// And new more prioity reminder is scheduled
        /// And events are raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldScheduleReminder_WhenMorePriorityStepRequested(UserInfo userInfo, Seed seed)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Scenario: Not schedule reminder (Less priority step)
        /// Given user with scheduled reminder
        /// When collection step is requested
        /// And step is associated with less priority action
        /// Then reminder is not cancelled
        /// And new reminder is not scheduled
        /// And no events are raised
        /// </summary>
        [Theory, Ignore("Not implemented")]
        public Task ShouldNotScheduleReminder_WhenLessPriorityStepRequested(UserInfo userInfo, Seed seed)
        {
            return Task.CompletedTask;
        }
    }
}