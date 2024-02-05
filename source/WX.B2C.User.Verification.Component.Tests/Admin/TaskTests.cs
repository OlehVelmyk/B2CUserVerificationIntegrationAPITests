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
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Admin
{
    internal class TaskTests : BaseComponentTest
    {
        private readonly IReadOnlyCollection<TaskType> _excludedTasks = new[] { TaskType.RiskListsScreening, TaskType.Address };

        private ApplicationFixture _applicationFixture;
        private TaskFixture _taskFixture;
        private CollectionStepsFixture _stepsFixture;
        private ChecksFixture _checksFixture;
        private ProfileFixture _profileFixture;
        private EventsFixture _eventsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _administratorFactory;
        private ICheckProvider _checkProvider;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _taskFixture = Resolve<TaskFixture>();
            _stepsFixture = Resolve<CollectionStepsFixture>();
            _checksFixture = Resolve<ChecksFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();
            _checkProvider = Resolve<ICheckProvider>();

            Arb.Register<UserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Get details for completed task by task id
        /// Given user with completed task
        /// When admin requests task assigned to user by task id
        /// Then he gets the details on that task
        /// And task state is completed
        /// And task steps and task checks are included
        /// </summary>
        [Theory]
        public async Task ShouldGetCompletedTask(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, _excludedTasks).Id.Dump();
            await _taskFixture.CompleteAsync(userId, taskId, seed);

            // Act
            var task = await adminClient.Tasks.GetAsync(userId, taskId);

            // Assert
            task.Should().NotBeNull();
            using (var scope = new AssertionScope())
            {
                task.Id.Should().Be(taskId);
                task.Type.Should().HaveValue();
                task.Variant.Should().NotBeNull();
                task.Variant?.Id.Should().NotBeEmpty();
                task.Variant?.Name.Should().NotBeNullOrEmpty();
                task.State.Should().Be(TaskState.Completed);
                task.Result.Should().Be(TaskResult.Passed);
                task.CreatedAt.Should().NotBe(default);
                task.CollectionSteps.Should().NotBeNullOrEmpty();
                task.Checks.Should().NotBeNull();
            }
            foreach (var step in task.CollectionSteps)
            {
                var expectedReviewResult = step.IsReviewNeeded
                    ? CollectionStepReviewResult.Approved
                    : (CollectionStepReviewResult?)null;

                using var scope = new AssertionScope();
                step.Should().NotBeNull();
                step.Id.Should().NotBeEmpty();
                step.State.Should().Be(CollectionStepState.Completed);
                step.ReviewResult.Should().Be(expectedReviewResult);
                step.RequestedAt.Should().NotBe(default);
                step.UpdatedAt.Should().NotBe(default);
                step.FormattedData.Should().NotBeNullOrEmpty();
                ////step.Data.Should().NotBeNull();
                step.Variant.Should().NotBeNull();
                step.Variant.Should().BeAssignableTo<CollectionStepVariantDto>();
                step.Variant.Name.Should().NotBeNull();
            }
            foreach (var check in task.Checks)
            {
                using var scope = new AssertionScope();
                check.Id.Should().NotBeEmpty();
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Passed);
                check.Type.Should().HaveValue();
                check.Variant.Should().NotBeNull();
                check.Variant.Id.Should().NotBeEmpty();
                check.Variant.Name.Should().NotBeNullOrEmpty();
                check.Variant.Provider.Should().HaveValue();
                ////check.RelatedTasks.Should().NotBeEmpty(); // TODO: WRXB-10617 Fix api to return RelatedTasks
                check.CompletedAt.Should().NotBe(default);
            }
        }

        /// <summary>
        /// Scenario: Get details for incomplete task by task id
        /// Given user with assigned incomplete task
        /// When admin requests task assigned to user by task id
        /// Then he gets the details on that task
        /// And task state is incomplete
        /// And task steps and task checks are empty
        /// </summary>
        [Theory]
        public async Task ShouldGetIncompleteTask(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, _excludedTasks).Id.Dump();

            // Act
            var task = await adminClient.Tasks.GetAsync(userId, taskId);

            // Assert
            task.Should().NotBeNull();
            using (var scope = new AssertionScope())
            {
                task.Id.Should().Be(taskId);
                task.Type.Should().HaveValue();
                task.Variant.Should().NotBeNull();
                task.Variant?.Id.Should().NotBeEmpty();
                task.Variant?.Name.Should().NotBeNullOrEmpty();
                task.State.Should().Be(TaskState.Incomplete);
                task.Result.Should().BeNull();
                task.CreatedAt.Should().NotBe(default);
                task.CollectionSteps.Should().NotBeNullOrEmpty();
                task.Checks.Should().NotBeNull();
            }
            foreach (var step in task.CollectionSteps)
            {
                var isStepCompleted = step.State == CollectionStepState.Completed;
                var expectedReviewResult = step.IsReviewNeeded && isStepCompleted
                    ? CollectionStepReviewResult.Approved
                    : (CollectionStepReviewResult?)null;

                using var scope = new AssertionScope();
                step.Should().NotBeNull();
                step.Id.Should().NotBeEmpty();
                step.State.Should().HaveValue();
                step.ReviewResult.Should().Be(expectedReviewResult);
                step.RequestedAt.Should().NotBe(default);
                step.UpdatedAt.Should().NotBe(default);
                step.FormattedData.Should().BeSomeOrNull(_ => isStepCompleted);
                ////step.Data.Should().BeNull();
                step.Variant.Should().NotBeNull();
                step.Variant.Should().BeAssignableTo<CollectionStepVariantDto>();
                step.Variant.Name.Should().NotBeNull();
            }
            foreach (var check in task.Checks)
            {
                var isCheckCompleted = check.State == CheckState.Complete;

                using var scope = new AssertionScope();
                check.Id.Should().NotBeEmpty();
                check.State.Should().HaveValue();
                check.Result.Should().BeSomeOrNull(_ => isCheckCompleted);
                check.Type.Should().HaveValue();
                check.Variant.Should().NotBeNull();
                check.Variant.Id.Should().NotBeEmpty();
                check.Variant.Name.Should().NotBeNullOrEmpty();
                check.Variant.Provider.Should().HaveValue();
                ////check.RelatedTasks.Should().NotBeEmpty(); // TODO: WRXB-10617 Fix api to return RelatedTasks
                check.CompletedAt.Should().NotBe(default);
            }
        }

        /// <summary>
        /// Scenario: Get task details for non-existent task
        /// Given user without assigned tasks
        /// When admin requests task by dummy task id
        /// Then he receives error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenTaskNotExist(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            Func<Task> getTask = () => adminClient.Tasks.GetAsync(userId, Guid.NewGuid());

            // Assert
            var exception = await getTask.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Get task details for task that`s assigned to another user
        /// Given two users with assigned tasks
        /// When admin requests task by user id of another user
        /// Then he receives error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenTaskOfOtherUser(UserInfo userInfo, Guid otherUserId, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks).Id.Dump();

            // Act
            Func<Task> getTask = () => adminClient.Tasks.GetAsync(otherUserId, taskId);

            // Assert
            var exception = await getTask.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Get all user tasks
        /// Given user with assigned tasks
        /// When admin requests tasks assigned to user
        /// Then he receives only tasks that belong to requested user
        /// </summary>
        [Theory]
        public async Task ShouldGetTasks(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            var tasks = await adminClient.Tasks.GetAllAsync(userId);

            // Assert
            tasks.Should().NotBeNullOrEmpty();
            foreach (var task in tasks)
            {
                var expectedResult = task.State == TaskState.Completed
                    ? TaskResult.Passed
                    : (TaskResult?)null;

                task.Should().NotBeNull();
                using (var scope = new AssertionScope())
                {
                    task.Id.Should().NotBeEmpty();
                    task.Type.Should().HaveValue();
                    task.Variant.Should().NotBeNull();
                    task.Variant?.Id.Should().NotBeEmpty();
                    task.Variant?.Name.Should().NotBeNullOrEmpty();
                    task.State.Should().HaveValue();
                    task.Result.Should().Be(expectedResult);
                    task.CreatedAt.Should().NotBe(default);
                    task.CollectionSteps.Should().NotBeNullOrEmpty();
                    task.Checks.Should().NotBeNull();
                }
                foreach (var step in task.CollectionSteps)
                {
                    var isStepCompleted = step.State == CollectionStepState.Completed;
                    var expectedReviewResult = step.IsReviewNeeded && isStepCompleted
                        ? CollectionStepReviewResult.Approved
                        : (CollectionStepReviewResult?)null;

                    using var scope = new AssertionScope();
                    step.Should().NotBeNull();
                    step.Id.Should().NotBeEmpty();
                    step.State.Should().HaveValue();
                    step.ReviewResult.Should().Be(expectedReviewResult);
                    step.RequestedAt.Should().NotBe(default);
                    step.UpdatedAt.Should().NotBe(default);
                    step.FormattedData.Should().BeSomeOrNull(_ => isStepCompleted);
                    ////step.Data.Should().BeSomeOrNull(_ => isStepCompleted);
                    step.Variant.Should().NotBeNull();
                    step.Variant.Should().BeAssignableTo<CollectionStepVariantDto>();
                    step.Variant.Name.Should().NotBeNull();
                }
                foreach (var check in task.Checks)
                {
                    var isCheckCompleted = check.State == CheckState.Complete;

                    using var scope = new AssertionScope();
                    check.Id.Should().NotBeEmpty();
                    check.State.Should().HaveValue();
                    check.Result.Should().BeSomeOrNull(_ => isCheckCompleted);
                    check.Type.Should().HaveValue();
                    check.Variant.Should().NotBeNull();
                    check.Variant.Id.Should().NotBeEmpty();
                    check.Variant.Name.Should().NotBeNullOrEmpty();
                    check.Variant.Provider.Should().HaveValue();
                    ////check.RelatedTasks.Should().NotBeEmpty(); // TODO: WRXB-10617 Fix api to return RelatedTasks
                    check.CompletedAt.Should().NotBe(default);
                }
            }
        }

        /// <summary>
        /// Scenario: Get all user tasks
        /// Given user without assigned tasks
        /// When admin requests tasks assigned to user
        /// Then he receives empty collection
        /// </summary>
        [Theory]
        public async Task ShouldGetEmptyArray_WhenUserHasNoTasks(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _profileFixture.CreateAsync(userInfo);

            // Act
            var tasks = await adminClient.Tasks.GetAllAsync(userId);

            // Assert
            tasks.Should().BeEmpty();
        }

        /// <summary>
        /// Scenario: Move completed task to incomplete
        /// Given user with completed task
        /// When admin moves task to incomplete
        /// Then task becomes incomplete
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Test is working, but flaky due to strange behavior of service, now it is ok. See test for more information")]
        public async Task ShouldIncompleteTask_WhenTaskCompleted(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, _excludedTasks).Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed); // Task can be completed before all check completed events processed by TaskEventHandler

            // Arrange
            var reason = new ReasonDto(nameof(ShouldIncompleteTask_WhenTaskCompleted));

            // Act
            await adminClient.Tasks.IncompleteAsync(reason, userId, taskId);

            // Assert
            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(adminClient.CorrelationId);
            var task = await adminClient.Tasks.GetAsync(userId, taskId);
            task.State.Should().Be(TaskState.Incomplete); // Task can be completed before last check completed events processed after task incomplete operation
            task.Result.Should().BeNull();
        }

        /// <summary>
        /// Scenario: Move incomplete task to incomplete
        /// Given user with incomplete task
        /// When admin moves task to incomplete
        /// Then task state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldKeepTaskUnchanged_WhenMoveIncompleteTaskToIncomplete(UserInfo userInfo, Seed seed)
        {
            // Given
            var reason = new ReasonDto(nameof(ShouldKeepTaskUnchanged_WhenMoveIncompleteTaskToIncomplete));

            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, _excludedTasks).Id.Dump();
            await adminClient.Tasks.IncompleteAsync(reason, userId, taskId);
            adminClient = _adminApiClientFactory.Create(admin);

            // Act
            await adminClient.Tasks.IncompleteAsync(reason, userId, taskId);

            // Assert
            _eventsFixture.ShouldNotExist<TaskIncompleteEvent>(adminClient.CorrelationId);
            var task = await adminClient.Tasks.GetAsync(userId, taskId);
            task.State.Should().Be(TaskState.Incomplete);
            task.Result.Should().BeNull();

        }

        /// <summary>
        /// Scenario: Complete task with "Failed" result
        /// Given user with incomplete task
        /// When admin completes task with "Failed" result
        /// Then task state becomes "Completed" 
        /// And task result becomes "Failed"
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Test is working, but flaky due to strange behavior of service, now it is ok. See test for more information")]
        public async Task ShouldCompleteTask_WhenTaskResultFailed(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks, _excludedTasks);
            var taskId = (task.Id, task.Type).Dump().Id;

            await _taskFixture.CompleteAsync(userId, taskId, seed); // Task can be completed before all check completed events processed by TaskEventHandler
            await _taskFixture.IncompleteAsync(userId, taskId);

            // Arrange
            var request = new CompleteTaskRequest(TaskResult.Failed, nameof(ShouldCompleteTask_WhenTaskResultFailed));

            // Act
            await adminClient.Tasks.CompleteAsync(request, userId, taskId); // Can occur error because last CheckCompletedEvent processed after task incomplete operation

            // Assert
            _eventsFixture.ShouldExistSingle<TaskCompletedEvent>(adminClient.CorrelationId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Completed);
            actualTask.Result.Should().Be(TaskResult.Failed);
        }

        /// <summary>
        /// Scenario: Complete task with "Passed" result
        /// Given user with incomplete task
        /// And all acceptance checks are completed
        /// And all task steps are completed
        /// When admin completes task with "Passed" result
        /// Then task state becomes "Completed" 
        /// And task result becomes "Passed"
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Test is working, but flaky due to strange behavior of service, now it is ok. See test for more information")]
        public async Task ShouldCompleteTask_WhenTaskResultPassed(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks, _excludedTasks);
            var taskId = (task.Id, task.Type).Dump().Id;

            await _taskFixture.CompleteAsync(userId, taskId, seed); // Task can be completed before all check completed events processed by TaskEventHandler
            await _taskFixture.IncompleteAsync(userId, taskId);

            // Arrange
            var request = new CompleteTaskRequest(TaskResult.Passed, nameof(ShouldCompleteTask_WhenTaskResultPassed));

            // Act
            await adminClient.Tasks.CompleteAsync(request, userId, taskId);

            // Assert
            // Can not find event because task is completed by last CheckCompletedEvent after task incomplete operation
            _eventsFixture.ShouldExistSingle<TaskCompletedEvent>(adminClient.CorrelationId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Completed);
            actualTask.Result.Should().Be(TaskResult.Passed);
        }

        /// <summary>
        /// Scenario: Complete task with not completed collection steps
        /// Given user with incomplete task
        /// And all acceptance checks are completed
        /// And task collection steps are not completed
        /// When admin tries to complete task with "Passed" result
        /// Then he receives error response with status code "Bad Request" 
        /// And task state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCompleteTaskWithNotCompletedSteps(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = (await adminClient.Tasks.GetAllAsync(userId))
                            .Where(task => task.State is TaskState.Incomplete);
            var task = faker.PickRandom(tasks, _excludedTasks.Append(TaskType.Address));
            var taskId = task.Id.Dump();
            await task.CollectionSteps.Where(step => step.State is CollectionStepState.Requested)
                                      .Skip(1)
                                      .ForeachConsistently(step => _stepsFixture.CompleteAsync(userId, step.Id, seed));

            // Arrange
            var request = new CompleteTaskRequest(TaskResult.Passed, nameof(ShouldGetError_WhenCompleteTaskWithNotCompletedSteps));

            // Act
            Func<Task> completeTask = () => adminClient.Tasks.CompleteAsync(request, userId, taskId);

            // Assert
            var exception = await completeTask.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: Complete task with not completed checks
        /// Given user with incomplete task
        /// And acceptance checks are not completed
        /// When admin tries to complete task with "Passed" result
        /// Then he receives error response with status code "Bad Request"
        /// And task state is not changed
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCompleteTaskWithNotCompletedChecks(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks.Where(t => t.Checks.Any()), _excludedTasks);
            var taskId = task.Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            var check = faker.PickRandom(task.Checks.OrderBy(check => check.Type).ToArray());
            var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);
            var stepVariant = faker.PickRandom(requiredData);

            await _stepsFixture.RequestAsync(userId, stepVariant, false, true, new[] { taskId });
            await _checksFixture.RequestAsync(userId, check.Variant.Id, new[] { taskId });

            // Arrange
            var request = new CompleteTaskRequest(TaskResult.Passed, nameof(ShouldGetError_WhenCompleteTaskWithNotCompletedChecks));

            // Act
            Func<Task> completeTask = () => adminClient.Tasks.CompleteAsync(request, userId, taskId);

            // Assert
            var exception = await completeTask.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Scenario: Complete task again with same result
        /// Given user with completed task
        /// And with "Passed" task result
        /// When admin tries to complete task with "Passed" result
        /// Then task state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldKeepTaskUnchanged_WhenCompleteWithSameResult(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, _excludedTasks).Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            // Arrange
            var request = new CompleteTaskRequest(TaskResult.Passed, nameof(ShouldKeepTaskUnchanged_WhenCompleteWithSameResult));

            // Act
            await adminClient.Tasks.CompleteAsync(request, userId, taskId);

            // Assert
            _eventsFixture.ShouldNotExist<TaskCompletedEvent>(adminClient.CorrelationId);
            var task = await adminClient.Tasks.GetAsync(userId, taskId);
            task.State.Should().Be(TaskState.Completed);
            task.Result.Should().Be(TaskResult.Passed);
        }

        /// <summary>
        /// Scenario: Complete task again with different result
        /// Given user with completed task
        /// And with "Passed" task result
        /// When admin tries to complete task with passed "Failed" result
        /// Then he receives error response with status code "Conflict"
        /// And task state is not changed
        /// And no events are raised
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenCompleteWithDifferentResult(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var taskId = faker.PickRandom(tasks, _excludedTasks).Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            // Arrange
            var request = new CompleteTaskRequest(TaskResult.Failed, nameof(ShouldGetError_WhenCompleteWithDifferentResult));

            // Act
            Func<Task> completeTask = () => adminClient.Tasks.CompleteAsync(request, userId, taskId);

            // Assert
            var exception = await completeTask.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        /// <summary>
        /// Scenario: Admin removes collection step from task
        /// Given user with incomplete task
        /// And requested collection step assigned to task
        /// When admin removes "Requested" collection step
        /// Then given collection step unassigned from this task
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRemoveTaskCollectionStep(UserInfo userInfo, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var tasksWithRequestedSteps = tasks.Where(HasRequestedCollectionSteps);
            var targetTask = faker.PickRandom(tasksWithRequestedSteps);
            var stepToRemove = faker.PickRandom(targetTask.CollectionSteps);

            // Act
            await adminClient.Tasks.RemoveCollectionStepAsync(userId, targetTask.Id, stepToRemove.Id);

            // Assert
            var updatedTask = await adminClient.Tasks.GetAsync(userId, targetTask.Id);
            updatedTask.CollectionSteps.Should().NotContain(x => x.Id == stepToRemove.Id);

            static bool HasRequestedCollectionSteps(TaskDto task) =>
                task.CollectionSteps.Any(step => step.State == CollectionStepState.Requested);
        }

        /// <summary>
        /// Scenario: Admin removes collection step from non existing task
        /// Given user without tasks
        /// And with several collection steps in "Requested" state
        /// When admin removes collection step from task which does not exist
        /// Then he receives error response with status code "Not Found"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenRemoveTaskWhichDoesNotExist(UserInfo userInfo, Guid taskId, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var collectionSteps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requestedCollectionSteps = collectionSteps.Where(step => step.State == CollectionStepState.Requested);
            var collectionStep = faker.PickRandom(requestedCollectionSteps);

            // Act
            Func<Task> task = () => adminClient.Tasks.RemoveCollectionStepWithHttpMessagesAsync(userId, taskId, collectionStep.Id);

            // Assert
            var exception = await task.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Scenario: Admin removes non existing collection step from task
        /// Given user with incomplete task
        /// When admin removes collection step which does not exist
        /// Then he receives error response with status code "Internal Server Error"
        /// </summary>
        [Theory]
        public async Task ShouldGetError_WhenRemoveCollectionStepWhichNotExist(UserInfo userInfo, Guid collectionStepId, Seed seed)
        {
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            // Given
            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Arrange
            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var incompleteTasks = tasks.Where(task => task.State == TaskState.Incomplete);
            var incompleteTask = faker.PickRandom(incompleteTasks);

            // Act
            Func< Task> action = () => adminClient.Tasks.RemoveCollectionStepWithHttpMessagesAsync(userId, incompleteTask.Id, collectionStepId);

            // Assert
            var exception = await action.Should().ThrowAsync<ErrorResponseException>();
            exception.Which.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
