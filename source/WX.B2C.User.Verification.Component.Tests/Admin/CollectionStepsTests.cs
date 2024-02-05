using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using AdminModels = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    internal class CollectionStepsTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private SurveyStepFixture _surveyStepFixture;
        private ProfileFixture _profileFixture;
        private EventsFixture _eventsFixture;
        private TaskFixture _taskFixture;
        private CollectionStepsFixture _collectionStepsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _adminFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _surveyStepFixture = Resolve<SurveyStepFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _taskFixture = Resolve<TaskFixture>();
            _collectionStepsFixture = Resolve<CollectionStepsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _adminFactory = Resolve<AdministratorFactory>();

            Arb.Register<UserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Receive all user collection steps
        /// Given user with several collection steps
        /// When collection steps are requested by userId
        /// Then receive it
        /// And it contain info about related tasks
        /// And it contain collection step data
        /// </summary>
        [Theory]
        public async Task ShouldGetAllUserCollectionSteps(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            var steps = await adminClient.CollectionStep.GetAllAsync(userId);

            // Assert
            steps.Should().NotBeEmpty();
            foreach (var step in steps)
            {
                using var scope = new AssertionScope();

                var isStepCompleted = step.State == CollectionStepState.Completed;

                step.Should().NotBeNull();
                step.Id.Should().NotBeEmpty();
                step.State.Should().HaveValue();
                step.ReviewResult.Should().BeNull();
                step.RelatedTasks.Should().NotBeEmpty();
                step.RequestedAt.Should().NotBe(default);
                step.UpdatedAt.Should().NotBe(default);
                step.Data.Should().BeSomeOrNull(_ => isStepCompleted);
                step.Variant.Should().NotBeNull();
                step.Variant.Should().BeAssignableTo<CollectionStepVariantDto>();
                step.Variant.Name.Should().NotBeNull();
            }
        }

        /// <summary>
        /// Scenario: Receive all user collection steps
        /// Given user without collection steps
        /// When collection steps are requested by userId
        /// Then receive empty collection
        /// And status code is successful
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyCollection_WhenUserDoNotHaveCollectionSteps(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            var steps = await adminClient.CollectionStep.GetAllAsync(userId);

            // Assert
            steps.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Receive collection step
        /// Given user with several collection steps
        /// When collection steps are requested by user id and collection step id
        /// Then receive it
        /// And it contain info about related tasks
        /// And it contain collection step data
        /// </summary>
        [Theory]
        public async Task ShouldGetCollectionStep(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var stepEvent = _eventsFixture.ShouldExist<CollectionStepRequestedEvent>(e => e.EventArgs.UserId == userId);
            var stepId = stepEvent.EventArgs.CollectionStepId;

            // Act
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);

            // Assert
            using var scope = new AssertionScope();

            var isStepCompleted = step.State == CollectionStepState.Completed;

            step.Should().NotBeNull();
            step.Id.Should().NotBeEmpty();
            step.State.Should().HaveValue();
            step.ReviewResult.Should().BeNull();
            step.RelatedTasks.Should().NotBeEmpty();
            step.RequestedAt.Should().NotBe(default);
            step.UpdatedAt.Should().NotBe(default);
            step.Data.Should().BeSomeOrNull(_ => isStepCompleted);
            step.Variant.Should().NotBeNull();
            step.Variant.Should().BeAssignableTo<CollectionStepVariantDto>();
            step.Variant.Name.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Receive collection step
        /// Given user have several collection steps
        /// When collection steps are requested by userId and collection step id
        /// And collection step do not exists
        /// Then receive error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCollectionStepDoNotExists(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var stepId = Guid.NewGuid();

            // Act
            Func<Task> getStep = () => adminClient.CollectionStep.GetAsync(userId, stepId);

            // Assert
            var exception = await getStep.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Admin request a document collection step
        /// Given user
        /// When collection step is requested
        /// And collection step type is document
        /// And user do not have collection steps with same xpath
        /// Then new collection step is created
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCreateDocumentCollectionStep(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            // Arrange
            var request = new DocumentCollectionStepRequest
            {
                Reason = nameof(ShouldCreateDocumentCollectionStep),
                Type = CollectionStepType.Document,
                IsRequired = false,
                IsReviewNeeded = false,
                TargetTasks = new[] { targetTask },
                DocumentCategory = DocumentCategory.Supporting
            };

            // Act
            await adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(adminClient.CorrelationId);
            var stepId = @event.EventArgs.CollectionStepId;
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);

            using var scope = new AssertionScope();

            step.Should().NotBeNull();
            step.Id.Should().Be(stepId);
            step.IsRequired.Should().Be(request.IsRequired);
            step.IsReviewNeeded.Should().Be(request.IsReviewNeeded);
            step.State.Should().Be(CollectionStepState.Requested);
            step.ReviewResult.Should().BeNull();
            step.RelatedTasks.Should().BeEquivalentTo(request.TargetTasks);
            step.RequestedAt.Should().NotBe(default);
            step.UpdatedAt.Should().NotBe(default);
            step.Data.Should().BeNull();
            step.Variant.Should().NotBeNull();
            step.Variant.Should().BeOfType<DocumentCollectionStepVariantDto>();
            step.Variant.Name.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Admin request a survey collection step
        /// Given user
        /// When collection step is requested
        /// And collection step type is survey
        /// And user do not have collection steps with same xpath
        /// Then new collection step is created
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCreateSurveyCollectionStep(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            // Arrange
            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldCreateSurveyCollectionStep),
                Type = CollectionStepType.Survey,
                IsRequired = false,
                IsReviewNeeded = false,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { targetTask }
            };

            // Act
            await adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(adminClient.CorrelationId);
            var stepId = @event.EventArgs.CollectionStepId;
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);

            using var scope = new AssertionScope();

            step.Should().NotBeNull();
            step.Id.Should().Be(stepId);
            step.IsRequired.Should().Be(request.IsRequired);
            step.IsReviewNeeded.Should().Be(request.IsReviewNeeded);
            step.State.Should().Be(CollectionStepState.Requested);
            step.ReviewResult.Should().BeNull();
            step.RelatedTasks.Should().BeEquivalentTo(request.TargetTasks);
            step.RequestedAt.Should().NotBe(default);
            step.UpdatedAt.Should().NotBe(default);
            step.Data.Should().BeNull();
            step.Variant.Should().NotBeNull();
            step.Variant.Should().BeOfType<SurveyCollectionStepVariantDto>();
            step.Variant.Name.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Admin request a verification details collection step
        /// Given user
        /// When collection step is requested
        /// And collection step type is verification details
        /// And user do not have collection steps with same xpath
        /// Then new collection step is created
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCreateVerificationDetailsCollectionStep(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            // Arrange
            var request = new VerificationDetailsCollectionStepRequest
            {
                Reason = nameof(ShouldCreateVerificationDetailsCollectionStep),
                Type = CollectionStepType.VerificationDetails,
                IsRequired = false,
                IsReviewNeeded = false,
                TargetTasks = new[] { targetTask },
                VerificationProperty = VerificationDetailsProperty.IpAddress
            };

            // Act
            await adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(adminClient.CorrelationId);
            var stepId = @event.EventArgs.CollectionStepId;
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);

            using var scope = new AssertionScope();

            step.Should().NotBeNull();
            step.Id.Should().Be(stepId);
            step.IsRequired.Should().Be(request.IsRequired);
            step.IsReviewNeeded.Should().Be(request.IsReviewNeeded);
            step.State.Should().Be(CollectionStepState.Requested);
            step.ReviewResult.Should().BeNull();
            step.RelatedTasks.Should().BeEquivalentTo(request.TargetTasks);
            step.RequestedAt.Should().NotBe(default);
            step.UpdatedAt.Should().NotBe(default);
            step.Data.Should().BeNull();
            step.Variant.Should().NotBeNull();
            step.Variant.Should().BeOfType<VerificationDetailsCollectionStepVariantDto>();
            step.Variant.Name.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Admin request a personal details collection step
        /// Given user
        /// When collection step is requested
        /// And collection step type is personal details
        /// And user do not have collection steps with same xpath
        /// Then new collection step is created
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCreatePersonalDetailsCollectionStep(UserInfo userInfo, AdminModels.PersonalDetailsProperty property)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            // Arrange
            var request = new PersonalDetailsCollectionStepRequest
            {
                Reason = nameof(ShouldCreatePersonalDetailsCollectionStep),
                Type = CollectionStepType.PersonalDetails,
                IsRequired = false,
                IsReviewNeeded = false,
                TargetTasks = new[] { targetTask },
                PersonalProperty = property
            };

            // Act
            await adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(adminClient.CorrelationId);
            var stepId = @event.EventArgs.CollectionStepId;
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);

            using var scope = new AssertionScope();

            step.Should().NotBeNull();
            step.Id.Should().Be(stepId);
            step.IsRequired.Should().Be(request.IsRequired);
            step.IsReviewNeeded.Should().Be(request.IsReviewNeeded);
            step.State.Should().Be(CollectionStepState.Requested);
            step.ReviewResult.Should().BeNull();
            step.RelatedTasks.Should().BeEquivalentTo(request.TargetTasks);
            step.RequestedAt.Should().NotBe(default);
            step.UpdatedAt.Should().NotBe(default);
            step.Data.Should().BeNull();
            step.Variant.Should().NotBeNull();
            step.Variant.Should().BeOfType<PersonalDetailsCollectionStepVariantDto>();
            step.Variant.Name.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Admin make collection step required
        /// Given user with collection step
        /// And it is not required
        /// When collection step is requested
        /// And requested collection step is required
        /// And collection step with requested xpath already exists
        /// Then collection step become required
        /// </summary>
        [Theory]
        public async Task ShouldMakeCollectionStepRequired_WhenCollectionStepAlreadyExists(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldMakeCollectionStepRequired_WhenCollectionStepAlreadyExists),
                Type = CollectionStepType.Survey,
                IsRequired = false,
                IsReviewNeeded = false,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { targetTask }
            };
            var stepId = await _collectionStepsFixture.RequestAsync(userId, request);

            // Arrange
            request.IsRequired = true;

            // Act
            await adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);
            step.IsRequired.Should().BeTrue();
        }

        /// <summary>
        /// Scenario: Admin make collection step review required
        /// Given user with collection step
        /// And it review is not required
        /// When collection step is requested
        /// And requested collection step review is required
        /// And collection step with requested xpath already exists
        /// Then collection step review become required
        /// </summary>
        [Theory]
        public async Task ShouldMakeReviewRequired_WhenCollectionStepAlreadyExists(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldMakeReviewRequired_WhenCollectionStepAlreadyExists),
                Type = CollectionStepType.Survey,
                IsRequired = false,
                IsReviewNeeded = false,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { targetTask }
            };
            var stepId = await _collectionStepsFixture.RequestAsync(userId, request);

            // Arrange
            request.IsReviewNeeded = true;

            // Act
            await adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);
            step.IsReviewNeeded.Should().BeTrue();
        }

        /// <summary>
        /// Scenario: Admin request a collection step (Not target tasks)
        /// Given user
        /// When collection step is requested
        /// And target tasks are absent
        /// Then receive error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenRequestStepWithoutTargetTasks(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            // Arrange
            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldGetError_WhenRequestStepWithoutTargetTasks),
                Type = CollectionStepType.Survey,
                IsRequired = false,
                IsReviewNeeded = false,
                TemplateId = Guid.NewGuid(),
                TargetTasks = Array.Empty<Guid>()
            };

            // Act
            Func<Task> requestStep = () => adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            var exception = await requestStep.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: Admin review collection step
        /// Given user with collection step
        /// And it review is required
        /// When collection step is reviewed
        /// Then collection step become completed
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldReviewCollectionStep(UserInfo userInfo, CollectionStepReviewResult reviewResult)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskEvent = _eventsFixture.ShouldExist<TaskCreatedEvent>(e => e.EventArgs.UserId == userId);
            var targetTask = taskEvent.EventArgs.TaskId;

            var templateId = Guid.NewGuid();
            var surveyRequest = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldReviewCollectionStep),
                Type = CollectionStepType.Survey,
                IsRequired = false,
                IsReviewNeeded = true,
                TemplateId = templateId,
                TargetTasks = new[] { targetTask }
            };

            var stepId = await _collectionStepsFixture.RequestAsync(userId, surveyRequest);
            await _surveyStepFixture.MoveInReviewAsync(userId, templateId);

            // Arrange
            var request = new ReviewCollectionStepRequest
            {
                ReviewResult = reviewResult,
                Reason = nameof(ShouldReviewCollectionStep)
            };

            // Act
            await adminClient.CollectionStep.ReviewAsync(request, userId, stepId);

            // Assert
            var step = await adminClient.CollectionStep.GetAsync(userId, stepId);

            using var scope = new AssertionScope();
            step.ReviewResult.Should().Be(reviewResult);
            step.State.Should().Be(CollectionStepState.Completed);
        }

        /// <summary>
        /// Scenario: Admin deletes requested collection step
        /// Given collection step assigned to user
        /// And step is in "Requested" state
        /// And step has no related tasks
        /// When admin deletes given step by id
        /// Then step is hard deleted
        /// And user has no steps with given id
        /// </summary>
        [Theory]
        public async Task ShouldDeleteCollectionStep(UserInfo userInfo, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var reason = nameof(ShouldDeleteCollectionStep);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requestedCollectionSteps = collectionSteps.Where(step => step.IsRequested());
            var collectionStep = faker.PickRandom(requestedCollectionSteps);
            await _taskFixture.RemoveCollectionStepAsync(userId, collectionStep);

            // Act
            var httpOperationResponse = await adminClient.CollectionStep.DeleteWithHttpMessagesAsync(new ReasonDto(reason), userId, collectionStep.Id);

            // Assert
            httpOperationResponse.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            collectionSteps.Should().NotContain(step => step.Id == collectionStep.Id);
        }

        /// <summary>
        /// Scenario: Admin deletes collection step with related tasks
        /// Given collection step assigned to user
        /// And step is in "Requested" state
        /// And step has several related tasks
        /// When admin deletes given step by id
        /// Then he receives error response with status code "Internal Server Error"
        /// And collection step with given id is not deleted
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenDeleteStepWithRelatedTasks(UserInfo userInfo, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var reason = nameof(ShouldGetError_WhenDeleteStepWithRelatedTasks);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requestedCollectionSteps = collectionSteps.Where(step => step.IsRequested());
            var collectionStep = faker.PickRandom(requestedCollectionSteps);

            // Act
            Func<Task> removeStep = () => adminClient.CollectionStep.DeleteAsync(new ReasonDto(reason), userId, collectionStep.Id);

            // Assert
            var exception = await removeStep.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            collectionSteps.Should().Contain(step => step.Id == collectionStep.Id);
        }

        /// <summary>
        /// Scenario: Admin deletes collection step in not "Requested" state
        /// Given collection step assigned to user
        /// And step is not in "Requested" state
        /// And step has no related tasks
        /// When admin deletes given step by id
        /// Then he receives error response with status code "Conflict"
        /// And collection step with given id is not deleted
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenDeleteStepNotInRequestedState(UserInfo userInfo, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var reason = nameof(ShouldGetError_WhenDeleteStepNotInRequestedState);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            var notRequestedSteps = collectionSteps.Where(step => !step.IsRequested());
            var collectionStep = faker.PickRandom(notRequestedSteps);
            await _taskFixture.RemoveCollectionStepAsync(userId, collectionStep);

            // Act
            Func<Task> removeStep = () => adminClient.CollectionStep.DeleteAsync(new ReasonDto(reason), userId, collectionStep.Id);

            // Assert
            var exception = await removeStep.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            collectionSteps.Should().Contain(step => step.Id == collectionStep.Id);
        }

        /// <summary>
        /// Scenario: Admin updates collection step
        /// Given collection step assigned to user
        /// And step is in "Requested" state
        /// When admin updates properties of collection step 
        /// Then collection step updated
        /// And event CollectionStepUpdated is raised
        /// </summary>
        [Theory]
        public async Task ShouldUpdateCollectionStep(UserInfo userInfo, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var reason = nameof(ShouldUpdateCollectionStep);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requestedCollectionSteps = collectionSteps.Where(step => step.IsRequested());
            var collectionStep = faker.PickRandom(requestedCollectionSteps);

            // Arrange
            var patchProperties = new Dictionary<string, bool?>
            {
                [nameof(UpdateCollectionStepRequest.IsReviewNeeded)] = !collectionStep.IsReviewNeeded,
                [nameof(UpdateCollectionStepRequest.IsRequired)] = !collectionStep.IsRequired
            };

            faker.ResetRandomly(patchProperties, patchProperties.Count - 1);

            var updateRequest = new UpdateCollectionStepRequest
            {
                Reason = reason,
                IsReviewNeeded = patchProperties[nameof(UpdateCollectionStepRequest.IsReviewNeeded)],
                IsRequired = patchProperties[nameof(UpdateCollectionStepRequest.IsRequired)]
            };

            // Act
            await adminClient.CollectionStep.UpdateAsync(updateRequest, userId, collectionStep.Id);

            // Assert
            var expectedIsRequired = updateRequest.IsRequired ?? collectionStep.IsRequired;
            var expectedIsReviewNeeded = updateRequest.IsReviewNeeded ?? collectionStep.IsReviewNeeded;

            var updatedStep = await adminClient.CollectionStep.GetAsync(userId, collectionStep.Id);
            updatedStep.Should().BeEquivalentTo(new
            {
                IsRequired = expectedIsRequired,
                IsReviewNeeded = expectedIsReviewNeeded,
                State = CollectionStepState.Requested,
                ReviewResult = (CollectionStepReviewResult?)null,
                collectionStep.RelatedTasks
            });

            var @event = _eventsFixture.ShouldExist<CollectionStepUpdatedEvent>(adminClient.CorrelationId);
            @event.EventArgs.Id.Should().Be(collectionStep.Id);
            @event.EventArgs.UserId.Should().Be(userId);
            @event.EventArgs.IsRequired.Should().Be(expectedIsRequired);
            @event.EventArgs.IsReviewNeeded.Should().Be(expectedIsReviewNeeded);
        }

        /// <summary>
        /// Scenario: Admin updates collection step in not "Requested" state
        /// Given collection step assigned to user
        /// And collection step is not in "Requested" state
        /// When admin updates properties of collection step
        /// Then he receives error response with status code "Conflict"
        /// And collection step not updated
        /// And event CollectionStepUpdated is not raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenUpdateStepWhichIsNotInRequestedState(UserInfo userInfo, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var reason = nameof(ShouldGetError_WhenUpdateStepWhichIsNotInRequestedState);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            var notRequestedSteps = collectionSteps.Where(step => !step.IsRequested());
            var collectionStep = faker.PickRandom(notRequestedSteps);

            // Arrange
            var patchProperties = new Dictionary<string, bool?>
            {
                [nameof(UpdateCollectionStepRequest.IsReviewNeeded)] = !collectionStep.IsReviewNeeded,
                [nameof(UpdateCollectionStepRequest.IsRequired)] = !collectionStep.IsRequired
            };
            faker.ResetRandomly(patchProperties, patchProperties.Count - 1);

            var updateRequest = new UpdateCollectionStepRequest
            {
                Reason = reason,
                IsReviewNeeded = patchProperties[nameof(UpdateCollectionStepRequest.IsReviewNeeded)],
                IsRequired = patchProperties[nameof(UpdateCollectionStepRequest.IsRequired)]
            };

            // Act
            Func<Task> updateStep = () => adminClient.CollectionStep.UpdateAsync(updateRequest, userId, collectionStep.Id);

            // Assert
            var exception = await updateStep.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            _eventsFixture.ShouldNotExist<CollectionStepUpdatedEvent>(adminClient.CorrelationId);
        }

        /// <summary>
        /// Scenario: Admin sends invalid update request
        /// Given collection step assigned to user
        /// And collection step is in "Requested" state
        /// When admin sends update request without any properties
        /// Then he receives error response with status code "Bad Request"
        /// And collection step not updated
        /// And event CollectionStepUpdated is not raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenUpdateRequestIsInvalid(UserInfo userInfo, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var reason = nameof(ShouldGetError_WhenUpdateRequestIsInvalid);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requestedCollectionSteps = collectionSteps.Where(step => step.IsRequested());
            var collectionStep = faker.PickRandom(requestedCollectionSteps);

            // Arrange
            var updateRequest = new UpdateCollectionStepRequest(reason);

            // Act
            Func<Task> updateStep = () => adminClient.CollectionStep.UpdateAsync(updateRequest, userId, collectionStep.Id);

            // Assert
            var exception = await updateStep.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            _eventsFixture.ShouldNotExist<CollectionStepUpdatedEvent>(adminClient.CorrelationId);
        }
    }
}