using FluentAssertions;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps;

internal class CreateApplicationStep : BaseStep
{
    private Dictionary<string, List<string>> _customHeaders = new();
    private IList<UserAction> _requiredActions = new List<UserAction>(
        new[]
        {
            new UserAction(ActionType.Selfie),
            new UserAction(ActionType.ProofOfIdentity), 
            new UserAction(ActionType.TaxResidence)
        });

    private readonly IPublicClient _pubApiClient;

    public CreateApplicationStep(IPublicClient pubApiClient)
    {
        _pubApiClient = pubApiClient;
    }

    public CreateApplicationStep WithCustomHeaders(Dictionary<string, List<string>> customHeaders)
    {
        _customHeaders = customHeaders;
        return this;
    }

    public CreateApplicationStep AddAction(UserAction userAction)
    {
        _requiredActions.Add(userAction);
        return this;
    }
    
    public CreateApplicationStep ReWriteActions(UserAction[] actions)
    {
        if (!actions.Any())
            throw new InvalidOperationException($"{nameof(actions)} can not be empty");

        _requiredActions = new List<UserAction>(actions);
        return this;
    }

    public override Task Execute()
    {
        return _pubApiClient.Applications.RegisterWithHttpMessagesAsync(_customHeaders);
    }

    public override async Task PostCondition()
    {
        var actions = await _pubApiClient.ExecuteUntilAsync(
            client => client.Actions.GetAsync(),
            actions => _requiredActions.All(requiredAction => actions.Any(requiredAction.Equals)));

        _requiredActions.Should().AllSatisfy(requiredAction => actions.Should().Contain(action => requiredAction.Equals(action)));
    }
}
