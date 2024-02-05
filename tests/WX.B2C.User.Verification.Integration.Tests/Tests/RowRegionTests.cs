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
using IAdminClient = WX.B2C.User.Verification.Api.Admin.Client.IUserVerificationApiClient;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckType;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckProviderType;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckResult;

namespace WX.B2C.User.Verification.Integration.Tests.Tests;

internal class RowRegionTests : BaseTest
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
                         .WithAddress(UserAddresses.Md))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Md))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserHasUsTaxResidence()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Md))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Md))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(StepDecorator.Create(Resolve<SubmitTaxResidenceStep>()
                                              .With("US"))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new []
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
        await VerifyApplicationApproved(new [] { TaxResidence });
    }

    [Theory]
    public async Task ShouldRejectApplication_WhenPoaDocumentSubmitted()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Md))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
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
                         .Create(AdminApi.DocumentCategory.ProofOfAddress, AdminApi.CollectionStepReviewResult.Rejected))
            .AddStep(Resolve<AdminRejectApplicationStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationState(AdminApi.ApplicationState.Rejected);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenIpMatchCheckFailed()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Md))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
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

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsPep()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Md)
                         .WithFullName(UserNames.PepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Md))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<AdminRequestSurveyStepFactory>()
                                              .Create(SurveyType.PepSurvey, AdminApi.TaskType.RiskListsScreening))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed)
                                                       })))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(Resolve<AdminReviewSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                         .Create(builder => builder.WithIsPep(true)
                                                   .WithIsSanctioned(false)
                                                   .WithIsAdverseMedia(false)))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsNotPepAndPepSurveyIsNotRequested()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Md)
                         .WithFullName(UserNames.PepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Md))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(false)
                                                                        .WithIsSanctioned(false)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed)
                                                       })))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsNotPep()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Md)
                         .WithFullName(UserNames.PepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .WithIpAddress(IpAddresses.Md))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<AdminRequestSurveyStepFactory>()
                                              .Create(SurveyType.PepSurvey, AdminApi.TaskType.RiskListsScreening))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed)
                                                       })))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(Resolve<AdminReviewSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                         .Create(builder => builder.WithIsPep(false)
                                                   .WithIsSanctioned(false)
                                                   .WithIsAdverseMedia(false)))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
    }

    [Theory]
    public async Task ShouldNotChangeApplicationState_WhenUserReachFirstThreshold()
    {
        await ShouldApproveApplication();
        
        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) = 
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.First);

        _flow = new BaseFlow()
                .AddStep(Resolve<AdminSetRiskLevelStepFactory>()
                             .Create(RiskRating.High))
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

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }
    
    [Theory]
    public async Task ShouldApproveApplication_WhenUserReachSecondThreshold()
    {
        await ShouldApproveApplication();

        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) = 
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.First);
        var (secondThresholdTriggerVariantId, secondThresholdTurnoverAmount) = 
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.Second);

        _flow = new BaseFlow()
                .AddStep(Resolve<AdminSetRiskLevelStepFactory>()
                             .Create(RiskRating.High))
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

    [TestCase(RiskRating.Low)]
    [TestCase(RiskRating.Medium)]
    public async Task ShouldNotChangeApplicationState_WhenUserReachThirdThreshold(RiskRating riskRating)
    {
        await ShouldApproveApplication();

        var (thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount) = 
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.RoW, ThresholdInfo.Third);

        _flow = new BaseFlow()
                .AddStep(Resolve<AdminSetRiskLevelStepFactory>()
                             .Create(riskRating))
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds, AdminApi.TaskState.Completed))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds, isOptional: true)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.Other))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }
    
    [Theory]
    public async Task ShouldNotChangeApplicationState_WhenUserReachThirdThresholdAndUserHasHighRisk()
    {
        await ShouldApproveApplication_WhenUserReachSecondThreshold();
        
        var (_, secondThresholdTurnoverAmount) = 
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.Second); 
        var (thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount) = 
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.Third);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount - secondThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds, AdminApi.TaskState.Completed))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds, isOptional: true)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.Other))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }
    
    [TestCase(RiskRating.Low)]
    [TestCase(RiskRating.Medium)]
    public async Task ShouldApproveApplication_WhenUserReachFourthThreshold(RiskRating riskRating)
    {
        await ShouldApproveApplication();
        
        var (thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.RoW, ThresholdInfo.Third);
        var (fourthThresholdTriggerVariantId, fourthThresholdTurnoverAmount) = 
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.RoW, ThresholdInfo.Fourth);

        _flow = new BaseFlow()
                .AddStep(Resolve<AdminSetRiskLevelStepFactory>()
                             .Create(riskRating))
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds, AdminApi.TaskState.Completed))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds, true)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(fourthThresholdTriggerVariantId, fourthThresholdTurnoverAmount - thirdThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.InReview)))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.Other))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserReachFourthThresholdAndUserHasHighRisk()
    {
        await ShouldApproveApplication_WhenUserReachSecondThreshold();

        var (_, secondThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.Second);
        var (thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.Third);
        var (fourthThresholdTriggerVariantId, fourthThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.RoW, ThresholdInfo.Fourth);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(thirdThresholdTriggerVariantId, thirdThresholdTurnoverAmount - secondThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds, AdminApi.TaskState.Completed))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds, true)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(fourthThresholdTriggerVariantId, fourthThresholdTurnoverAmount - thirdThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.InReview)))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.Other))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }
}
