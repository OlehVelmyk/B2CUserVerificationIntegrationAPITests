using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;

internal class TaskAssignedCondition : IAdditionalCondition
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly TaskType _taskType;
    private readonly TaskState _taskState;

    public TaskAssignedCondition(VerificationAdminApiClientFactory adminApiClientFactory,
                                 TaskType taskType,
                                 TaskState taskState)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _taskType = taskType;
        _taskState = taskState;
    }

    public async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();
        var tasks = await adminApiClient.ExecuteUntilAsync(
            client => client.Tasks.GetAllAsync(userId),
            tasks => tasks.FirstOrDefault(task => task.Type == _taskType && task.State == _taskState) is not null);

        tasks.Should().ContainSingle(task => task.Type == _taskType && task.State == _taskState);
    }
}

internal class TaskAssignedConditionFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public TaskAssignedConditionFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public TaskAssignedCondition Create(TaskType taskType, TaskState taskState = TaskState.Incomplete) =>
        new(_adminApiClientFactory, taskType, taskState);
}
