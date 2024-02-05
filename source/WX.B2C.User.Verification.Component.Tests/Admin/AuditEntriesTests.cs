using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using PublicEvents = WX.B2C.User.Verification.Events.Events;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    internal class AuditEntriesTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private TaskFixture _taskFixture;
        private DocumentsFixture _documentsFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private ProfileFixture _profileFixture;
        private EventsFixture _eventsFixture;
        private DocumentProvider _documentProvider;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _taskFixture = Resolve<TaskFixture>();
            _documentsFixture = Resolve<DocumentsFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _documentProvider = Resolve<DocumentProvider>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();

            Arb.Register<TaxResidenceArbitrary>();
            Arb.Register<UserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Receive audit entries details about personal details
        /// Given user with personal details
        /// When admin requests audit entries about personal details
        /// Then receive requested audit entries only assigned to user
        /// And event type is "PersonalDetailsUpdatedEvent"
        /// </summary>
        [Theory]
        public async Task ShouldGetPersonalDetailsAuditEntries(UserInfo userInfo)
        {
            // Given
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _profileFixture.CreateAsync(userInfo);

            // Act
            var result = await adminClient.ExecuteUntilAsync(
                client => client.AuditEntries.GetAsync(userId, filter: EntryTypeEqualFilter(EntryType.PersonalDetails)),
                result => !result.Items.IsNullOrEmpty());

            // Assert
            var auditEntry = result.Items.Should().NotBeNullOrEmpty().And.HaveCount(1).And.Subject.First();
            auditEntry.UserId.Should().Be(userId);
            auditEntry.EntryKey.Should().Be(userId);
            auditEntry.EntryType.Should().Be(EntryType.PersonalDetails);
            auditEntry.EventType.Should().Be(nameof(PersonalDetailsUpdatedEvent));
            auditEntry.Data.Should().NotBeNullOrEmpty();
            auditEntry.CreatedAt.Should().BeAfter(createdAfter);
            auditEntry.Initiation.Should().NotBeNull();
            auditEntry.Initiation.Initiator.Should().NotBeNullOrEmpty();
            auditEntry.Initiation.Reason.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Scenario: Receive audit entries details about verification details
        /// Given user with verification details
        /// When admin requests audit entries about verification details
        /// Then receive requested audit entries only assigned to user
        /// And event type is "VerificationDetailsUpdatedEvent"
        /// </summary>
        [Theory]
        public async Task ShouldGetVerificationDetailsAuditEntries(UserInfo userInfo, TaxResidence taxResidence)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _profileFixture.CreateAsync(userInfo);
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(taxResidence));

            // Act
            var result = await adminClient.ExecuteUntilAsync(
                client => client.AuditEntries.GetAsync(userId, filter: EntryTypeEqualFilter(EntryType.VerificationDetails)),
                result => !result.Items.IsNullOrEmpty());

            // Assert
            var auditEntry = result.Items.Should().NotBeNullOrEmpty().And.HaveCount(1).And.Subject.First();
            auditEntry.UserId.Should().Be(userId);
            auditEntry.EntryKey.Should().Be(userId);
            auditEntry.EntryType.Should().Be(EntryType.VerificationDetails);
            auditEntry.EventType.Should().Be(nameof(VerificationDetailsUpdatedEvent));
            auditEntry.Data.Should().NotBeNullOrEmpty();
            auditEntry.CreatedAt.Should().BeAfter(createdAfter);
            auditEntry.Initiation.Should().NotBeNull();
            auditEntry.Initiation.Initiator.Should().NotBeNullOrEmpty();
            auditEntry.Initiation.Reason.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Scenario: Receive audit entries details about documents
        /// Given user with submitted document
        /// When admin requests audit entries about documents
        /// Then receive requested audit entries only assigned to user
        /// And event type is "DocumentSubmittedEvent"
        /// </summary>
        [Theory]
        public async Task ShouldGetDocumentAuditEntries(UserInfo userInfo, Seed seed)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _profileFixture.CreateAsync(userInfo);

            var document = _documentProvider.GetDocumentToSubmit(Models.Enums.DocumentCategory.ProofOfIdentity, DocumentTypes.Passport, 1, "jpeg", seed);
            var documentId = await _documentsFixture.SubmitByAdminAsync(userId, document);

            // Act
            var result = await adminClient.ExecuteUntilAsync(
                client => client.AuditEntries.GetAsync(userId, filter: EntryTypeEqualFilter(EntryType.Document)),
                result => !result.Items.IsNullOrEmpty());

            // Assert
            var auditEntry = result.Items.Should().NotBeNullOrEmpty().And.HaveCount(1).And.Subject.First();
            auditEntry.UserId.Should().Be(userId);
            auditEntry.EntryKey.Should().Be(documentId);
            auditEntry.EntryType.Should().Be(EntryType.Document);
            auditEntry.EventType.Should().Be(nameof(DocumentSubmittedEvent));
            auditEntry.Data.Should().NotBeNullOrEmpty();
            auditEntry.CreatedAt.Should().BeAfter(createdAfter);
            auditEntry.Initiation.Should().NotBeNull();
            auditEntry.Initiation.Initiator.Should().NotBeNullOrEmpty();
            auditEntry.Initiation.Reason.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Scenario: Receive audit entries details about collection steps
        /// Given user with completed collection steps
        /// When admin requests audit entries about collection steps
        /// Then receive requested audit entries only assigned to user
        /// And next event types are present: "CollectionStepCompletedEvent", "CollectionStepRequestedEvent"
        /// </summary>
        [Theory]
        public async Task ShouldGetCollectionStepAuditEntries(UserInfo userInfo)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            _eventsFixture.ShouldExist<CollectionStepRequestedEvent>(e => e.EventArgs.UserId == userId);
            _eventsFixture.ShouldExist<CollectionStepCompletedEvent>(e => e.EventArgs.UserId == userId);

            // Arrange
            var expectedCreatedAt = DateTime.UtcNow;
            var expectedEventTypes = new[] { nameof(CollectionStepRequestedEvent), nameof(CollectionStepCompletedEvent) };

            // Act
            var result = await adminClient.ExecuteUntilAsync(
                client => client.AuditEntries.GetAsync(userId, filter: EntryTypeEqualFilter(EntryType.CollectionStep)),
                result => result.Items.Select(entry => entry.EventType).ContainsAll(expectedEventTypes));

            // Assert
            foreach (var entry in result.Items)
            {
                entry.UserId.Should().Be(userId);
                entry.EntryKey.Should().NotBeEmpty();
                entry.EntryType.Should().Be(EntryType.CollectionStep);
                entry.EventType.Should().BeOneOf(expectedEventTypes);
                entry.Data.Should().NotBeNullOrEmpty();
                entry.CreatedAt.Should().BeAfter(createdAfter);
                entry.Initiation.Should().NotBeNull();
                entry.Initiation.Initiator.Should().NotBeNullOrEmpty();
                entry.Initiation.Reason.Should().NotBeNullOrEmpty();
            }
        }

        /// <summary>
        /// Scenario: Receive audit entries details about checks
        /// Given user with completed checks
        /// When admin requests audit entries about checks
        /// Then receive requested audit entries only assigned to user
        /// And next event types are present: "CheckCreatedEvent", "CheckCompletedEvent"
        /// </summary>
        [Theory]
        public async Task ShouldGetCheckAuditEntries(UserInfo userInfo)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _applicationFixture.RegisterApplicationAsync(userInfo);
            _eventsFixture.ShouldExist<CheckCreatedEvent>(e => e.EventArgs.UserId == userId);
            _eventsFixture.ShouldExist<CheckStartedEvent>(e => e.EventArgs.UserId == userId);
            _eventsFixture.ShouldExist<CheckCompletedEvent>(e => e.EventArgs.UserId == userId);

            // Arrange
            var possibleEventTypes = new[] { nameof(CheckStartedEvent), nameof(CheckPerformedEvent), nameof(CheckErrorOccuredEvent) };
            var requiredEventTypes = new[] { nameof(CheckCreatedEvent), nameof(CheckCompletedEvent) };
            var expectedEventTypes = requiredEventTypes.Concat(possibleEventTypes).ToArray();

            // Act
            var result = await adminClient.ExecuteUntilAsync(
                client => client.AuditEntries.GetAsync(userId, filter: EntryTypeEqualFilter(EntryType.Check)),
                result => result.Items.Select(entry => entry.EventType).ContainsAll(requiredEventTypes));

            // Assert
            foreach (var entry in result.Items)
            {
                entry.UserId.Should().Be(userId);
                entry.EntryKey.Should().NotBeEmpty();
                entry.EntryType.Should().Be(EntryType.Check);
                entry.EventType.Should().BeOneOf(expectedEventTypes);
                entry.Data.Should().NotBeNullOrEmpty();
                entry.CreatedAt.Should().BeAfter(createdAfter);
                entry.Initiation.Should().NotBeNull();
                entry.Initiation.Initiator.Should().NotBeNullOrEmpty();
                entry.Initiation.Reason.Should().NotBeNullOrEmpty();
            }
        }

        /// <summary>
        /// Scenario: Receive audit entries details about tasks
        /// Given user with completed and incomplete checks
        /// When admin requests audit entries about tasks
        /// Then receive requested audit entries only assigned to user
        /// And next event types are present: "TaskCreatedEvent", "TaskCompletedEvent", "TaskIncompleteEvent"
        /// </summary>
        [Theory]
        public async Task ShouldGetTaskAuditEntries(UserInfo userInfo, Seed seed)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, new[] { TaskType.RiskListsScreening, TaskType.Address }).Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);
            await _taskFixture.IncompleteAsync(userId, taskId);

            // Arrange
            var possibleEventTypes = new[] { nameof(TaskCollectionStepAddedEvent) };
            var requiredEventTypes = new[] { nameof(TaskCreatedEvent), nameof(TaskCompletedEvent), nameof(TaskIncompleteEvent) };
            var expectedEventTypes = requiredEventTypes.Concat(possibleEventTypes).ToArray();

            // Act
            var result = await adminClient.ExecuteUntilAsync(
                client => client.AuditEntries.GetAsync(userId, filter: EntryTypeEqualFilter(EntryType.Task)),
                result => result.Items.Select(entry => entry.EventType).ContainsAll(requiredEventTypes));

            // Assert
            foreach (var entry in result.Items)
            {
                entry.UserId.Should().Be(userId);
                entry.EntryKey.Should().NotBeEmpty();
                entry.EntryType.Should().Be(EntryType.Task);
                entry.EventType.Should().BeOneOf(expectedEventTypes);
                entry.Data.Should().NotBeNullOrEmpty();
                entry.CreatedAt.Should().BeAfter(createdAfter);
                entry.Initiation.Should().NotBeNull();
                entry.Initiation.Initiator.Should().NotBeNullOrEmpty();
                entry.Initiation.Reason.Should().NotBeNullOrEmpty();
            }
        }

        /// <summary>
        /// Scenario: Receive audit entries details about applications
        /// Given user with application 
        /// And application is moved from applied state
        /// When admin requests audit entries about applications
        /// Then receive requested audit entries only assigned to user
        /// And next event types are present: "ApplicationRegisteredEvent", "ApplicationStateChangedEvent", "ApplicationRequiredTaskAddedEvent"
        /// </summary>
        [Theory]
        public async Task ShouldGetApplicationAuditEntries(UserInfo userInfo)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _applicationFixture.RegisterApplicationAsync(userInfo);

            var @event = _eventsFixture.ShouldExist<ApplicationRequiredTaskAddedEvent>(e => e.EventArgs.UserId == userId);
            var applicationId = @event.EventArgs.ApplicationId;
            
            await _applicationFixture.RejectAsync(userId, applicationId);

            // Arrange
            var expectedEventTypes = new[] { nameof(PublicEvents.ApplicationRegisteredEvent),
                                             nameof(ApplicationStateChangedEvent),
                                             nameof(ApplicationRequiredTaskAddedEvent) };

            // Act
            var result = await adminClient.ExecuteUntilAsync(
                client => client.AuditEntries.GetAsync(userId, filter: EntryTypeEqualFilter(EntryType.Application)),
                result => result.Items.Select(entry => entry.EventType).ContainsAll(expectedEventTypes));

            // Assert
            foreach (var entry in result.Items)
            {
                entry.UserId.Should().Be(userId);
                entry.EntryKey.Should().Be(applicationId);
                entry.EntryType.Should().Be(EntryType.Application);
                entry.EventType.Should().BeOneOf(expectedEventTypes);
                entry.Data.Should().NotBeNullOrEmpty();
                entry.CreatedAt.Should().BeAfter(createdAfter);
                entry.Initiation.Should().NotBeNull();
                entry.Initiation.Initiator.Should().NotBeNullOrEmpty();
                entry.Initiation.Reason.Should().NotBeNullOrEmpty();
            }
        }

        /// <summary>
        /// Scenario: Receive details about all audit entries 
        /// Given user with audit entries
        /// When admin requests audit entries
        /// Then receive audit entries only assigned to user
        /// </summary>
        [Theory]
        public async Task ShouldGetAuditEntries(UserInfo userInfo)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var userId = userInfo.UserId.Dump();
            var createdAfter = DateTime.UtcNow;
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var top = 10;

            // Act
            var result = await adminClient.AuditEntries.GetAsync(userId, top: top);

            // Assert
            result.NextPageLink.Should().NotBeNull();
            result.TotalCount.Should().BeGreaterThan(top);
            result.Items.Should().NotBeNullOrEmpty().And.HaveCount(top);
            foreach (var entry in result.Items)
            {
                entry.UserId.Should().Be(userId);
                entry.EntryKey.Should().NotBeEmpty();
                entry.EntryType.Should().HaveValue();
                entry.EventType.Should().NotBeNullOrEmpty();
                entry.Data.Should().NotBeNullOrEmpty();
                entry.CreatedAt.Should().BeAfter(createdAfter);
                entry.Initiation.Should().NotBeNull();
                entry.Initiation.Initiator.Should().NotBeNullOrEmpty();
                entry.Initiation.Reason.Should().NotBeNullOrEmpty();
            }
        }

        /// <summary>
        /// Scenario: Receive details about all audit entries (Empty)
        /// Given user without audit entries
        /// When admin requests audit entries
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyArray_WhenUserHasNoAuditEntries(Guid userId)
        {
            // Given            
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            // Act
            var result = await adminClient.AuditEntries.GetAsync(userId);

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.NextPageLink.Should().BeNull();
        }

        private static string EntryTypeEqualFilter(EntryType value) =>
            $"{nameof(AuditEntryDto.EntryType)} eq '{value}'";
    }
}