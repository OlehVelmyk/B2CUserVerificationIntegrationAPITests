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
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    internal class CheckTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private ProfileFixture _profileFixture;
        private TaskFixture _taskFixture;
        private ChecksFixture _checksFixture;
        private EventsFixture _eventsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _adminFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _taskFixture = Resolve<TaskFixture>();
            _checksFixture = Resolve<ChecksFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _adminFactory = Resolve<AdministratorFactory>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<EeaUserInfoArbitrary>();
        }

        /// <summary> 
        /// Scenario: Get check details by check id
        /// Given user with completed check
        /// When admin requests check assigned to user by check id
        /// Then he gets the details on that check
        /// And check state is completed
        /// And check input data are included
        /// </summary>
        [Theory]
        public async Task ShouldGetCompletedCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var checks = (await adminClient.Checks.GetAllAsync(userId))
                            .OrderBy(c => c.Type)
                            .ToArray();
            var checkToComplete = FakerFactory.Create(seed).PickRandom(checks);
            var checkId = await _checksFixture.CompleteAsync(userId, 
                                                             checkToComplete.Variant.Id, 
                                                             seed, 
                                                             checkToComplete.RelatedTasks.ToArray());

            // Act
            var check = await adminClient.Checks.GetAsync(userId, checkId);

            // Assert
            check.Should().NotBeNull();

            using (new AssertionScope())
            {
                check.Id.Should().Be(checkId);
                check.State.Should().Be(AdminApi.CheckState.Complete);
                check.Result.Should().NotBeNull();
                check.Type.Should().HaveValue();
                check.Variant.Should().NotBeNull();
                check.Variant.Id.Should().NotBeEmpty();
                check.Variant.Name.Should().NotBeEmpty();
                check.Variant.Provider.Should().HaveValue();
                check.RelatedTasks.Should().NotBeEmpty();
                check.CompletedAt.Should().NotBe(default);
                check.InputData.Should().NotBeNullOrEmpty();
                check.InputDocuments.Should().NotBeNull();
            }

            foreach (var input in check.InputData)
            {
                input.Should().NotBeNull();
                input.Name.Should().NotBeNull();
                input.Value.Should().NotBeNull();
            }
            foreach (var input in check.InputDocuments)
            {
                input.Should().NotBeNull();
                input.Id.Should().NotBeEmpty();
                input.Category.Should().HaveValue();
                input.Type.Should().NotBeEmpty();
                input.Files.Should().NotBeNullOrEmpty();
                input.SubmittedAt.Should().NotBe(default);
            }
        }

        /// <summary>
        /// Scenario: Get check details for non-existent check
        /// Given user without assigned checks
        /// When admin requests check assigned to user by dummy check id
        /// Then he receives error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCheckNotExist(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var checkId = Guid.NewGuid();

            // Act
            Func<Task> getCheck = () => adminClient.Checks.GetAsync(userId, checkId);

            // Assert
            var exception = await getCheck.Should().ThrowAsync<AdminApi.ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Get all user checks
        /// Given user with assigned checks
        /// When admin requests checks assigned to user
        /// Then he receives only checks that belong to requested user
        /// </summary>
        [Theory]
        public async Task ShouldGetChecks(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var excludedTasks = new[] { AdminApi.TaskType.RiskListsScreening };
            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, excludedTasks).Id;
            await _taskFixture.CompleteAsync(userId, taskId, seed);

            // Act
            var checks = await adminClient.Checks.GetAllAsync(userId);

            // Assert
            checks.Should().NotBeNullOrEmpty();
            foreach (var check in checks)
            {
                var expectedResult = check.State == AdminApi.CheckState.Complete
                    ? AdminApi.CheckResult.Passed
                    : (AdminApi.CheckResult?)null;

                check.Should().NotBeNull();

                using (var scope = new AssertionScope())
                {
                    check.Id.Should().NotBeEmpty();
                    check.State.Should().HaveValue();
                    check.Result.Should().Be(expectedResult);
                    check.Type.Should().HaveValue();
                    check.Variant.Should().NotBeNull();
                    check.Variant.Id.Should().NotBeEmpty();
                    check.Variant.Name.Should().NotBeEmpty();
                    check.Variant.Provider.Should().HaveValue();
                    check.RelatedTasks.Should().NotBeEmpty();
                    check.CompletedAt.Should().NotBe(default);
                    check.InputData.Should().NotBeNull();
                    check.InputDocuments.Should().NotBeNull();
                }

                foreach (var input in check.InputData)
                {
                    input.Should().NotBeNull();
                    input.Name.Should().NotBeNull();
                    input.Value.Should().NotBeNull();
                }
                foreach (var input in check.InputDocuments)
                {
                    input.Should().NotBeNull();
                    input.Id.Should().NotBeEmpty();
                    input.Category.Should().HaveValue();
                    input.Type.Should().NotBeEmpty();
                    input.Files.Should().NotBeNullOrEmpty();
                    input.SubmittedAt.Should().NotBe(default);
                }
            }
        }

        /// <summary>
        /// Scenario: Get all user checks
        /// Given user without assigned checks
        /// When admin requests checks assigned to user
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyArray_WhenUserHasNoChecks(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            var checks = await adminClient.Checks.GetAllAsync(userId);

            // Assert
            checks.Should().BeEmpty();
        }

        /// <summary> 
        /// Scenario: Re-request check
        /// Given user with completed check and assigned tasks
        /// When request check of given type and variant by given tasks
        /// Then new check is requested
        /// And check is completed
        /// And related tasks are present
        /// And event about creation of check is raised
        /// And event about completion of check is raised too
        /// </summary>
        [Theory]
        public async Task ShouldReRequestCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            _eventsFixture.ShouldExist<CheckCompletedEvent>(e => e.EventArgs.UserId == userId);
            var checks = await adminClient.Checks.GetAllAsync(userId);
            var orderedChecks = checks.Where(check => check.State == AdminApi.CheckState.Complete)
                                      .OrderBy(check => check.Type).ToArray();
            var check = FakerFactory.Create(seed).PickRandom(orderedChecks);

            // Arrange
            var request = new AdminApi.CheckRequest
            {
                VariantId = check.Variant.Id,
                RelatedTasks = check.RelatedTasks,
                Reason = nameof(ShouldReRequestCheck)
            };

            // Act
            await adminClient.Checks.RequestAsync(request, userId);

            // Assert
            _eventsFixture.ShouldExistSingle<CheckCreatedEvent>(adminClient.CorrelationId);
            _eventsFixture.ShouldExistSingle<CheckStartedEvent>(adminClient.CorrelationId);
            _eventsFixture.ShouldExistExact<CheckCompletedEvent>(
                2, e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == check.Variant.Id);
        }

        /// <summary> 
        /// Scenario: Request duplicated check
        /// Given user with pending check and assigned tasks
        /// When request same check 
        /// Then new check is requested
        /// And event about creation of check is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestDuplicatedCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var checks = await adminClient.Checks.GetAllAsync(userId);
            var orderedChecks = checks.Where(check => check.State == AdminApi.CheckState.Pending)
                                      .OrderBy(check => check.Type).ToArray();
            var check = FakerFactory.Create(seed).PickRandom(orderedChecks);

            // Arrange
            var request = new AdminApi.CheckRequest
            {
                VariantId = check.Variant.Id,
                RelatedTasks = check.RelatedTasks,
                Reason = nameof(ShouldRequestDuplicatedCheck)
            };

            // Act
            await adminClient.Checks.RequestAsync(request, userId);

            // Assert
            _eventsFixture.ShouldExist<CheckCreatedEvent>(adminClient.CorrelationId);

            checks = await adminClient.Checks.GetAllAsync(userId);
            checks.Where(c => c.Type == check.Type).Should().HaveCount(2);
        }

        /// <summary> 
        /// Scenario: Request unique check 
        /// Given user with assigned tasks
        /// When request check of given type and variant without related tasks
        /// And check is requested first time
        /// Then check is requested
        /// And related tasks are empty
        /// And event about creation of check is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestUniqueCheck(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _profileFixture.CreateAsync(userInfo);

            // Arrange
            var variantId = new Guid("779E3ED5-68E1-4C95-8E15-E6D8957820BC");
            var request = new AdminApi.CheckRequest
            {
                VariantId = variantId,
                RelatedTasks = new List<Guid>(),
                Reason = nameof(ShouldRequestUniqueCheck)
            };

            // Act
            await adminClient.Checks.RequestAsync(request, userId);

            // Assert
            var checkId = _eventsFixture.ShouldExist<CheckCreatedEvent>(adminClient.CorrelationId).EventArgs.CheckId;

            var checks = await adminClient.Checks.GetAllAsync(userId);
            var check = checks.Where(check => check.Variant.Id == variantId).Should().HaveCount(1).And.Subject.First();
            check.Id.Should().Be(checkId);
            check.State.Should().Be(AdminApi.CheckState.Pending);
            check.RelatedTasks.Should().BeEmpty();
        }
    }
}
