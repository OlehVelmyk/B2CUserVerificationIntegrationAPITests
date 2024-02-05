using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Integration.Tests.Builders;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using IAdminClient = WX.B2C.User.Verification.Api.Admin.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminUpdateVerificationDetailsStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly Func<AdminUpdateVerificationDetailsRequestBuilder, AdminUpdateVerificationDetailsRequestBuilder> _buildRequest;
    
    public AdminUpdateVerificationDetailsStep(
        VerificationAdminApiClientFactory adminApiClientFactory, 
        Func<AdminUpdateVerificationDetailsRequestBuilder, AdminUpdateVerificationDetailsRequestBuilder> buildRequest)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _buildRequest = buildRequest;
    }
    
    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var requestBuilder = new AdminUpdateVerificationDetailsRequestBuilder();
        var adminApiClient = await _adminApiClientFactory.Create();

        var request = _buildRequest(requestBuilder).Build();
        await adminApiClient.Profile.UpdateAsync(request, userId);
    }
}

internal class AdminUpdateVerificationDetailsStepFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public AdminUpdateVerificationDetailsStepFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public AdminUpdateVerificationDetailsStep Create(
        Func<AdminUpdateVerificationDetailsRequestBuilder, AdminUpdateVerificationDetailsRequestBuilder> buildRequest) =>
            new (_adminApiClientFactory, buildRequest);
}
