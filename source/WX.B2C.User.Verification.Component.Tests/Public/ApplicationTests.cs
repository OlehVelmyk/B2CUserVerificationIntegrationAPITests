using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
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
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using ErrorResponseException = WX.B2C.User.Verification.Api.Public.Client.Models.ErrorResponseException;
using PublicEvents = WX.B2C.User.Verification.Events.Events;

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    internal partial class ApplicationTests : BaseComponentTest
    {
        private ProfileFixture _profileFixture;
        private EventsFixture _eventsFixture;
        private PublicApiClientFactory _publicApiClientFactory;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _profileFixture = Resolve<ProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<GbUserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<EeaUserInfoArbitrary>();
            Arb.Register<ApacUserInfoArbitrary>();
            Arb.Register<RoWUserInfoArbitrary>();
            Arb.Register<GlobalUserInfoArbitrary>();
            Arb.Register<RuUserInfoArbitrary>();
            Arb.Register<UnsupportedUserInfoArbitrary>();
            Arb.Register<UnsupportedStateUsUserInfoArbitrary>();
            Arb.Register<EmptyUserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Start verification
        /// Given user with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And product type is WirexBasic
        /// And state is applied
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRegisterApplication(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            await publicClient.Applications.RegisterAsync();

            // Assert
            _eventsFixture.ShouldExistSingle<PublicEvents.ApplicationRegisteredEvent>(publicClient.CorrelationId);
            _eventsFixture.ShouldExist<TaskCreatedEvent>(publicClient.CorrelationId);
            _eventsFixture.ShouldExist<ApplicationRequiredTaskAddedEvent>(publicClient.CorrelationId);

            var application = await adminClient.Applications.GetDefaultAsync(userId);
            application.Should().NotBeNull();

            using var scope = new AssertionScope();
            application.State.Should().Be(ApplicationState.Applied);
            application.RequiredTasks.Should().NotBeNullOrEmpty();
            application.AllowedActions.Should().NotBeNullOrEmpty();
            application.DecisionReasons.Should().BeEmpty();
        }
        
        /// <summary>
        /// Scenario: Start verification several times at the same time
        /// Given user with personal details
        /// And with residence county
        /// When he registers application several times at the same time in parallel
        /// Then application only one is assigned to user
        /// And product type is WirexBasic
        /// And state is applied
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRegisterApplication_WhenRegisterCalledSimultaneously(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            Func<Task> act = async () =>
            {
                try
                {
                    await publicClient.Applications.RegisterAsync();
                }
                catch (ErrorResponseException e)
                {
                    //Conflict is expected exception
                    if (e.Response.StatusCode == HttpStatusCode.Conflict)
                        return;
                    
                    throw;
                }
            };
            await Task.WhenAll(act(), act(), act());
            
            // Assert
            _eventsFixture.ShouldExistSingle<PublicEvents.ApplicationRegisteredEvent>(publicClient.CorrelationId);
            _eventsFixture.ShouldExistSingle<ApplicationAutomatedEvent>(publicClient.CorrelationId);

            var application = await adminClient.Applications.GetDefaultAsync(userId);
            application.Should().NotBeNull();
            
            using var scope = new AssertionScope();
            application.State.Should().Be(ApplicationState.Applied);
            application.RequiredTasks.Should().NotBeNullOrEmpty();
            application.AllowedActions.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Scenario: Start verification too early
        /// Given user with personal details
        /// And without residence county
        /// When he registers application
        /// Then he receives error response with status code "Too Early"
        /// And application is not registered
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenUserWithoutCountry(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            userInfo.Address = null;

            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            await _profileFixture.CreateAsync(userInfo);

            // Act
            Func<Task> registerApplication = () => publicClient.Applications.RegisterAsync();

            // Assert
            var exception = await registerApplication.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().HaveValue(425);
        }

        /// <summary>
        /// Scenario: Start verification (Ip address)
        /// Given user with personal details
        /// And with residence county
        /// And his bearer token with ip address
        /// When he registers application
        /// Then application is assigned to user
        /// And verification details are created
        /// And ip address is present
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSetIpAddressFromClaims_WhenIpHeaderNotProvided(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var correlationId = Guid.NewGuid();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress, correlationId);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            await publicClient.Applications.RegisterAsync();

            // Assert
            _eventsFixture.ShouldExistSingle<PublicEvents.ApplicationRegisteredEvent>(correlationId);
            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var change = @event.EventArgs.Changes.Should().HaveCount(1).And.Contain<string>(VerificationDetails.IpAddress).Which;
            change.Should().Match(old => old.Should().BeNull(), @new => @new.Should().Be(userInfo.IpAddress));
        }

        /// <summary>
        /// Scenario: Start verification (Ip address)
        /// Given user with personal details
        /// And with residence county
        /// And his bearer token with ip address
        /// When he registers application
        /// And http request contains test ip header
        /// Then application is assigned to user
        /// And verification details are created
        /// And ip address is present with value from header
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldSetIpAddressFromHeader_WhenNotProduction(UserInfo userInfo, IPAddress testIpAddress)
        {
            var userId = userInfo.UserId.Dump();
            var ipAddress = testIpAddress.ToString().Dump();

            var correlationId = Guid.NewGuid();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress, correlationId);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            await publicClient.Applications.RegisterWithHttpMessagesAsync(new Dictionary<string, List<string>>
            {
                { IpAddresses.TestIpHeaderKey, new List<string> { ipAddress } }
            });

            // Assert
            _eventsFixture.ShouldExistSingle<PublicEvents.ApplicationRegisteredEvent>(correlationId);
            var @event = _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
            var change = @event.EventArgs.Changes.Should().HaveCount(1).And.Contain<string>(VerificationDetails.IpAddress).Which;
            change.Should().Match(old => old.Should().BeNull(), @new => @new.Should().Be(ipAddress));
        }

        /// <summary>
        /// Scenario: Start verification (No ip address)
        /// Given user with personal details
        /// And with residence county
        /// And his bearer token without ip address
        /// When he registers application
        /// Then he receives error response with status code "Bad Request"
        /// And application is not registered
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenTokenWithoutIpAddress(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, ipAddress: null);
            await _profileFixture.CreateAsync(userInfo);

            // Act
            Func<Task> registerApplication = () => publicClient.Applications.RegisterAsync();

            // Assert
            var exception = await registerApplication.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Scenario: Start verification (GB tasks)
        /// Given user from GB with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And all GB onboarding tasks are assigned to user
        /// And they all are linked to user application
        /// And all GB acceptance checks are assigned to user
        /// And events are raised
        /// </summary>
        [Theory]
        public Task ShouldAssignGbTasks(GbUserInfo userInfo) =>
            ShouldAssignTasks(userInfo);

        /// <summary>
        /// Scenario: Start verification (US tasks)
        /// Given user from US with personal details
        /// And with residence county and state
        /// When he registers application
        /// Then application is assigned to user
        /// And all US onboarding tasks are assigned to user
        /// And they all are linked to user application
        /// And all US acceptance checks are assigned to user
        /// And events are raised
        /// </summary>
        [Theory]
        public Task ShouldAssignUsTasks(UsUserInfo userInfo) =>
            ShouldAssignTasks(userInfo);

        /// <summary>
        /// Scenario: Start verification (EEA tasks)
        /// Given user from EEA with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And all EEA onboarding tasks are assigned to user
        /// And they all are linked to user application
        /// And all EEA acceptance checks are assigned to user
        /// And events are raised
        /// </summary>
        [Theory]
        public Task ShouldAssignEeaTasks(EeaUserInfo userInfo) =>
            ShouldAssignTasks(userInfo);

        /// <summary>
        /// Scenario: Start verification (APAC tasks)
        /// Given user from EEA with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And all APAC onboarding tasks are assigned to user
        /// And they all are linked to user application
        /// And all APAC acceptance checks are assigned to user
        /// And events are raised
        /// </summary>
        [Theory]
        public Task ShouldAssignApacTasks(ApacUserInfo userInfo) =>
            ShouldAssignTasks(userInfo);

        /// <summary>
        /// Scenario: Start verification (RoW tasks)
        /// Given user from RoW with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And all RoW onboarding tasks are assigned to user
        /// And they all are linked to user application
        /// And all RoW acceptance checks are assigned to user
        /// And events are raised
        /// </summary>
        [Theory]
        public Task ShouldAssignRoWTasks(RoWUserInfo userInfo) =>
            ShouldAssignTasks(userInfo);

        /// <summary>
        /// Scenario: Start verification (Ru tasks)
        /// Given user from Ru with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And all Ru onboarding tasks are assigned to user
        /// And they all are linked to user application
        /// And all Ru acceptance checks are assigned to user
        /// And events are raised
        /// </summary>
        [Theory]
        public Task ShouldAssignRuTasks(RuUserInfo userInfo) =>
            ShouldAssignTasks(userInfo);

        /// <summary>
        /// Scenario: Start verification (Global tasks)
        /// Given user from Global with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And all Global onboarding tasks are assigned to user
        /// And they all are linked to user application
        /// And all Global acceptance checks are assigned to user
        /// And events are raised
        /// </summary>
        [Theory]
        public Task ShouldAssignGlobalTasks(GlobalUserInfo userInfo) =>
            ShouldAssignTasks(userInfo);

        /// <summary>
        /// Scenario: Start verification (Collection steps)
        /// Given user with personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And collection steps are requested for user
        /// And no duplicated collection steps are present
        /// And events are raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestCollectionSteps(EmptyUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress, correlationId);

            await _profileFixture.CreateAsync(userInfo);

            var xPathes = new[] { PersonalDetails.FullName, PersonalDetails.Birthdate, PersonalDetails.ResidenceAddress,
                                  VerificationDetails.TaxResidence, VerificationDetails.IdDocumentNumber, VerificationDetails.IsPep,
                                  VerificationDetails.IsAdverseMedia, VerificationDetails.IsSanctioned, Constants.Documents.ProofOfIdentity };

            // Act
            await publicClient.Applications.RegisterAsync();

            // Assert
            _eventsFixture.ShouldExistSingle<PublicEvents.ApplicationRegisteredEvent>(correlationId);

            xPathes.Foreach(xPath => _eventsFixture.ShouldExist<CollectionStepRequestedEvent>(e => IsRequested(e, xPath)));
            _eventsFixture.ShouldExist<CollectionStepRequestedEvent>(IsSelfieRequested);

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            steps.Select(step => step.Variant.Name).Should().OnlyHaveUniqueItems();

            bool IsRequested(CollectionStepRequestedEvent e, string xPath) =>
                e.CorrelationId == correlationId && e.EventArgs.XPath == xPath;

            bool IsSelfieRequested(CollectionStepRequestedEvent e) =>
                e.CorrelationId == correlationId && e.EventArgs.XPath.StartsWith(Constants.Documents.Selfie);
        }

        /// <summary>
        /// Scenario: Start verification (Completed steps\checks)
        /// Given user with enriched personal details
        /// And with residence county
        /// When he registers application
        /// Then application is assigned to user
        /// And collection steps are requested for user
        /// And steps for which data are already present should be completed
        /// And checks for which completed required data should be completed
        /// And events are raised
        /// </summary>
        [Theory]
        public async Task ShouldCompleteCollectionStepsAndChecks(EeaUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress, correlationId);

            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var xPathes = new[] { PersonalDetails.FullName, PersonalDetails.Birthdate, PersonalDetails.ResidenceAddress, VerificationDetails.IpAddress };
            var checks = new[] { CheckType.NameAndDoBDuplication, CheckType.IpMatch };
            var tasks = new[] { TaskType.DuplicationScreening, TaskType.Address };

            // Act
            await publicClient.Applications.RegisterAsync();

            // Assert
            _eventsFixture.ShouldExistSingle<PublicEvents.ApplicationRegisteredEvent>(correlationId);

            xPathes.Foreach(xPath => _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(e => IsStepCompleted(e, xPath)));

            tasks.Select(task => _eventsFixture.ShouldExistSingle<TaskCreatedEvent>(e => IsTaskCreated(e, task)))
                 .Select(e => e.EventArgs.TaskId)
                 .Foreach(taskId => _eventsFixture.ShouldExistSingle<ApplicationRequiredTaskAddedEvent>(e => IsTaskAdded(e, taskId)));

            checks.Foreach(check => _eventsFixture.ShouldExist<CheckCompletedEvent>(e => IsCheckCompleted(e, check)));

            var completedChecks = await adminClient.Checks.GetAllAsync(userId);
            completedChecks.Where(check => check.State == CheckState.Complete).Select(check => check.Type).Should().BeEquivalentTo(checks);

            bool IsStepCompleted(CollectionStepCompletedEvent e, string xPath) =>
                e.CorrelationId == correlationId && e.EventArgs.XPath == xPath;

            bool IsTaskCreated(TaskCreatedEvent e, TaskType taskType) =>
                e.CorrelationId == correlationId && e.EventArgs.Type == taskType.To<Verification.Events.Internal.Enums.TaskType>();

            bool IsTaskAdded(ApplicationRequiredTaskAddedEvent e, Guid taskId) =>
                e.CorrelationId == correlationId && e.EventArgs.TaskId == taskId;

            bool IsCheckCompleted(CheckCompletedEvent e, CheckType type) =>
                e.CorrelationId == correlationId && e.EventArgs.Type == type.To<Verification.Events.Internal.Enums.CheckType>();
        }

        /// <summary>
        /// Scenario: Start verification (Dummy product type)
        /// Given user with personal details
        /// And with residence county
        /// When he registers application with dummy product type
        /// Then he receives error response with status code "Bad Request"
        /// And application is not registered
        /// And no events are raised
        /// </summary>
        [Test, Ignore("Now product type is only WirexBasic")]
        public async Task ShouldGetError_WhenDummyProductType()
        { }

        /// <summary>
        /// Scenario: Start verification second time
        /// Given user with personal details
        /// And with residence county
        /// And with application
        /// When he registers new application
        /// Then he receives error response with status code "Conflict"
        /// And new application is not registered
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenRegisterSecondApplication(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            await _profileFixture.CreateAsync(userInfo);
            await publicClient.Applications.RegisterAsync();

            // Act
            Func<Task> registerApplication = () => publicClient.Applications.RegisterAsync();

            // Assert
            var exception = await registerApplication.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        /// <summary>
        /// Scenario: Start verification (Unavailable country)
        /// Given user with personal details
        /// And with residence county 
        /// And country is not supported
        /// When he registers application
        /// Then he receives error response with status code "Unprocessable Entity"
        /// And application is not registered
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCountryUnavailable(UnsupportedUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            await _profileFixture.CreateAsync(userInfo);

            // Act
            Func<Task> registerApplication = () => publicClient.Applications.RegisterAsync();

            // Assert
            var exception = await registerApplication.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        /// <summary>
        /// Scenario: Start verification (Unavailable state)
        /// Given user with personal details
        /// And with residence county and state
        /// And state is not supported
        /// When he registers application
        /// Then he receives error response with status code "Unprocessable Entity"
        /// And application is not registered
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenStateUnavailable(UnsupportedStateUsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            await _profileFixture.CreateAsync(userInfo);

            // Act
            Func<Task> registerApplication = () => publicClient.Applications.RegisterAsync();

            // Assert
            var exception = await registerApplication.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }
    }
}
