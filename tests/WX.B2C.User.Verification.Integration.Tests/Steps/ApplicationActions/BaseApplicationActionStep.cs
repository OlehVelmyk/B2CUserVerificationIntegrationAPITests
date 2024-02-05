using FluentAssertions;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;

internal abstract class BaseApplicationActionStep : BaseStep
{
    private readonly IPublicClient _pubApiClient;
    protected UserAction UserAction;

    protected BaseApplicationActionStep(IPublicClient pubApiClient, UserAction userAction)
    {
        _pubApiClient = pubApiClient;
        UserAction = userAction;
    }

    public override async Task PostCondition()
    {
        var actions = await _pubApiClient.ExecuteUntilAsync(
            client => client.Actions.GetAsync(),
            actions => actions.All(action => !UserAction.Equals(action)));

        actions.Should().NotContain(action => UserAction.Equals(action),
                                    "User should not have action {0} open but have {1}",
                                    UserAction.ToString(), actions.AsString());
    }
}
