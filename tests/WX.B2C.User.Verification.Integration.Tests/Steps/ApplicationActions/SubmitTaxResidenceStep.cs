using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;

internal class SubmitTaxResidenceStep : BaseApplicationActionStep
{
    private readonly IPublicClient _publicClient;
    private string? _country;

    public SubmitTaxResidenceStep(IPublicClient publicClient) 
        : base(publicClient, new UserAction(ActionType.TaxResidence))
    {
        _publicClient = publicClient;
    }

    public override Task Execute()
    {
        return SubmitTaxResidence();
    }

    public SubmitTaxResidenceStep With(string country)
    {
        _country = country;
        return this;
    }

    private Task SubmitTaxResidence()
    {
        var request = new UpdateVerificationDetailsRequest
        {
            TaxResidence = new[]
            {
                _country ?? StepContext.Instance[General.TaxResidence]
            }
        };

        StepContext.Instance[General.CorrelationId] = Guid.NewGuid().ToString();

        return _publicClient.Profile.UpdateAsync(request);
    }
}
