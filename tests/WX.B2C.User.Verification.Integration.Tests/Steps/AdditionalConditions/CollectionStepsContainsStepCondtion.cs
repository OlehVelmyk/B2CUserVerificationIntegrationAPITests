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

internal class CollectionStepsContainsStepCondtion : IAdditionalCondition
{
    private readonly VerificationAdminApiClientFactory _verificationAdminApiClientFactory;
    private Func<CollectionStepDto, bool> _predicate;

    public CollectionStepsContainsStepCondtion(
        VerificationAdminApiClientFactory verificationAdminApiClientFactory,
        Func<CollectionStepDto, bool> predicate)
    {
        _verificationAdminApiClientFactory = verificationAdminApiClientFactory;
        _predicate = predicate;
    }

    public async Task Execute()
    {
        var adminClient = await _verificationAdminApiClientFactory.Create();
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);

        var response = await adminClient
            .ExecuteUntilAsync(client => client.CollectionStep.GetAllAsync(userId),
                collectionStepDtos =>
                    collectionStepDtos.Any(collectionStepDto => _predicate(collectionStepDto)));

        response.Should().ContainSingle(collectionStepDto =>
            _predicate(collectionStepDto));
    }
}

internal class CollectionStepsContainsStepCondtionFactory
{
    private readonly VerificationAdminApiClientFactory _verificationAdminApiClientFactory;

    public CollectionStepsContainsStepCondtionFactory(
        VerificationAdminApiClientFactory verificationAdminApiClientFactory)
    {
        _verificationAdminApiClientFactory = verificationAdminApiClientFactory;
    }

    public CollectionStepsContainsStepCondtion Create(Func<CollectionStepDto, bool> predicate) =>
        new(_verificationAdminApiClientFactory, predicate);
}