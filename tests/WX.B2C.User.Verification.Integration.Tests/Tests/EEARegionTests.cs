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

namespace WX.B2C.User.Verification.Integration.Tests.Tests;

[TestFixture]
internal class EeaRegionTests : BaseTest
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
    public async Task ShouldApproveApplication_WhenUserHasLowOrMediumRiskLevel(RiskRating riskRating, int userAge)
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAge(userAge))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey)))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey, riskRating));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserHasHighRiskLevel()
    {
        const int UserAge = 80;

        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAge(UserAge))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey)))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<SubmitSurveyStepFactory>()
                                              .Create(SurveyType.OnboardingSurvey))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfFunds)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.OccupationSurvey)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.SoFSurvey))))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OccupationSurvey))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.SoFSurvey))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(RiskRating.High);
    }

    [Theory]
    public async Task ShouldRejectApplication_WhenUserHasExtraHighRiskLevel()
    {
        const int UserAge = 80;

        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Nl)
                         .WithAge(UserAge))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Fr))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey, RiskRating.High));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationState(AdminApi.ApplicationState.Rejected);
        await VerifyUserRiskLevel(RiskRating.ExtraHigh);
    }

    #region Monitoring

    [TestCase(RiskRating.Low, 35)]
    [TestCase(RiskRating.Medium, 65)]
    public async Task ShouldNotChangeApplicationState_WhenUserReachFirstThreshold(RiskRating riskRating, int age)
    {
        await ShouldApproveApplication_WhenUserHasLowOrMediumRiskLevel(riskRating, age);

        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Eea, ThresholdInfo.First);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(firstThresholdTriggerVariantId, firstThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfAddress, isOptional: true)))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.OccupationSurvey, isOptional: true)))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.FinancialCondition, AdminApi.TaskState.Completed))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(Resolve<SubmitSurveyStepFactory>()
                             .Create(SurveyType.OccupationSurvey))
                .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                                  .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
                                      .AddPostCondition(Resolve<CollectionStepsContainsStepCondtionFactory>()
                                                            .Create(CollectionStepDtoFunctionsProvider.PoANotRequiredInReview)))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    [TestCase(RiskRating.Low, 35)]
    [TestCase(RiskRating.Medium, 65)]
    public async Task ShouldNotChangeApplicationState_WhenUserReachSecondThreshold(RiskRating riskRating, int age)
    {
        await ShouldNotChangeApplicationState_WhenUserReachFirstThreshold(riskRating, age);

        var (_, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Eea, ThresholdInfo.First);
        var (secondThresholdTriggerVariantId, secondThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Eea, ThresholdInfo.Second);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(secondThresholdTriggerVariantId, secondThresholdTurnoverAmount - firstThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds, isOptional: true)))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.SoFSurvey, isOptional: true)))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds, AdminApi.TaskState.Completed))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(Resolve<SubmitSurveyStepFactory>()
                             .Create(SurveyType.SoFSurvey))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    [TestCase(RiskRating.Low, 35)]
    [TestCase(RiskRating.Medium, 65)]
    public async Task ShouldNotChangeApplicationState_WhenRepeatingTurnoverReachedSeveralTimes(RiskRating riskRating, int age)
    {
        await ShouldNotChangeApplicationState_WhenUserReachSecondThreshold(riskRating, age);


        var (reapingThresholdTriggerVariantId, repeatingTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Eea, ThresholdInfo.RepeatingTurnoverThresholdStep);

        _flow = new BaseFlow()
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingTurnoverAmount))
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingTurnoverAmount));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    [TestCase(RiskRating.High)]
    [TestCase(RiskRating.ExtraHigh)]
    public async Task ShouldNotChangeApplicationState_WhenRepeatingTurnoverReachedSeveralTimesAndUserHasHighOrExtraHighRiskLevel(RiskRating riskRating)
    {
        await ShouldApproveApplication_WhenUserBecameHighOrExtraHighRiskLevelOnMonitoring(riskRating);

        var (reapingThresholdTriggerVariantId, repeatingTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Eea, ThresholdInfo.RepeatingTurnoverThresholdStep);
        var (_, repeatingThresholdInitialThresholdAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Eea, ThresholdInfo.RepeatingTurnoverInitialThreshold);

        _flow = new BaseFlow()
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingThresholdInitialThresholdAmount + repeatingTurnoverAmount))
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingTurnoverAmount))
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingTurnoverAmount));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [TestCase(RiskRating.High)]
    [TestCase(RiskRating.ExtraHigh)]
    public async Task ShouldApproveApplication_WhenUserBecameHighOrExtraHighRiskLevelOnMonitoring(RiskRating riskRating)
    {
        await ShouldApproveApplication_WhenUserHasLowOrMediumRiskLevel(RiskRating.Low, UserAges.LowRiskFactorAge);

        _flow = new BaseFlow()
            .AddStep(StepDecorator.Create(Resolve<AdminSetRiskLevelStepFactory>()
                                              .Create(riskRating))
                                  .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                        .Create(AdminApi.TaskType.ProofOfFunds, AdminApi.TaskState.Completed))
                                  .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                        .Create(AdminApi.TaskType.FinancialCondition, AdminApi.TaskState.Completed))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.OccupationSurvey, isOptional: true)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.SoFSurvey, isOptional: true)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfFunds, isOptional: true)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress, isOptional: true)))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.Approved)))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OccupationSurvey))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.SoFSurvey))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    #endregion
}
