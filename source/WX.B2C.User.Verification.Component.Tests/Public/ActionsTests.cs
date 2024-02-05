using System;
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
using WX.B2C.User.Verification.Component.Tests.Mappers;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Extensions;
using ActionType = WX.B2C.User.Verification.Api.Public.Client.Models.ActionType;

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    internal partial class ActionsTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private ProfileFixture _profileFixture;
        private CollectionStepsFixture _collectionStepsFixture;
        private PublicApiClientFactory _publicApiClientFactory;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _adminFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _collectionStepsFixture = Resolve<CollectionStepsFixture>();
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _adminFactory = Resolve<AdministratorFactory>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<GbUserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<EeaUserInfoArbitrary>();
            Arb.Register<ApacUserInfoArbitrary>();
            Arb.Register<RoWUserInfoArbitrary>();
            Arb.Register<GlobalUserInfoArbitrary>();
            Arb.Register<RuUserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: User gets actions assigned to him
        /// Given user without collection steps
        /// And he didn`t start verification
        /// When user requests actions assigned to him
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyCollection_WhenDoNotHaveCollectionSteps(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var client = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            var userActions = await client.Actions.GetAsync();

            // Assert
            userActions.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: User gets actions assigned to him
        /// Given user with rejected application
        /// And several collection steps assigned to given application
        /// When user requests actions assigned to him
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyCollection_WhenApplicationIsRejected(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var reason = new ReasonDto(nameof(ShouldGetEmptyCollection_WhenApplicationIsRejected));
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var application = await adminClient.Applications.GetDefaultAsync(userId);
            await adminClient.Applications.RejectAsync(reason, userId, application.Id);

            // Act
            var userActions = await publicClient.Actions.GetAsync();

            // Assert
            userActions.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: User gets actions assigned to him
        /// Given user with cancelled application
        /// And several collection steps assigned to given application
        /// When user requests actions assigned to him
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyCollection_WhenApplicationIsCancelled(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var application = await adminClient.Applications.GetDefaultAsync(userId);
            await _applicationFixture.ApproveAsync(userId, application.Id, seed);
            await _applicationFixture.CancelAsync(userId, application.Id);

            // Act
            var userActions = await publicClient.Actions.GetAsync();

            // Assert
            userActions.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: User gets actions assigned to him
        /// Given user with several collection steps
        /// And no collection steps are in "Requested" state
        /// When user requests actions assigned to him
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyCollection_WhenUserDoNotHaveRequestedCollectionSteps(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            await _collectionStepsFixture.CompleteAllAsync(userId, steps.Select(x => x.Id), true, seed);

            // Act
            var userActions = await publicClient.Actions.GetAsync();

            // Assert
            userActions.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: User gets actions assigned to him
        /// Given user with applied application
        /// And several collection steps assigned to given application
        /// And some collection steps are in "Requested" state
        /// When user requests actions assigned to him
        /// Then he receives only actions for requested collection steps
        /// </summary>
        [Theory]
        public async Task ShouldGetOnlyRequested(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var expectedActions = steps.Where(step => step.State == CollectionStepState.Requested)
                                       .Select(step => step.Variant)
                                       .Select(ActionMapper.Map)
                                       .Where(action => action.HasValue);

            // Act
            var userActions = await publicClient.Actions.GetAsync();
            var actualActions = userActions.Select(ua => ua.ActionType);

            // Assert
            actualActions.Should().BeEquivalentTo(expectedActions);
        }

        /// <summary>
        /// Scenario: User gets all possible actions
        /// Given user with applied application
        /// And collection steps for all actions are requested
        /// When user requests actions assigned to him
        /// Then he receives all possible actions
        /// </summary>
        [Theory]
        public async Task ShouldGetAllActions(UsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskId = await adminClient.Tasks.GetAllAsync(userId).Select(tasks => tasks.Select(task => task.Id).First());

            await _collectionStepsFixture.RequestAsync(userId, DocumentCategory.ProofOfAddress, DocumentTypes.Other, new[] { taskId });
            await _collectionStepsFixture.RequestAsync(userId, DocumentCategory.ProofOfFunds, DocumentTypes.Other, new[] { taskId });
            await _collectionStepsFixture.RequestAsync(userId, DocumentCategory.Taxation, DocumentTypes.W9Form, new[] { taskId });

            // Arrange 
            var expectedActions = Enum.GetNames(typeof(ActionType)).Select(Enum.Parse<ActionType>);

            // Act
            var userActions = await publicClient.Actions.GetAsync();
            var actualActions = userActions.Select(ua => ua.ActionType);

            // Assert
            actualActions.Should().BeEquivalentTo(expectedActions);
        }

        /// <summary>
        /// Scenario: User gets many actions about surveys
        /// Given user with applied application
        /// And survey collection steps are requested
        /// When user requests actions assigned to him
        /// Then he receives many actions about surveys
        /// </summary>
        [Theory]
        public async Task ShouldGetActions_WhenManySurveysRequested(UsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var taskId = await adminClient.Tasks.GetAllAsync(userId).Select(tasks => tasks.Select(task => task.Id).First());

            await _collectionStepsFixture.RequestAsync(userId, Surveys.UsEddSurvey, new[] { taskId });
            await _collectionStepsFixture.RequestAsync(userId, Surveys.UkOnboardingSurvey, new[] { taskId });
            await _collectionStepsFixture.RequestAsync(userId, Surveys.UkPepSurvey, new[] { taskId });
            await _collectionStepsFixture.RequestAsync(userId, Surveys.UkOccupationSurvey, new[] { taskId });
            await _collectionStepsFixture.RequestAsync(userId, Surveys.UkSofSurvey, new[] { taskId });

            // Act
            var userActions = await publicClient.Actions.GetAsync();
            var actions = userActions.Where(ua => ua.ActionType == ActionType.Survey);

            // Assert
            actions.Should().HaveCount(6);
            userActions.Should().ContainsSurvey(Surveys.UsCddSurvey, Surveys.UsCddTag);
            userActions.Should().ContainsSurvey(Surveys.UsEddSurvey, Surveys.UsEddTag);
            userActions.Should().ContainsSurvey(Surveys.UkOnboardingSurvey, Surveys.UkOnboardingTag);
            userActions.Should().ContainsSurvey(Surveys.UkPepSurvey, Surveys.UkPepTag);
            userActions.Should().ContainsSurvey(Surveys.UkOccupationSurvey, Surveys.UkOccupationTag);
            userActions.Should().ContainsSurvey(Surveys.UkSofSurvey, Surveys.UkSofTag);
        }

        /// <summary>
        /// Scenario: User from GB gets onboarding actions
        /// Given user from GB
        /// And he started verification
        /// When user requests actions assigned to him
        /// Then he receives GB onboarding actions
        /// </summary>
        [Theory]
        public Task ShouldGetGbActions(GbUserInfo userInfo) =>
            ShouldGetActions(userInfo);

        /// <summary>
        /// Scenario: User from US gets onboarding actions
        /// Given user from US
        /// And he started verification
        /// When user requests actions assigned to him
        /// Then he receives US onboarding actions
        /// </summary>
        [Theory]
        public Task ShouldGetUsActions(UsUserInfo userInfo) =>
            ShouldGetActions(userInfo);

        /// <summary>
        /// Scenario: User from EEA gets onboarding actions
        /// Given user from EEA
        /// And he started verification
        /// When user requests actions assigned to him
        /// Then he receives EEA onboarding actions
        /// </summary>
        [Theory]
        public Task ShouldGetEeaActions(EeaUserInfo userInfo) => 
            ShouldGetActions(userInfo);

        /// <summary>
        /// Scenario: User from APAC gets onboarding actions
        /// Given user from APAC
        /// And he started verification
        /// When user requests actions assigned to him
        /// Then he receives APAC onboarding actions
        /// </summary>
        [Theory]
        public Task ShouldGetApacActions(ApacUserInfo userInfo) =>
            ShouldGetActions(userInfo);

        /// <summary>
        /// Scenario: User from RoW gets onboarding actions
        /// Given user from RoW
        /// And he started verification
        /// When user requests actions assigned to him
        /// Then he receives RoW onboarding actions
        /// </summary>
        [Theory]
        public Task ShouldGetRoWActions(RoWUserInfo userInfo) =>
            ShouldGetActions(userInfo);

        /// <summary>
        /// Scenario: User from Global gets onboarding actions
        /// Given user from Global
        /// And he started verification
        /// When user requests actions assigned to him
        /// Then he receives Global onboarding actions
        /// </summary>
        [Theory]
        public Task ShouldGetGlobalActions(GlobalUserInfo userInfo) =>
            ShouldGetActions(userInfo);

        /// <summary>
        /// Scenario: User from Ru gets onboarding actions
        /// Given user from Ru
        /// And he started verification
        /// When user requests actions assigned to him
        /// Then he receives Ru onboarding actions
        /// </summary>
        [Theory]
        public Task ShouldGetRuActions(RuUserInfo userInfo) =>
            ShouldGetActions(userInfo);
    }
}
