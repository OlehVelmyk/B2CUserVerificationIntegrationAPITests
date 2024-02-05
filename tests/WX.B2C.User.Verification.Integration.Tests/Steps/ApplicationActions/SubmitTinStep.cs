using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;

internal class SubmitTinStep : BaseApplicationActionStep
{
    private TinType _type = TinType.SSN;
    private string _number = "650139522";
    private readonly IPublicClient _publicClient;

    public SubmitTinStep(IPublicClient publicClient) 
        : base(publicClient, new UserAction(ActionType.Tin))
    {
        _publicClient = publicClient;
    }

    public SubmitTinStep With(TinType tinType, string tinNumber)
    {
        _type = tinType;
        _number = tinNumber;
        return this;
    }

    public override Task Execute()
    {
        var request = new UpdateVerificationDetailsRequest
        {
            Tin = new TinDto
            {
                Type = _type,
                Number = _number
            }
        };

        return _publicClient.Profile.UpdateAsync(request);
    }
}
