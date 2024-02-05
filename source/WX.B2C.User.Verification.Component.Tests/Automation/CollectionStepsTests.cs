using System;
using System.Threading.Tasks;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class CollectionStepsTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private SurveyFixture _surveyFixture;
        private ProfileFixture _profileFixture;
        private TurnoverFixture _turnoverFixture;
        private DocumentsFixture _documentsFixture;
        private EventsFixture _eventsFixture;
        private CollectionStepsFixture _collectionStepsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _adminFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _surveyFixture = Resolve<SurveyFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _turnoverFixture = Resolve<TurnoverFixture>();
            _documentsFixture = Resolve<DocumentsFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _collectionStepsFixture = Resolve<CollectionStepsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _adminFactory = Resolve<AdministratorFactory>();

            Arb.Register<AddressArbitrary>();
            Arb.Register<UserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Complete personal details step
        /// Given user with requested personal details step
        /// When required data is submitted
        /// Then step state becomes "Completed"
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCompletePersonalDetailsStep(UserInfo userInfo, Address address)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var stepId = await _collectionStepsFixture.RequestAsync(userId, AdminApi.PersonalDetailsProperty.ResidenceAddress);

            // Act
            await _profileFixture.UpdateAsync(userId, address);

            // Assert
            _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(e => e.EventArgs.CollectionStepId == stepId);
        }

        /// <summary>
        /// Scenario: Complete verification details step
        /// Given user with requested verification details step
        /// When required data is submitted
        /// Then step state becomes "Completed"
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCompleteVerificationDetailsStep(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var stepId = await _collectionStepsFixture.RequestAsync(userId, AdminApi.VerificationDetailsProperty.Turnover);

            // Act
            var turnover = 100m;
            await _turnoverFixture.UpdateAsync(userId, turnover);

            // Assert
            _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(e => e.EventArgs.CollectionStepId == stepId);
        }

        /// <summary>
        /// Scenario: Complete document step
        /// Given user with requested document step
        /// When required document is submitted
        /// Then step state becomes "Completed"
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCompleteDocumentStep(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var stepId = await _collectionStepsFixture.RequestAsync(userId, AdminApi.DocumentCategory.ProofOfFunds);

            // Act
            await _documentsFixture.SubmitAsync(userId, DocumentCategory.ProofOfFunds, seed);

            // Assert
            _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(e => e.EventArgs.CollectionStepId == stepId);
        }

        /// <summary>
        /// Scenario: Complete survey step
        /// Given user with requested survey step
        /// When required survey is submitted
        /// Then step state becomes "Completed"
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCompleteSurveyStep(UserInfo userInfo, Guid templateId)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            var stepId = await _collectionStepsFixture.RequestAsync(userId, templateId);

            // Act
            await _surveyFixture.SubmitAsync(userId, templateId);

            // Assert
            _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(e => e.EventArgs.CollectionStepId == stepId);
        }

        /// <summary>
        /// Scenario: Request already requested step
        /// Given user with requested step
        /// When request same step again
        /// Then new step is not created
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotRequestSameStep(UserInfo userInfo, Guid templateId, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            await _collectionStepsFixture.RequestAsync(userId, templateId);

            // Arrange
            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks);

            var request = new AdminApi.SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldNotRequestSameStep),
                Type = AdminApi.CollectionStepType.Survey,
                IsRequired = true,
                IsReviewNeeded = false,
                TemplateId = templateId,
                TargetTasks = new[] { task.Id }
            };

            // Act
            await adminClient.CollectionStep.RequestAsync(request, userId);

            // Assert
            _eventsFixture.ShouldNotExist<CollectionStepRequestedEvent>(adminClient.CorrelationId);
        }

        /// <summary>
        /// Scenario: Move in review step
        /// Given user with requested step
        /// And step is review needed
        /// When submit required data
        /// Then step state becomes "InReview"
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldMoveInReviewStep(UserInfo userInfo, Guid templateId, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks);

            var request = new AdminApi.SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldMoveInReviewStep),
                Type = AdminApi.CollectionStepType.Survey,
                IsRequired = true,
                IsReviewNeeded = true,
                TemplateId = templateId,
                TargetTasks = new[] { task.Id }
            };

            var stepId = await _collectionStepsFixture.RequestAsync(userId, request);

            // Act
            await _surveyFixture.SubmitAsync(userId, templateId);

            // Assert
            _eventsFixture.ShouldNotExist<CollectionStepCompletedEvent>(e => e.EventArgs.CollectionStepId == stepId);
            _eventsFixture.ShouldExistSingle<CollectionStepReadyForReviewEvent>(e => e.EventArgs.CollectionStepId == stepId);
        }
    }
}
