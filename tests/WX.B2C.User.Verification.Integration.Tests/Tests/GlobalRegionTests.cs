using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Steps;
using WX.B2C.User.Verification.Integration.Tests.Steps.Admin;
using WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Tests;

[TestFixture]
internal class GlobalRegionTests : BaseTest
{
    private IFlow _flow;

    [SetUp]
    public void SetUp()
    {
        StepContext.Reset();
        _flow = new BaseFlow();
    }

    [Theory]
    public async Task ShouldApproveApplication()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Tr))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.ProofOfAddress))
                         .WithIpAddress(IpAddresses.Tr))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }
}
