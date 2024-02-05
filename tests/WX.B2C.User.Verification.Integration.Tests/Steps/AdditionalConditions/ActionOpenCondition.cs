using FluentAssertions;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;

internal class ActionOpenCondition : IAdditionalCondition
{
    private readonly IPublicClient _pubApiClient;
    private readonly UserAction _action;

    public ActionOpenCondition(IPublicClient pubApiClient,
                               UserAction action)
    {
        _pubApiClient = pubApiClient;
        _action = action;
    }

    public async Task Execute()
    {
        var actions = await _pubApiClient.ExecuteUntilAsync(
            client => client.Actions.GetAsync(),
            actions => actions.Any(action => _action.Equals(action)));

        actions.Should().ContainSingle(action => _action.Equals(action),
                                       "User should have action {0} open but have {1}",
                                       _action.ToString(), actions.AsString());
    }
}

internal class ActionOpenConditionFactory
{
    private readonly IPublicClient _pubApiClient;

    public ActionOpenConditionFactory(IPublicClient pubApiClient)
    {
        _pubApiClient = pubApiClient;
    }

    public ActionOpenCondition Create(UserAction action) =>
        new(_pubApiClient, action);
}
