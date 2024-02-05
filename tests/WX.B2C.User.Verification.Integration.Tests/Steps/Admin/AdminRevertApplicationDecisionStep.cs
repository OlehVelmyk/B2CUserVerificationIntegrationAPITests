using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminRevertApplicationDecisionStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _clientFactory;
    private readonly ApplicationState _expectedState;

    public AdminRevertApplicationDecisionStep(VerificationAdminApiClientFactory clientFactory,
                                              ApplicationState expectedState)
    {
        _clientFactory = clientFactory;
        _expectedState = expectedState;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        using var adminApiClient = await _clientFactory.Create();

        var application = await adminApiClient.Applications.GetDefaultAsync(userId);

        await adminApiClient.Applications.RevertDecisionAsync(ReasonProvider.CreateDto(), userId, application.Id);
    }

    public override async Task PostCondition()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        using var adminApiClient = await _clientFactory.Create();

        var application = await adminApiClient.ExecuteUntilAsync(
             client => client.Applications.GetDefaultAsync(userId),
             application => application.State == _expectedState);

        application.State.Should().Be(_expectedState);
    }
}
internal class AdminRevertApplicationDecisionStepFactory
{
    private readonly VerificationAdminApiClientFactory _clientFactory;

    public AdminRevertApplicationDecisionStepFactory(VerificationAdminApiClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public AdminRevertApplicationDecisionStep Create(ApplicationState expectedState)
    {
        return new(_clientFactory, expectedState);
    }
}
