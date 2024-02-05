using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.Risks.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Providers;
using WX.B2C.User.Verification.Integration.Tests.Steps;
using WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;
using WX.B2C.User.Verification.Integration.Tests.Steps.Admin;
using WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckType;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckResult;

namespace WX.B2C.User.Verification.Integration.Tests.Tests;

[TestFixture]
internal class ApacRegionTests : BaseTest
{
    private IFlow _flow;

    [SetUp]
    public void SetUp()
    {
        StepContext.Reset();
        _flow = new BaseFlow();
    }

    [TestCase(RiskRating.Low, 35)]
    [TestCase(RiskRating.Medium, 65)]
    [TestCase(RiskRating.High, 80)]
    public async Task ShouldApproveApplication(RiskRating riskRating, int userAge)
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Au)
                         .WithAge(userAge))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Au))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTaxResidenceStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserHasUsTaxResidence()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Au))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Au))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(StepDecorator.Create(Resolve<SubmitTaxResidenceStep>()
                                              .With("US"))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(TaxResidence, result: Failed)
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.W9Form))))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Taxation, DocumentTypes.W9Form))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.Taxation))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.TaxResidence));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { TaxResidence });
        await VerifyUserRiskLevel(RiskRating.Low);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenIpMatchCheckFailed()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Au))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(IpMatch, result: Failed)
                                                       }))
                                  .AddPrecondition(Resolve<ActionOpenConditionFactory>()
                                                       .Create(new UserAction(ActionType.ProofOfAddress))))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.Address));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { IpMatch });
    }

    #region MonitoringTests

    [Theory]
    public async Task ShouldNotChangeApplicationState_WhenUserReachFirstThreshold()
    {
        await ShouldApproveApplication(RiskRating.High, 80);

        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Apac, ThresholdInfo.First);

        _flow = new BaseFlow()
            .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                              .Create(firstThresholdTriggerVariantId, firstThresholdTurnoverAmount))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress, isOptional: true)))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.Approved)))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserReachSecondThreshold()
    {
        await ShouldApproveApplication(RiskRating.High, 80);

        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Apac, ThresholdInfo.First);
        var (secondThresholdTriggerVariantId, secondThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Apac, ThresholdInfo.Second);

        _flow = new BaseFlow()
            .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                              .Create(firstThresholdTriggerVariantId, firstThresholdTurnoverAmount))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress, isOptional: true)))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.Approved)))
            .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                              .Create(secondThresholdTriggerVariantId, secondThresholdTurnoverAmount - firstThresholdTurnoverAmount))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress)))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.InReview)))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task PH_ShouldNotChangeApplicationState_WhenUserReachFirstThreshold()
    {
        await ApproveApplication_WhenUserFromPhAndHasHighRiskLevel();

        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Ph, ThresholdInfo.First);

        _flow = new BaseFlow()
            .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                              .Create(firstThresholdTriggerVariantId, firstThresholdTurnoverAmount))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress, isOptional: true)))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.Approved)))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task PH_ShouldApproveApplication_WhenUserReachSecondThreshold()
    {
        await ApproveApplication_WhenUserFromPhAndHasHighRiskLevel();

        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Ph, ThresholdInfo.First);
        var (secondThresholdTriggerVariantId, secondThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Ph, ThresholdInfo.Second);

        _flow = new BaseFlow()
            .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                              .Create(firstThresholdTriggerVariantId, firstThresholdTurnoverAmount))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress, isOptional: true)))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.Approved)))
            .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                              .Create(secondThresholdTriggerVariantId, secondThresholdTurnoverAmount - firstThresholdTurnoverAmount))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress)))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.InReview)))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    private async Task ApproveApplication_WhenUserFromPhAndHasHighRiskLevel()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                     .WithAddress(UserAddresses.Ph)
                     .WithAge(75))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Ph))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTaxResidenceStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(RiskRating.High);
    }

    #endregion
}
