using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Providers;
using IAdminClient = WX.B2C.User.Verification.Api.Admin.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminApproveTaskStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly TaskType _taskType;

    public AdminApproveTaskStep(VerificationAdminApiClientFactory adminApiClientFactory,
                                TaskType taskType)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _taskType = taskType;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();
        
        var tasks = await adminApiClient.Tasks.GetAllAsync(userId);
        var task = tasks.First(t => t.Type == _taskType);
        var request = new CompleteTaskRequest
        {
            Reason = ReasonProvider.Create("Manual task approve", nameof(AdminApproveTaskStep)),
            Result = TaskResult.Passed
        };

        await adminApiClient.Tasks.CompleteAsync(request, userId, task.Id);
    }
}

internal class AdminApproveTaskStepFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public AdminApproveTaskStepFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public AdminApproveTaskStep Create(TaskType taskType) =>
        new(_adminApiClientFactory, taskType);
}
