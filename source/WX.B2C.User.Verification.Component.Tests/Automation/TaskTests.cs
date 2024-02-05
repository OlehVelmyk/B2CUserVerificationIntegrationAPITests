using System;
using System.Collections.Generic;
using System.Linq;
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
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class TaskTests : BaseComponentTest
    {
        private readonly IReadOnlyCollection<TaskType> _excludedTasks = new[] { TaskType.RiskListsScreening, TaskType.Address };

        private ApplicationFixture _applicationFixture;
        private TaskFixture _taskFixture;
        private CollectionStepsFixture _stepsFixture;
        private ChecksFixture _checksFixture;
        private ProfileFixture _profileFixture;
        private ExternalProfileFixture _externalProfileFixture;
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
            _externalProfileFixture = Resolve<ExternalProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _administratorFactory = Resolve<AdministratorFactory>();
            _checkProvider = Resolve<ICheckProvider>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<GlobalUserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Add required collection step to task
        /// Given user with completed task
        /// When request new required collection step
        /// Then task state becomes "Incomplete"
        /// And task collection steps are extended by requested step
        /// And event task incomplete is raised
        /// And event task step added is raised too
        /// </summary>
        [Theory]
        public async Task ShouldIncompleteTask_WhenAddRequiredStep(UserInfo userInfo, Seed seed)
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
            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldIncompleteTask_WhenAddRequiredStep),
                Type = CollectionStepType.Survey,
                IsRequired = true,
                IsReviewNeeded = false,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { taskId }
            };

            // Act
            var stepId = await _stepsFixture.RequestAsync(userId, request);

            // Assert
            _eventsFixture.ShouldExistSingle<TaskCollectionStepAddedEvent>(
            e => e.EventArgs.TaskId == taskId && e.EventArgs.CollectionStepId == stepId);
            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId);
            var task = await adminClient.Tasks.GetAsync(userId, taskId);
            task.State.Should().Be(TaskState.Incomplete);
            task.Result.Should().BeNull();
        }

        /// <summary>
        /// Scenario: Add optional collection step to task
        /// Given user with completed task
        /// When request new optional collection step
        /// Then task state is not changed
        /// And only event new task step added is raised
        /// </summary>
        [Theory]
        public async Task ShouldKeepTaskStateUnchanged_WhenAddOptionalStep(UserInfo userInfo, Seed seed)
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
            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldKeepTaskStateUnchanged_WhenAddOptionalStep),
                Type = CollectionStepType.Survey,
                IsRequired = false,
                IsReviewNeeded = false,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { taskId }
            };

            // Act
            var stepId = await _stepsFixture.RequestAsync(userId, request);

            // Assert
            _eventsFixture.ShouldExistSingle<TaskCollectionStepAddedEvent>(
            e => e.EventArgs.TaskId == taskId && e.EventArgs.CollectionStepId == stepId);
            _eventsFixture.ShouldNotExist<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId);
            var task = await adminClient.Tasks.GetAsync(userId, taskId);
            task.State.Should().Be(TaskState.Completed);
            task.Result.Should().NotBeNull();
        }

        /// <summary>
        /// Scenario: Make collection step required
        /// Given user with completed task
        /// And having optional uncompleted step 
        /// When request collection step became required
        /// Then task must be incomplete
        /// </summary>
        [Theory]
        public async Task ShouldIncompleteTaskState_WhenOptionalStepBecameRequired(UserInfo userInfo, Seed seed, Guid actCorrelationId)
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
            var templateId = Guid.NewGuid();
            var requestOptionalStep = BuildStepRequest(false);
            var stepId = await _stepsFixture.RequestAsync(userId, requestOptionalStep);

            // Arrange
            var requestRequiredStep = BuildStepRequest(true);
            adminClient.CorrelationId = actCorrelationId;

            // Act
            await adminClient.CollectionStep.RequestAsync(requestRequiredStep, userId);

            // Assert
            _eventsFixture.ShouldExistSingle<CollectionStepRequiredEvent>(e => e.EventArgs.CollectionStepId == stepId);
            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId && e.CorrelationId == actCorrelationId);
           
            var task = await adminClient.Tasks.GetAsync(userId, taskId);
            task.State.Should().Be(TaskState.Incomplete);

            SurveyCollectionStepRequest BuildStepRequest(bool isRequired) =>
                new()
                {
                    Reason = nameof(ShouldIncompleteTaskState_WhenOptionalStepBecameRequired),
                    Type = CollectionStepType.Survey,
                    IsRequired = isRequired,
                    IsReviewNeeded = false,
                    TemplateId = templateId,
                    TargetTasks = new[] { taskId }
                };
        }

        /// <summary>
        /// Scenario: Check is completed with "Failed" result
        /// Given user with completed task
        /// And task contains instructed checks
        /// When task check is completed with "Failed" result
        /// Then task state becomes "Incomplete"
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldIncompleteTask_WhenCheckFails(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            _eventsFixture.ShouldExistSingle<TaskCreatedEvent>(
                e => e.EventArgs.UserId == userId && e.EventArgs.Type.ToString() == TaskType.Identity.ToString());
            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = tasks.First(task => task.Type == TaskType.Identity);
            var taskId = task.Id;

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.IdentityDocument);
            var checkOptions = OnfidoCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            var check = task.Checks.First(check => check.Type == CheckType.IdentityDocument);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, new[] { taskId });

            // Assert
            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Incomplete);
            actualTask.Result.Should().BeNull();
        }

        /// <summary>
        /// Scenario: Complete last task step in task
        /// Given user with incomplete task
        /// And all task collection steps are completed except one
        /// And all task checks are completed
        /// When complete last task step in task
        /// Then task state becomes "Completed"
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldCompleteTask_WhenCompleteLastStep(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks, _excludedTasks);
            var taskId = task.Id.Dump();

            // Act
            var stepIds = task.CollectionSteps.Select(step => step.Id);
            await _stepsFixture.CompleteAllAsync(userId, stepIds, false, seed);

            // Assert
            _eventsFixture.ShouldExist<TaskCompletedEvent>(e => e.EventArgs.Id == taskId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Completed);
            actualTask.Result.Should().Be(TaskResult.Passed);
        }

        /// <summary>
        /// Scenario: Complete last task check in task
        /// Given user with incomplete task
        /// And all task collection steps are completed 
        /// And all task checks are completed except one
        /// When complete last check in task
        /// Then task state becomes "Completed"
        /// And event is raised
        /// </summary>
        [Theory, Ignore("Test is working, but flaky due to strange behavior of service, now it is ok. See test for more information")]
        public async Task ShouldCompleteTask_WhenCompleteLastCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks.Where(t => t.Checks.Any()), new[] { TaskType.Address });
            var taskId = task.Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed); // Task can be completed before all check completed events processed by TaskEventHandler
            await _taskFixture.IncompleteAsync(userId, taskId);

            // Arrange
            var check = FakerFactory.Create(seed).PickRandom(task.Checks.OrderBy(check => check.Type).ToArray());

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, new[] { taskId });

            // Assert
            // Can not find second event because task is completed by last CheckCompletedEvent after task incomplete operation with same correlation id
            _eventsFixture.ShouldExistExact<TaskCompletedEvent>(2, e => e.EventArgs.Id == taskId);
        }

        /// <summary>
        /// Scenario: Successfully complete new check
        /// Given user with completed task
        /// When admin rquests new check
        /// And check is completed successfully
        /// Then task state is not changed
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldKeepTaskStateUnchanged_WhenSuccessfullyCompleteNewCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = faker.PickRandom(tasks.Where(t => t.Checks.Any()), new[] { TaskType.Address });
            var taskId = task.Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            // Arrange
            var check = FakerFactory.Create(seed).PickRandom(task.Checks.OrderBy(check => check.Type).ToArray());

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, new[] { taskId });

            // Assert
            _eventsFixture.ShouldNotExist<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId);
        }

        /// <summary>
        /// Scenario: Move task collection step into "ReviewNeeded" state
        /// Given user with incomplete task
        /// And task contains task steps
        /// When submit data for last task step
        /// And step requires review
        /// Then task state is not changed
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldKeepTaskStateUnchanged_WhenMoveStepIntoReviewNeeded(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = (await adminClient.Tasks.GetAllAsync(userId))
                .Where(task => task.State is TaskState.Incomplete);
            var task = faker.PickRandom(tasks, _excludedTasks);
            var taskId = task.Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldKeepTaskStateUnchanged_WhenMoveStepIntoReviewNeeded),
                Type = CollectionStepType.Survey,
                IsRequired = true,
                IsReviewNeeded = true,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { taskId }
            };
            var stepId = await _stepsFixture.RequestAsync(userId, request);
            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId);

            // Act
            await _stepsFixture.MoveInReviewAsync(userId, stepId, seed);

            // Assert
            _eventsFixture.ShouldNotExistExact<TaskCompletedEvent>(2, e => e.EventArgs.Id == taskId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Incomplete);
            actualTask.Result.Should().BeNull();
        }

        /// <summary>
        /// Scenario: Review task collection step that requires review
        /// Given user with incomplete task
        /// And task contains task steps
        /// And one step requires review
        /// When review last task step as 'Approved'
        /// Then task state is not changed
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldCompleteTask_WhenApproveStep(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = (await adminClient.Tasks.GetAllAsync(userId))
                .Where(task => task.State is TaskState.Incomplete);
            var task = faker.PickRandom(tasks, _excludedTasks);
            var taskId = task.Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldCompleteTask_WhenApproveStep),
                Type = CollectionStepType.Survey,
                IsRequired = true,
                IsReviewNeeded = true,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { taskId }
            };
            var stepId = await _stepsFixture.RequestAsync(userId, request);
            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId);

            // Act
            await _stepsFixture.MoveInReviewAsync(userId, stepId, seed);
            await _stepsFixture.ReviewAsync(userId, stepId, CollectionStepReviewResult.Approved);

            // Assert
            _eventsFixture.ShouldExistExact<TaskCompletedEvent>(2, e => e.EventArgs.Id == taskId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Completed);
            actualTask.Result.Should().Be(TaskResult.Passed);
        }

        /// <summary>
        /// Scenario: Review task collection step that requires review (Rejected review result)
        /// Given user with incomplete task
        /// And task contains task steps
        /// And one step requires review
        /// When review last task step as 'Rejected'
        /// Then task state is not changed
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldKeepTaskStateUnchanged_WhenRejectStep(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var faker = FakerFactory.Create(seed);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = (await adminClient.Tasks.GetAllAsync(userId))
                .Where(task => task.State is TaskState.Incomplete);
            var task = faker.PickRandom(tasks, _excludedTasks);
            var taskId = task.Id.Dump();

            await _taskFixture.CompleteAsync(userId, taskId, seed);

            var request = new SurveyCollectionStepRequest
            {
                Reason = nameof(ShouldKeepTaskStateUnchanged_WhenRejectStep),
                Type = CollectionStepType.Survey,
                IsRequired = true,
                IsReviewNeeded = true,
                TemplateId = Guid.NewGuid(),
                TargetTasks = new[] { taskId }
            };
            var stepId = await _stepsFixture.RequestAsync(userId, request);
            _eventsFixture.ShouldExistSingle<TaskIncompleteEvent>(e => e.EventArgs.Id == taskId);

            // Act
            await _stepsFixture.MoveInReviewAsync(userId, stepId, seed);
            await _stepsFixture.ReviewAsync(userId, stepId, CollectionStepReviewResult.Rejected);

            // Assert
            _eventsFixture.ShouldNotExistExact<TaskCompletedEvent>(2, e => e.EventArgs.Id == taskId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Incomplete);
            actualTask.Result.Should().BeNull();
        }

        /// <summary>
        /// Scenario: Complete task with "None" auto complete policy
        /// Given user with incomplete task
        /// And task has "None" auto complete policy
        /// When complete last task step and task check
        /// Then task state is not changed
        /// </summary>
        [Theory, Ignore("TODO: Think how to implement")]
        public async Task ShouldKeepTaskStateUnchanged_WhenAutoCompletePolicyIsNone(GlobalUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = tasks.First(task => task.Type is TaskType.Address);
            var taskId = task.Id.Dump();

            // Act
            var steps = task.CollectionSteps.Where(step => step.State is CollectionStepState.Requested);
            await _stepsFixture.CompleteAllAsync(userId, steps.Select(s => s.Id), true, seed);

            // Assert
            _eventsFixture.ShouldNotExist<TaskCompletedEvent>(e => e.EventArgs.Id == taskId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Incomplete);
            actualTask.Result.Should().BeNull();
        }

        /// <summary>
        /// Scenario: Complete task with "None" auto complete policy
        /// Given user with incomplete task
        /// And task has "None" auto complete policy
        /// And all task checks are completed
        /// And all task collection steps are completed
        /// When admin completes task 
        /// Then task state becomes "Completed"
        /// And event is raised
        /// </summary>
        [Theory, Ignore("TODO: Think how to implement")]
        public async Task ShouldCompleteTask_WhenAutoCompletePolicyIsNone(GlobalUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var task = tasks.First(task => task.Type is TaskType.Address);
            var taskId = task.Id.Dump();

            var steps = task.CollectionSteps.Where(step => step.State is CollectionStepState.Requested);
            await _stepsFixture.CompleteAllAsync(userId, steps.Select(s => s.Id), false, seed);

            // Arrange
            var request = new CompleteTaskRequest(TaskResult.Passed, nameof(ShouldCompleteTask_WhenAutoCompletePolicyIsNone));

            // Act
            await adminClient.Tasks.CompleteAsync(request, userId, taskId);

            // Assert
            _eventsFixture.ShouldExistSingle<TaskCompletedEvent>(e => e.EventArgs.Id == taskId);
            var actualTask = await adminClient.Tasks.GetAsync(userId, taskId);
            actualTask.State.Should().Be(TaskState.Completed);
            actualTask.Result.Should().Be(TaskResult.Passed);
        }
    }
}