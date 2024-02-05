using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.Risks.Events.Enums;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using AdminModels = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class ApplicationTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private EventsFixture _eventsFixture;
        private UserRiskLevelFixture _riskLevelFixture;
        private TaskFixture _taskFixture;
        private ChecksFixture _checksFixture;
        private ExternalProfileFixture _externalProfileFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _riskLevelFixture = Resolve<UserRiskLevelFixture>();
            _taskFixture = Resolve<TaskFixture>();
            _checksFixture = Resolve<ChecksFixture>();
            _externalProfileFixture = Resolve<ExternalProfileFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<GbUserInfoArbitrary>();
            Arb.Register<GlobalUserInfoArbitrary>();
            Arb.Register<BlackListCountryArbitrary>();
        }

        /// <summary>
        /// Scenario: Approve application (Task complete)
        /// Given user with applied application
        /// And with evaluated risk level
        /// When last task is completed
        /// Then application is approved
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldApproveApplication_WhenCompleteLastTask(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _riskLevelFixture.ChangeRiskLevelAsync(RiskRating.Low, userId);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            // Act
            var taskIds = application.RequiredTasks.Select(task => task.Id).ToArray();
            await _applicationFixture.CompeteTasksAsync(userId, taskIds, seed);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId && 
                                                                                                 e.EventArgs.NewState == ApplicationState.Approved);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Applied);
                @event.EventArgs.DecisionReasons.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Approve application (Risk level evaluated)
        /// Given user with applied application
        /// And all tasks are completed
        /// When risk level is evaluated
        /// Then application is approved
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldApproveApplication_WhenRiskLevelEvaluated(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            var taskIds = application.RequiredTasks.Select(task => task.Id).ToArray();
            await _applicationFixture.CompeteTasksAsync(userId, taskIds, seed);

            // Act
            await _riskLevelFixture.ChangeRiskLevelAsync(RiskRating.Low, userId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                                 e.EventArgs.NewState == ApplicationState.Approved);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Applied);
                @event.EventArgs.DecisionReasons.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Not approve application (Manual task)
        /// Given user with applied application
        /// And all tasks are completed except manual one
        /// When risk level is evaluated
        /// Then application is not approved
        /// And event is not raised
        /// </summary>
        [Theory, Ignore("TODO: Think how to implement")]
        public async Task ShouldNotApproveApplication_WhenManualTask(GlobalUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await client.Tasks.GetAllAsync(userId);
            var taskIds = tasks.Where(task => task.Type != AdminModels.TaskType.Address)
                               .Select(task => task.Id)
                               .ToArray();
            await _applicationFixture.CompeteTasksAsync(userId, taskIds, seed);

            // Act
            await _riskLevelFixture.ChangeRiskLevelAsync(RiskRating.Low, userId);

            // Assert
            _eventsFixture.ShouldNotExist<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId &&
                                                                             e.EventArgs.NewState == ApplicationState.Approved);
        }

        /// <summary>
        /// Scenario: Move application in review (Task incomplete)
        /// Given user with approved application
        /// When task became incomplete
        /// Then application is in review
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldMoveApplicationInReview_WhenTaskBecameIncomplete(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var faker = FakerFactory.Create(seed);
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var taskToIncomplete = faker.PickRandom(tasks);

            // Act
            await _taskFixture.IncompleteAsync(userId, taskToIncomplete.Id);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId && 
                                                                                                 e.EventArgs.NewState == ApplicationState.InReview);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Approved);
                @event.EventArgs.DecisionReasons.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Scenario: Reject application (Fraud)
        /// Given user with applied application
        /// When check is failed with check decision 'Fraud'
        /// Then application is rejected
        /// And decision reason is 'Fraud'
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRejectApplication_WhenUserFraud(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(AdminModels.CheckType.IdentityDocument, CheckDecisions.Fraud);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminApiClient = _adminApiClientFactory.Create(admin);
            var application = await adminApiClient.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            var checks = await adminApiClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(IsIdentityDocumentCheck));
            var check = checks.First(IsIdentityDocumentCheck);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                                 e.EventArgs.NewState == ApplicationState.Rejected);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Applied);
                @event.EventArgs.DecisionReasons.Should().HaveCount(1);
                @event.EventArgs.DecisionReasons.Should().HaveElementAt(0, ApplicationDecisions.Fraud);
            }

            bool IsIdentityDocumentCheck(AdminModels.CheckDto checkDto) =>
                checkDto.Type == AdminModels.CheckType.IdentityDocument && checkDto.Variant.Provider == AdminModels.CheckProviderType.Onfido;
        }
        
        /// <summary>
        /// Scenario: Reject application (Duplicate account)
        /// Given user with applied application
        /// When check is failed with check decision 'Duplicate account'
        /// Then application is rejected
        /// And decision reason is 'Duplicate account'
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRejectApplication_WhenUserHasDuplicateAccount(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(AdminModels.CheckType.FaceDuplication, CheckDecisions.DuplicateAccount);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            var checks = await client.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(IsFaceDuplicationCheck));
            var check = checks.First(IsFaceDuplicationCheck);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                             e.EventArgs.NewState == ApplicationState.Rejected);

            using var _ = new AssertionScope();
            @event.EventArgs.ApplicationId.Should().Be(applicationId);
            @event.EventArgs.PreviousState.Should().Be(ApplicationState.Applied);
            @event.EventArgs.DecisionReasons.Should().HaveCount(1);
            @event.EventArgs.DecisionReasons.Should().HaveElementAt(0, ApplicationDecisions.DuplicateAccount);

            check = await client.Checks.GetAsync(userId, check.Id);
            check.State.Should().Be(AdminModels.CheckState.Complete);
            check.Result.Should().Be(AdminModels.CheckResult.Failed);
            check.Decision.Should().Be(ApplicationDecisions.DuplicateAccount);

            bool IsFaceDuplicationCheck(AdminModels.CheckDto checkDto) =>
                checkDto.Type == AdminModels.CheckType.FaceDuplication && checkDto.Variant.Provider == AdminModels.CheckProviderType.Onfido;
        }

        /// <summary>
        /// Scenario: Reject application (Blacklisted country)
        /// Given user with applied application
        /// When poi issuing country is blacklisted country
        /// Then application is rejected
        /// And decision reason is 'Fraud'
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRejectApplication_WhenUserFromBlacklistedCountry(UserInfo userInfo,
                                                                                 BlackListCountry blackListCountry,
                                                                                 Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Passed(AdminModels.CheckType.IdentityDocument)
                                               .WithPoiIssuingCountry(blackListCountry.CountryCode);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            var checks = await client.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(IsIdentityDocumentCheck));

            var check = checks.First(IsIdentityDocumentCheck);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                                e.EventArgs.NewState
                                                                                             == ApplicationState.Rejected);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Applied);
                @event.EventArgs.DecisionReasons.Should().HaveCount(1);
                @event.EventArgs.DecisionReasons.Should().HaveElementAt(0, ApplicationDecisions.Fraud);
            }

            bool IsIdentityDocumentCheck(AdminModels.CheckDto checkDto) => 
                checkDto.Type == AdminModels.CheckType.IdentityDocument && checkDto.Variant.Provider == AdminModels.CheckProviderType.Onfido;
        }

        /// <summary>
        /// Scenario: Reject application (Rejection policy)
        /// Given user from GB
        /// And user has applied application
        /// When user risk level is evaluated
        /// And user is ExtraHigh risk 
        /// Then application is rejected
        /// And decision reasons is not empty
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRejectApplicationAccordingToRejectionPolicy(GbUserInfo userInfo)
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
            await _riskLevelFixture.ChangeRiskLevelAsync(RiskRating.ExtraHigh, userId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                                 e.EventArgs.NewState == ApplicationState.Rejected);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Applied);
                @event.EventArgs.DecisionReasons.Should().HaveCount(1);
            }
        }

        /// <summary>
        /// Scenario: Add conditional tasks when application risk level is high 
        /// Given user from GB with completed tasks
        /// When risk level evaluated
        /// And risk level is high
        /// Then conditional tasks must be added to application
        /// And application state is applied
        /// </summary>
        [Theory]
        public async Task ShouldAddConditionalTasks_WhenRiskLevelIsHighRisk(GbUserInfo userInfo, Seed seed, Guid correlationId)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);

            var taskIds = application.RequiredTasks.Select(task => task.Id).ToArray();
            await _applicationFixture.CompeteTasksAsync(userId, taskIds, seed);

            // Act
            await _riskLevelFixture.ChangeRiskLevelAsync(RiskRating.High, userId, correlationId);

            // Assert
            _eventsFixture.ShouldExistSingle<TaskCreatedEvent>(@event => @event.EventArgs.Type == TaskType.FinancialCondition
                                                                      && @event.CorrelationId == correlationId);
            _eventsFixture.ShouldExistSingle<TaskCreatedEvent>(@event => @event.EventArgs.Type == TaskType.ProofOfFunds
                                                                      && @event.CorrelationId == correlationId);
            _eventsFixture.ShouldExist<ApplicationRequiredTaskAddedEvent>(correlationId);
            _eventsFixture.ShouldExist<TaskCollectionStepAddedEvent>(correlationId);
            _eventsFixture.ShouldExist<TaskIncompleteEvent>(@event => @event.EventArgs.Type == TaskType.Address
                                                                      && @event.CorrelationId == correlationId);

            var actualApplication = await client.Applications.GetDefaultAsync(userId);
            actualApplication.State.Should().Be(AdminModels.ApplicationState.Applied);
        }

        /// <summary>
        /// Scenario: Application must be approved when conditional tasks passed
        /// Given user from GB with completed tasks
        /// And risk level is high
        /// And conditional tasks created
        /// When conditional tasks completed
        /// Then application must be approved
        /// </summary>
        [Theory]
        public async Task ShouldApproveApplication_WhenConditionalTaskCompleted(GbUserInfo userInfo, Seed seed, Guid correlationId)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            var taskIds = application.RequiredTasks.Select(task => task.Id).ToArray();
            await _applicationFixture.CompeteTasksAsync(userId, taskIds, seed);
            await _riskLevelFixture.ChangeRiskLevelAsync(RiskRating.High, userId, correlationId);

            WaitUntilTriggerCompleted("222E9D58-CACB-4094-9399-2AA50AA2D766");
            WaitUntilTriggerCompleted("AF4321CB-E246-479A-9F13-84AC8942C078");
            WaitUntilTriggerCompleted("D064BF48-8B8C-44C7-851C-59A69E78C37A");

            //Arrange
            var actualApplication = await client.Applications.GetDefaultAsync(userId);
            var requiredTasks = actualApplication.RequiredTasks
                .Where(task => task.State == AdminModels.TaskState.Incomplete)
                .Select(dto => dto.Id)
                .ToArray();

            // Act
            await _applicationFixture.CompeteTasksAsync(userId, requiredTasks, seed);

            // Assert
            _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(@event => @event.EventArgs.ApplicationId == application.Id);
            var applicationAfterChangingState = await client.Applications.GetDefaultAsync(userId);
            applicationAfterChangingState.State.Should().Be(AdminModels.ApplicationState.Approved);

            void WaitUntilTriggerCompleted(string variantId) =>
                _eventsFixture.ShouldExistSingle<TriggerCompletedEvent>(
                @event => @event.EventArgs.VariantId == new Guid(variantId) && @event.CorrelationId == correlationId);
        }
    }
}
