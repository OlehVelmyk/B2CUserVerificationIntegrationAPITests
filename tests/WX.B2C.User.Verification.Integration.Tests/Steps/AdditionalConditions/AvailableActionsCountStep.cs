using FluentAssertions;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;

internal class AvailableActionsCountStep : IAdditionalCondition
{
    private readonly IPublicClient _publicClient;
    private readonly int _expectedCount;

    public AvailableActionsCountStep(
        IPublicClient publicClient,
        int expectedCount)
    {
        _publicClient = publicClient;
        _expectedCount = expectedCount;
    }

    public async Task Execute()
    {
        var response = await _publicClient.ExecuteUntilAsync(client => client.Actions.GetAsync(),
            x => x.Count == _expectedCount);

        response.Count.Should().Be(_expectedCount, $"Expected to find {_expectedCount} actions but found {response.Count}");
    }
}

internal class AvailableActionsCountStepFactory
{
    private readonly IPublicClient _publicClient;

    public AvailableActionsCountStepFactory(IPublicClient publicClient)
    {
        _publicClient = publicClient;
    }

    public AvailableActionsCountStep Create(int expectedCount) =>
        new(_publicClient, expectedCount);
}