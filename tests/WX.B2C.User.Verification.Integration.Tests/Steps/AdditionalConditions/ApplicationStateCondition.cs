using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using IAdminClient = WX.B2C.User.Verification.Api.Admin.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;

internal class ApplicationStateCondition : IAdditionalCondition
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly ApplicationState _applicationState;
    
    public ApplicationStateCondition(VerificationAdminApiClientFactory adminApiClientFactory,
                                     ApplicationState applicationState)
    {
        _applicationState = applicationState;
        _adminApiClientFactory = adminApiClientFactory;
    }

    public async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();
        var application = await adminApiClient.ExecuteUntilAsync(
            client => client.Applications.GetDefaultAsync(userId),
            application => application.State == _applicationState);

        application.State.Should().Be(_applicationState, $"After period of time application state do not became {_applicationState}");
    }
}

internal class ApplicationStateConditionFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    
    public ApplicationStateConditionFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public ApplicationStateCondition Create(ApplicationState applicationState) =>
        new (_adminApiClientFactory, applicationState);
}
