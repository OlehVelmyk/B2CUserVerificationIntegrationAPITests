using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Enums;
using WX.B2C.User.Verification.Events.Events;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    internal class ApplicationTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private ProfileFixture _profileFixture;
        private TaskFixture _taskFixture;
        private EventsFixture _eventsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _taskFixture = Resolve<TaskFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();

            Arb.Register<UserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Get details for applied application by id
        /// Given user with applied application
        /// And application has not passed tasks
        /// When admin requests application assigned to user by id
        /// Then he gets the details on that application
        /// And application state is applied
        /// And required tasks are included
        /// And allowed actions contains Reject action
        /// </summary>
        [Theory]
        public async Task ShouldGetAppliedApplication(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetAsync(userId, applicationId);

            // Assert
            var expectedAllowedActions = new[] { AdminApi.ApplicationAction.Reject };
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.Applied);
                actual.AllowedActions.Should().BeEquivalentTo(expectedAllowedActions);
                actual.DecisionReasons.Should().BeEmpty();
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get details for applied application by id
        /// Given user with applied application
        /// And all application tasks are passed
        /// When admin requests application assigned to user by id
        /// Then he gets the details on that application
        /// And application state is applied
        /// And required tasks are included
        /// And allowed actions contains Reject and Approve actions
        /// </summary>
        [Theory]
        public async Task ShouldGetAppliedApplicationWithApproveAction(UserInfo userInfo, Seed seed)
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

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetAsync(userId, applicationId);

            // Assert
            var expectedAllowedActions = new[] { AdminApi.ApplicationAction.Reject, AdminApi.ApplicationAction.Approve };
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.Applied);
                actual.AllowedActions.Should().BeEquivalentTo(expectedAllowedActions);
                actual.DecisionReasons.Should().BeEmpty();
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get details for application in review by id
        /// Given user with application in review
        /// And all application task are passed
        /// When admin requests application assigned to user by id
        /// Then he gets the details on that application
        /// And application state is in review
        /// And required tasks are included
        /// And allowed actions contains Approve and Reject actions
        /// </summary>
        [Theory]
        public async Task ShouldGetInReviewApplicationWithApproveAction(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);
            await _applicationFixture.RequestReviewAsync(userId, applicationId);

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetAsync(userId, applicationId);

            // Assert
            var expectedAllowedActions = new[] { AdminApi.ApplicationAction.Approve, AdminApi.ApplicationAction.Reject };
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.InReview);
                actual.AllowedActions.Should().BeEquivalentTo(expectedAllowedActions);
                actual.DecisionReasons.Should().BeEmpty();
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get details for application in review by id
        /// Given user with application in review
        /// And not all application tasks are passed
        /// When admin requests application assigned to user by id
        /// Then he gets the details on that application
        /// And application state is in review
        /// And required tasks are included
        /// And allowed actions contains Reject actions
        /// </summary>
        [Theory]
        public async Task ShouldGetInReviewApplication(UserInfo userInfo, Seed seed)
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
            await _applicationFixture.RequestReviewAsync(userId, applicationId);

            var tasks = await client.Tasks.GetAllAsync(userId);
            var taskToIncomplete = faker.PickRandom(tasks);
            await _taskFixture.IncompleteAsync(userId, taskToIncomplete.Id);

            // Arrange
            tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetAsync(userId, applicationId);

            // Assert
            var expectedAllowedActions = new[] { AdminApi.ApplicationAction.Reject };
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.InReview);
                actual.AllowedActions.Should().BeEquivalentTo(expectedAllowedActions);
                actual.DecisionReasons.Should().BeEmpty();
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get details for approved application by id
        /// Given user with approved application
        /// When admin requests application assigned to user by id
        /// Then he gets the details on that application
        /// And application state is approved
        /// And required tasks are included
        /// And allowed actions Review and Reject included
        /// </summary>
        [Theory]
        public async Task ShouldGetApprovedApplication(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetAsync(userId, applicationId);

            // Assert
            var expectedAllowedActions = new[] { AdminApi.ApplicationAction.Review, AdminApi.ApplicationAction.Reject };
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.Approved);
                actual.AllowedActions.Should().BeEquivalentTo(expectedAllowedActions);
                actual.DecisionReasons.Should().BeEmpty();
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get details for rejected application by id
        /// Given user with rejected application
        /// When admin requests application assigned to user by id
        /// Then he gets the details on that application
        /// And application state is rejected
        /// And required tasks are included
        /// And allowed actions contains RevertDecision Action
        /// And decisions are included
        /// </summary>
        [Theory]
        public async Task ShouldGetRejectedApplication(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.RejectAsync(userId, applicationId);

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetAsync(userId, applicationId);

            // Assert
            var expectedAllowedActions = new[] { AdminApi.ApplicationAction.RevertDecision };
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.Rejected);
                actual.AllowedActions.Should().BeEquivalentTo(expectedAllowedActions);
                actual.DecisionReasons.Should().HaveCount(1);
                actual.DecisionReasons.Should().HaveElementAt(0, nameof(ShouldGetRejectedApplication));
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get details for cancelled application by id
        /// Given user with rejected application
        /// When admin requests application assigned to user by id
        /// Then he gets the details on that application
        /// And application state is cancelled
        /// And required tasks are included
        /// And allowed actions contains RevertDecision Action
        /// And decisions are included
        /// </summary>
        [Theory]
        public async Task ShouldGetCancelledApplication(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);
            await _applicationFixture.CancelAsync(userId, applicationId);

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetAsync(userId, applicationId);

            // Assert
            var expectedAllowedActions = new[] { AdminApi.ApplicationAction.RevertDecision };
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.Cancelled);
                actual.AllowedActions.Should().BeEquivalentTo(expectedAllowedActions);
                actual.DecisionReasons.Should().HaveCount(1);
                actual.DecisionReasons.Should().HaveElementAt(0, nameof(ShouldGetCancelledApplication));
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get details for non-existent application
        /// Given user without assigned application
        /// When admin requests application by dummy id
        /// Then he receives error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenApplicationNotExist(UserInfo userInfo, Guid applicationId)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            Func<Task> getApplication = () => client.Applications.GetAsync(userId, applicationId);

            // Assert
            var exception = await getApplication.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Get details for application that`s assigned to another user
        /// Given two users with assigned applications
        /// When admin requests task by user id of another user
        /// Then he receives error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenApplicationOfOtherUser(UserInfo first, UserInfo second)
        {
            // Given
            var firstUserId = first.UserId.Dump();
            var secondUserId = second.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(first);
            await _applicationFixture.BuildApplicationAsync(second);

            // Arrange
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            var secondUserApplication = await client.Applications.GetDefaultAsync(secondUserId);
            var secondUserApplicationId = secondUserApplication.Id;

            // Act
            Func<Task> getApplication = () => client.Applications.GetAsync(firstUserId, secondUserApplicationId);

            // Assert
            var exception = await getApplication.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Get details for default application
        /// Given user with application
        /// When admin requests default application assigned to user
        /// Then he gets the details on that application
        /// And application state is present
        /// And required tasks are included
        /// And allowed actions are included
        /// </summary>
        [Theory]
        public async Task ShouldGetDefaultApplication(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var actual = await client.Applications.GetDefaultAsync(userId);

            // Assert
            using (new AssertionScope())
            {
                actual.State.Should().Be(AdminApi.ApplicationState.Applied);
                actual.AllowedActions.Should().BeEquivalentTo(new[] { AdminApi.ApplicationAction.Reject });
                actual.DecisionReasons.Should().BeEmpty();
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get all user applications
        /// Given user with assigned application
        /// When admin requests applications assigned to user
        /// Then he receives only one application
        /// And application belongs to requested user
        /// </summary>
        [Theory]
        public async Task ShouldGetApplications(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var tasks = await client.Tasks.GetAllAsync(userId);
            var expectedTasks = tasks.Select(TaskMappersExtensions.Map);

            // Act
            var applications = await client.Applications.GetAllAsync(userId);

            // Assert
            using (new AssertionScope())
            {
                applications.Should().HaveCount(1);
                var actual = applications.First();
                actual.State.Should().Be(AdminApi.ApplicationState.Applied);
                actual.AllowedActions.Should().BeEquivalentTo(new[] { AdminApi.ApplicationAction.Reject });
                actual.DecisionReasons.Should().BeEmpty();
                actual.RequiredTasks.Should().BeEquivalentTo(expectedTasks);
            }
        }

        /// <summary>
        /// Scenario: Get all user applications (Absent)
        /// Given user without assigned applications
        /// When admin requests applications assigned to user
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyArray_WhenUserHasNoApplications(UserInfo userInfo)
        {
            // Given
            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);

            // Act
            var applications = await client.Applications.GetAllAsync(userId);

            // Assert
            applications.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Approve application
        /// Given user with assigned application
        /// And application state is in review
        /// And all tasks are completed
        /// When admin approves application
        /// Then application state is approved
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldApproveApplication(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);
            await _applicationFixture.RequestReviewAsync(userId, applicationId);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldApproveApplication));

            // Act
            await client.Applications.ApproveAsync(reason, userId, applicationId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(correlationId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UserId.Should().Be(userId);
                @event.EventArgs.NewState.Should().Be(ApplicationState.Approved);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.InReview);
                @event.EventArgs.Initiation.Reason.Should().Be(reason.Reason);
                @event.EventArgs.DecisionReasons.Should().BeEmpty();

                var actualApplication = await client.Applications.GetAsync(userId, applicationId);
                actualApplication.State.Should().Be(AdminApi.ApplicationState.Approved);
            }
        }

        /// <summary>
        /// Scenario: Approve application (Inappropriate state)
        /// Given user with assigned application
        /// And application state is rejected
        /// And all tasks are completed
        /// When admin approves application
        /// Then he receives error response with status code "Conflict"
        /// And application state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenApproveApplicationWithInappropriateState(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            var taskIds = application.RequiredTasks.Select(task => task.Id).ToArray();
            await _applicationFixture.CompeteTasksAsync(userId, taskIds, seed);
            await _applicationFixture.RejectAsync(userId, applicationId);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldGetError_WhenApproveApplicationWithInappropriateState));

            // Act
            Func<Task> approveApplication = () => client.Applications.ApproveAsync(reason, userId, applicationId);

            // Assert
            var exception = await approveApplication.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            _eventsFixture.ShouldNotExist<ApplicationStateChangedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Approve application (Incomplete tasks)
        /// Given user with assigned application
        /// And application state is applied
        /// And not all tasks are completed
        /// When admin approves application
        /// Then he receives error response with status code "Conflict"
        /// And application state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenApproveApplicationWithIncompleteTask(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var reason = new AdminApi.ReasonDto(nameof(ShouldGetError_WhenApproveApplicationWithIncompleteTask));

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            // Act
            Func<Task> approveApplication = () => client.Applications.ApproveAsync(reason, userId, applicationId);

            // Assert
            var exception = await approveApplication.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            _eventsFixture.ShouldNotExist<ApplicationStateChangedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Request review for application
        /// Given user with assigned application
        /// And application state is approved
        /// When admin requests review for application
        /// Then application state is in review
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldMoveApplicationToInReview(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldMoveApplicationToInReview));

            // Act
            await client.Applications.RequestReviewAsync(reason, userId, applicationId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(correlationId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UserId.Should().Be(userId);
                @event.EventArgs.NewState.Should().Be(ApplicationState.InReview);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Approved);
                @event.EventArgs.Initiation.Reason.Should().Be(reason.Reason);
                @event.EventArgs.DecisionReasons.Should().BeEmpty();

                var actualApplication = await client.Applications.GetAsync(userId, applicationId);
                actualApplication.State.Should().Be(AdminApi.ApplicationState.InReview);
            }
        }

        /// <summary>
        /// Scenario: Request review for application (Inappropriate state)
        /// Given user with assigned application
        /// And application state is applied
        /// When admin requests review for application
        /// Then he receives error response with status code "Conflict"
        /// And application state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenMoveApplicationInReviewWithInappropriateState(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var reason = new AdminApi.ReasonDto(nameof(ShouldMoveApplicationToInReview));

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            // Act
            Func<Task> requestReview = () => client.Applications.RequestReviewAsync(reason, userId, applicationId);

            // Assert
            var exception = await requestReview.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            _eventsFixture.ShouldNotExist<ApplicationStateChangedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Reject application
        /// Given user with assigned application
        /// And application state is applied
        /// When admin rejects application
        /// Then application state is rejected
        /// And decision is present
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRejectApplication(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            var reason = new AdminApi.ReasonDto(nameof(ShouldRejectApplication));

            // Act
            await client.Applications.RejectAsync(reason, userId, applicationId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(correlationId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UserId.Should().Be(userId);
                @event.EventArgs.NewState.Should().Be(ApplicationState.Rejected);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Applied);
                @event.EventArgs.Initiation.Reason.Should().Be(reason.Reason);
                @event.EventArgs.DecisionReasons.Should().HaveCount(1);
                @event.EventArgs.DecisionReasons.Should().HaveElementAt(0, reason.Reason);

                var actualApplication = await client.Applications.GetAsync(userId, applicationId);
                actualApplication.State.Should().Be(AdminApi.ApplicationState.Rejected);
            }
        }

        /// <summary>
        /// Scenario: Reject application (already rejected)
        /// Given user with assigned application
        /// And application state is rejected
        /// When admin rejects application
        /// Then application state is rejected
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldDoNothing_WhenApplicationRejected(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.RejectAsync(userId, applicationId);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldDoNothing_WhenApplicationRejected));

            // Act
            await client.Applications.RejectAsync(reason, userId, applicationId);

            // Assert
            _eventsFixture.ShouldNotExist<ApplicationStateChangedEvent>(correlationId);
            var actualApplication = await client.Applications.GetAsync(userId, applicationId);
            actualApplication.State.Should().Be(AdminApi.ApplicationState.Rejected);
        }

        /// <summary>
        /// Scenario: Cancel application
        /// Given user with assigned application
        /// And application state is approved or in review
        /// When admin rejects application
        /// Then application state is cancelled
        /// And decision is present
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCancelApplication(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldCancelApplication));

            // Act
            await client.Applications.RejectAsync(reason, userId, applicationId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(correlationId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UserId.Should().Be(userId);
                @event.EventArgs.NewState.Should().Be(ApplicationState.Cancelled);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Approved);
                @event.EventArgs.Initiation.Reason.Should().Be(reason.Reason);
                @event.EventArgs.DecisionReasons.Should().HaveCount(1);
                @event.EventArgs.DecisionReasons.Should().HaveElementAt(0, reason.Reason);

                var actualApplication = await client.Applications.GetAsync(userId, applicationId);
                actualApplication.State.Should().Be(AdminApi.ApplicationState.Cancelled);
            }
        }

        /// <summary>
        /// Scenario: Revert application reject decision
        /// Given user with assigned application
        /// And application state is rejected 
        /// And previous state is applied
        /// And decision is present
        /// When admin requests revert application decision
        /// Then application state is applied
        /// And decision is absent
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRevertRejectDecision(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.RejectAsync(userId, applicationId);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldRevertRejectDecision));

            // Act
            await client.Applications.RevertDecisionAsync(reason, userId, applicationId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(correlationId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UserId.Should().Be(userId);
                @event.EventArgs.NewState.Should().Be(ApplicationState.Applied);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Rejected);
                @event.EventArgs.Initiation.Reason.Should().Be(reason.Reason);
                @event.EventArgs.DecisionReasons.Should().BeEmpty();

                var actualApplication = await client.Applications.GetAsync(userId, applicationId);
                actualApplication.State.Should().Be(AdminApi.ApplicationState.Applied);
            }
        }

        /// <summary>
        /// Scenario: Revert application cancel decision
        /// Given user with assigned application
        /// And application state is cancelled 
        /// And previous state is approved or in review
        /// And decision is present
        /// When admin requests revert application decision
        /// Then application state is approved or in review
        /// And decision is absent
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRevertCancelDecision(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;
            await _applicationFixture.ApproveAsync(userId, applicationId, seed);
            await _applicationFixture.CancelAsync(userId, applicationId);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldRevertCancelDecision));

            // Act
            await client.Applications.RevertDecisionAsync(reason, userId, applicationId);

            // Assert
            using (new AssertionScope())
            {
                var @event = _eventsFixture.ShouldExistSingle<ApplicationStateChangedEvent>(correlationId);
                @event.EventArgs.ApplicationId.Should().Be(applicationId);
                @event.EventArgs.UserId.Should().Be(userId);
                @event.EventArgs.NewState.Should().Be(ApplicationState.Approved);
                @event.EventArgs.PreviousState.Should().Be(ApplicationState.Cancelled);
                @event.EventArgs.Initiation.Reason.Should().Be(reason.Reason);
                @event.EventArgs.DecisionReasons.Should().BeEmpty();

                var actualApplication = await client.Applications.GetAsync(userId, applicationId);
                actualApplication.State.Should().Be(AdminApi.ApplicationState.Approved);
            }
        }

        /// <summary>
        /// Scenario: Revert application reject decision (Inappropriate state)
        /// Given user with assigned application
        /// And application state is not rejected or cancelled
        /// When admin requests revert application decision
        /// Then he receives error response with status code "Conflict"
        /// And application state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenRevertDecisionWithInappropriateState(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var reason = new AdminApi.ReasonDto(nameof(ShouldGetError_WhenRevertDecisionWithInappropriateState));

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            // Act
            Func<Task> revertDecision = () => client.Applications.RevertDecisionAsync(reason, userId, applicationId);

            // Assert
            var exception = await revertDecision.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            _eventsFixture.ShouldNotExist<ApplicationStateChangedEvent>(correlationId);
        }

        /// <summary>
        /// Scenario: Move cancelled application in approved state
        /// Given user with assigned application
        /// And application state is cancelled
        /// When admin approves application
        /// Then he receives error response with status code "Conflict"
        /// And application state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenApproveCancelledApplication(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var application = await client.Applications.GetDefaultAsync(userId);
            var applicationId = application.Id;

            await _applicationFixture.ApproveAsync(userId, applicationId, seed);
            await _applicationFixture.CancelAsync(userId, applicationId);

            // Arrange
            var reason = new AdminApi.ReasonDto(nameof(ShouldGetError_WhenApproveCancelledApplication));

            // Act
            Func<Task> approveApplication = () => client.Applications.ApproveAsync(reason, userId, applicationId);

            // Assert
            var exception = await approveApplication.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            _eventsFixture.ShouldNotExist<ApplicationStateChangedEvent>(correlationId);
        }
    }
}
