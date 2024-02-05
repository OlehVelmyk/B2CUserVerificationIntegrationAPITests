using WX.B2C.Risks.Api.Admin.Client;
using WX.B2C.Risks.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminSetRiskLevelStep : BaseStep
{
    private readonly RisksAdminApiClientFactory _risksAdminApiClientFactory;
    private readonly RiskRating _riskRating;

    public AdminSetRiskLevelStep(RisksAdminApiClientFactory risksAdminApiClientFactory,
                                 RiskRating riskRating)
    {
        _risksAdminApiClientFactory = risksAdminApiClientFactory;
        _riskRating = riskRating;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var client = await _risksAdminApiClientFactory.Create();

        await client.UserRisks.UpdateAsync(userId, new UpdateRiskRatingRequest
        {
            IgnoreReassessment = true,
            Reason = ReasonProvider.Create(),
            Initiator = nameof(AdminSetRiskLevelStep),
            RiskRating = _riskRating
        });
    }
}

internal class AdminSetRiskLevelStepFactory
{
    private readonly RisksAdminApiClientFactory _risksAdminApiClientFactory;

    public AdminSetRiskLevelStepFactory(RisksAdminApiClientFactory risksAdminApiClientFactory)
    {
        _risksAdminApiClientFactory = risksAdminApiClientFactory;
    }

    public AdminSetRiskLevelStep Create(RiskRating riskRating) =>
        new(_risksAdminApiClientFactory, riskRating);
}
