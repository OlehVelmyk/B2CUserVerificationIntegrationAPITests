using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminRejectApplicationStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    
    public AdminRejectApplicationStep(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();
        
        var application = await adminApiClient.Applications.GetDefaultAsync(userId);
        var reason = ReasonProvider.CreateDto(callerMethod: nameof(AdminRejectApplicationStep));
        
        await adminApiClient.Applications.RejectAsync(reason, userId, application.Id);
    }
}
