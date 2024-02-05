using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using PublicEvents = WX.B2C.User.Verification.Events.Events;

namespace WX.B2C.User.Verification.Component.Tests.Public
{
    internal partial class ApplicationTests
    {
        private async Task ShouldAssignTasks(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _administratorFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var correlationId = Guid.NewGuid();
            var publicClient = _publicApiClientFactory.Create(userId, userInfo.IpAddress, correlationId);

            await _profileFixture.CreateAsync(userInfo);

            // Arrange 
            var expectedTasks = ApplicationFixture.GetOnboardingTasks(userInfo);
            var expectedChecks = ApplicationFixture.GetAcceptanceChecks(userInfo);

            // Act
            await publicClient.Applications.RegisterAsync();

            // Assert
            _eventsFixture.ShouldExistSingle<PublicEvents.ApplicationRegisteredEvent>(correlationId);

            expectedTasks.Select(taskType => _eventsFixture.ShouldExistSingle<TaskCreatedEvent>(e => IsCreated(e, taskType)))
                         .Select(e => e.EventArgs.TaskId)
                         .Foreach(taskId => _eventsFixture.ShouldExistSingle<ApplicationRequiredTaskAddedEvent>(e => IsAdded(e, taskId)));

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            tasks.Select(task => task.Type.To<TaskType>()).Should().BeEquivalentTo(expectedTasks);

            await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Select(check => check.Type.To<CheckType>()).ContainsAll(expectedChecks));

            bool IsCreated(TaskCreatedEvent e, TaskType taskType) =>
                e.CorrelationId == correlationId && e.EventArgs.Type == taskType;

            bool IsAdded(ApplicationRequiredTaskAddedEvent e, Guid taskId) =>
                e.CorrelationId == correlationId && e.EventArgs.TaskId == taskId;
        }
    }
}
