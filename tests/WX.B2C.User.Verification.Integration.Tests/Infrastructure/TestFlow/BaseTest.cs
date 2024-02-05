using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WX.B2C.Risks.Api.Admin.Client;
using WX.B2C.Risks.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.Preconditions;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;

internal class BaseTest
{
    protected static async Task VerifyApplicationApproved(CheckType[]? excludedChecks = null)
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await Resolve<VerificationAdminApiClientFactory>().Create();

        var application = await adminApiClient.ExecuteUntilAsync(
            client => client.Applications.GetDefaultAsync(userId),
            application => application.State is ApplicationState.Approved);

        application.State.Should().Be(ApplicationState.Approved);
        application.RequiredTasks.Should().OnlyContain(task => task.Result == TaskResult.Passed);
        var actions = await Resolve<IPublicClient>().Actions.GetAsync();
        actions.Should().BeEmpty();

        await VerifyUserChecks(excludedChecks);
    }
    
    protected static async Task VerifyApplicationState(ApplicationState expectedState)
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await Resolve<VerificationAdminApiClientFactory>().Create();

        var application = await adminApiClient.ExecuteUntilAsync(
             client => client.Applications.GetDefaultAsync(userId),
             application => application.State == expectedState);

        application.State.Should().Be(expectedState);
    }

    protected static T Resolve<T>()
    {
        return ServiceLocator.Provider.GetRequiredService<T>();
    }

    protected static async Task VerifyUserRiskLevel(RiskRating riskRating)
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await Resolve<RisksAdminApiClientFactory>().Create();

        var userRiskProfile = await adminApiClient.ExecuteUntilAsync(
            client => client.UserRisks.GetAsync(userId), 
            userRiskProfile => userRiskProfile.RiskRating == riskRating);

        userRiskProfile.RiskRating.Should().Be(riskRating);
    }

    protected static async Task VerifyUserChecks(CheckType[]? excludedChecks = null)
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await Resolve<VerificationAdminApiClientFactory>().Create();

        var checks = await adminApiClient.Checks.GetAllAsync(userId);
        if (excludedChecks is not null)
            checks = checks.Where(check => !excludedChecks.Contains(check.Type)).ToList();
        
        checks.Should().OnlyContain(check => check.State == CheckState.Complete &&
                                             check.Result == CheckResult.Passed);
    }
}
