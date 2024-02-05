using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;

internal class ChecksCondition : IAdditionalCondition
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly UserCheck[] _requiredChecks;

    public ChecksCondition(VerificationAdminApiClientFactory adminApiClientFactory, 
                           UserCheck[] requiredChecks)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _requiredChecks = requiredChecks;
    }

    public async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var checks = await adminApiClient.ExecuteUntilAsync(
            client => client.Checks.GetAllAsync(userId),
            checks => IsChecksConditionPassed(_requiredChecks, checks.ToArray()));

        foreach (var group in _requiredChecks.GroupBy(check => check))
        {
            var requiredCheck = group.Key;
            var requiredChecks = group.ToArray();
            checks.Where(CreateMatchExpression(requiredCheck))
                  .Should().HaveSameCount(requiredChecks, "User should have {0} checks {1}\nWhen user has {2}",
                                          requiredChecks.Length, requiredCheck.ToString(), checks.Where(check => check.Type == requiredCheck.Type).AsString());
        }
    }

    private static Func<CheckDto, bool> CreateMatchExpression(UserCheck userCheck) =>
        check => userCheck.Type == check.Type &&
                 userCheck.Provider == check.Variant.Provider &&
                 userCheck.State == check.State &&
                 userCheck.Result == check.Result &&
                 userCheck.Decision == check.Decision;

    private static bool IsChecksConditionPassed(UserCheck[] requiredChecks, CheckDto[] checks)
    {
        foreach (var group in requiredChecks.GroupBy(check => check))
        {
            var requiredCheck = group.Key;
            var requiredChecksCount = group.ToArray().Length;

            if (checks.Count(CreateMatchExpression(requiredCheck)) != requiredChecksCount)
                return false;
        }
        
        return true;
    }
}

internal class ChecksConditionFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    
    public ChecksConditionFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public ChecksCondition Create(UserCheck[] requiredChecks) =>
        new(_adminApiClientFactory, requiredChecks);
}
