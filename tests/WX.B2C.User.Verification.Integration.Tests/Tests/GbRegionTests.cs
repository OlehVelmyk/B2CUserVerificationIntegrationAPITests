using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.Risks.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Providers;
using WX.B2C.User.Verification.Integration.Tests.Steps;
using WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;
using WX.B2C.User.Verification.Integration.Tests.Steps.Admin;
using WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;
using WX.Integration.Tests.EventListener.Common;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;

using static WX.B2C.User.Verification.Integration.Tests.Constants.CheckDecisions;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckType;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckResult;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckState;
using static WX.B2C.User.Verification.Api.Admin.Client.Models.CheckProviderType;

namespace WX.B2C.User.Verification.Integration.Tests.Tests;

[TestFixture]
internal class GbRegionTests : BaseTest
{
    private IFlow _flow;
    private VerificationAdminApiClientFactory _adminApiClientFactory;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _adminApiClientFactory = Resolve<VerificationAdminApiClientFactory>();
    }

    [SetUp]
    public void SetUp()
    {
        StepContext.Reset();
        _flow = new BaseFlow();
    }

    [TestCase(RiskRating.Low, UserAges.LowRiskFactorAge)]
    [TestCase(RiskRating.Medium, UserAges.MediumRiskFactorAge)]
    public async Task ShouldApproveApplication_WhenUserHasLowOrMediumRiskRating(RiskRating riskRating, int userAge)
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithAge(userAge))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey, riskRating))
            .AddStep(Resolve<SubmitTaxResidenceStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserHasHighRiskLevel()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithAge(UserAges.HighRiskFactorAge))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<SubmitSurveyStepFactory>()
                                              .Create(SurveyType.OnboardingSurvey, RiskRating.High))
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
    public async Task ShouldApproveApplication_WhenUserHasHighRiskAndUsTaxResidence()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAge(UserAges.HighRiskFactorAge)
                         .WithAddress(UserAddresses.Gb))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>().With("US"))
            .AddStep(StepDecorator.Create(Resolve<SubmitSurveyStepFactory>()
                                              .Create(SurveyType.OnboardingSurvey, RiskRating.High))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(TaxResidence, result: Failed),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.W9Form))))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Taxation, DocumentTypes.W9Form))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.Taxation))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.TaxResidence))
            .AddStep(StepDecorator.Create(Resolve<SubmitSurveyStepFactory>()
                                              .Create(SurveyType.OccupationSurvey))
                                  .AddPrecondition(Resolve<ActionOpenConditionFactory>()
                                                       .Create(new UserAction(ActionType.ProofOfFunds)))
                                  .AddPrecondition(Resolve<ActionOpenConditionFactory>()
                                                       .Create(new UserAction(ActionType.ProofOfAddress)))
                                  .AddPrecondition(Resolve<ActionOpenConditionFactory>()
                                                       .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.OccupationSurvey)))
                                  .AddPrecondition(Resolve<ActionOpenConditionFactory>()
                                                       .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.SoFSurvey))))
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
        await VerifyApplicationApproved(new[] { TaxResidence });
        await VerifyUserRiskLevel(RiskRating.High);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserHasHighRiskAndResubmitPoa()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAge(UserAges.HighRiskFactorAge)
                         .WithAddress(UserAddresses.Gb))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<SubmitSurveyStepFactory>()
                                              .Create(SurveyType.OnboardingSurvey, RiskRating.High))
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
                         .Create(AdminApi.DocumentCategory.ProofOfFunds))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress, AdminApi.CollectionStepReviewResult.Rejected))
            .AddStep(Resolve<AdminRequestDocumentStepFactory>()
                         .Create(AdminApi.TaskType.Address, AdminApi.DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement, isReviewNeeded: true))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.Address));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(RiskRating.High);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserHasHighRiskAndResubmitPof()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAge(UserAges.HighRiskFactorAge)
                         .WithAddress(UserAddresses.Gb))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(StepDecorator.Create(Resolve<SubmitSurveyStepFactory>()
                                              .Create(SurveyType.OnboardingSurvey, RiskRating.High))
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
                         .Create(AdminApi.DocumentCategory.ProofOfFunds, AdminApi.CollectionStepReviewResult.Rejected))
            .AddStep(Resolve<AdminRequestDocumentStepFactory>()
                         .Create(AdminApi.TaskType.ProofOfFunds, AdminApi.DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement, isReviewNeeded: true))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfFunds))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(RiskRating.High);
    }

    [Theory]
    public async Task ShouldRejectApplication_WhenFaceDuplicationCheckFailedWithDecisionFraud()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAge(UserAges.HighRiskFactorAge)
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.OnfidoChecksConsiderUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo));

        await _flow.Execute();

        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        using var _ = new AssertionScope();
        await VerifyApplicationState(AdminApi.ApplicationState.Rejected);

        var checks = await adminApiClient.ExecuteUntilAsync(
            client => client.Checks.GetAllAsync(userId),
            checks => checks.Any(check => check.Type == FaceDuplication &&
                                          check.State == Complete));

        checks.Should().ContainSingle(check => check.Type == FaceDuplication &&
                                               check.Result == Failed &&
                                               check.Decision == Fraud);
    }

    [Theory]
    public async Task ShouldRejectApplication_WhenUserHasExtraHighRiskRating()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithAge(UserAges.ExtraHighRiskFactorAge))
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

    [Theory]
    public async Task ShouldApproveApplication_WhenUserHasUsTaxResidence()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>().With("US"))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.Taxation, DocumentTypes.W9Form))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(TaxResidence, result: Failed)
                                                       }))
                                  .AddPrecondition(Resolve<ActionOpenConditionFactory>()
                                                       .Create(new UserAction(ActionType.W9Form))))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.Taxation))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.TaxResidence));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { TaxResidence });
    }

    [Theory]
    public async Task ShouldApproveApplicationAndNotFailIpMatch_WhenUserHasIpFromEurope()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                     .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                     .WithIpAddress(IpAddresses.Md))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(Resolve<SubmitTaxResidenceStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsNotPepAndPepSurveyIsNotRequested()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.PepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(false)
                                                                        .WithIsSanctioned(false)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed),
                                                       })))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsNotPepAndPepSurveyIsRequested()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.PepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminRequestSurveyStepFactory>()
                                              .Create(SurveyType.PepSurvey, AdminApi.TaskType.RiskListsScreening))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed),
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
    public async Task ShouldApproveApplication_WhenUserIsPepAndPepSurveyIsRequested()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.PepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminRequestSurveyStepFactory>()
                                              .Create(SurveyType.PepSurvey, AdminApi.TaskType.RiskListsScreening))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed),
                                                       })))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(Resolve<AdminReviewSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(true)
                                                                        .WithIsSanctioned(false)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfFunds)))
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
                         .Create(AdminApi.DocumentCategory.ProofOfFunds))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
        await VerifyUserRiskLevel(RiskRating.High);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsPepAndPepSurveyIsNotRequested()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.PepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(true)
                                                                        .WithIsSanctioned(false)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed),
                                                       }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfAddress)))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfFunds)))
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
                         .Create(AdminApi.DocumentCategory.ProofOfFunds))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
        await VerifyUserRiskLevel(RiskRating.High);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsSanctioned()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.SanctionedUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(false)
                                                                        .WithIsSanctioned(true)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed),
                                                       })))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsNotSanctioned()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.SanctionedUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(false)
                                                                        .WithIsSanctioned(false)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, PassFort, result: Failed),
                                                       })))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { RiskListsScreening });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenIdentityDocumentCheckFailedAndAdminRevertApplicationDecision()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.IdentityDocumentCheckFailedWithDecisionFraudUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Fraud)
                                                        }))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.Rejected)))
            .AddStep(Resolve<AdminUpdateUserFullNameStepFactory>()
                         .Create(new FullName
                         {
                             FirstName = RandomProvider.GenerateString(),
                             LastName = RandomProvider.GenerateString()
                         }))
            .AddStep(Resolve<AdminRevertApplicationDecisionStepFactory>()
                         .Create(AdminApi.ApplicationState.Applied))
            .AddStep(Resolve<AdminRequestDocumentStepFactory>()
                         .Create(AdminApi.TaskType.Identity, AdminApi.DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { IdentityDocument });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenIdentityDocumentCheckFailedAndUserResubmitPoi()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.IdentityDocumentCheckFailedWithDecisionResubmitUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, Pending, null),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfIdentity))))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateUserFullNameStepFactory>()
                                              .Create(new FullName
                                              {
                                                  FirstName = RandomProvider.GenerateString(),
                                                  LastName = RandomProvider.GenerateString()
                                              }))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(NameAndDoBDuplication),
                                                            UserCheck.Create(NameAndDoBDuplication),
                                                        })))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(IdentityDocument, Onfido)
                                                        })))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(Resolve<SubmitTaxResidenceStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { IdentityDocument });
    }

    [Theory]
    public async Task ShouldNotRequestPoi_WhenPoiResubmitAttemptsExceeded()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.IdentityDocumentCheckFailedWithDecisionResubmitUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, Pending, null),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfIdentity))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, Pending, null),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfIdentity))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, Pending, null),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfIdentity))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, Pending, null),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.ProofOfIdentity))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(IdentityDocument, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                        })));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationState(AdminApi.ApplicationState.Applied);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenFacialSimilarityCheckFailedAndUserResubmitSelfie()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.FacialSimilarityCheckFailedWithDecisionResubmitUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Selfie))))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateUserFullNameStepFactory>()
                                              .Create(new FullName
                                              {
                                                  FirstName = RandomProvider.GenerateString(),
                                                  LastName = RandomProvider.GenerateString()
                                              }))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(NameAndDoBDuplication),
                                                            UserCheck.Create(NameAndDoBDuplication),
                                                            UserCheck.Create(IdentityDocument, Onfido),
                                                            UserCheck.Create(IdentityDocument, Onfido),
                                                        })))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                        })))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.OnboardingSurvey))
            .AddStep(Resolve<SubmitTaxResidenceStep>());

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new[] { FacialSimilarity });
    }

    [Theory]
    public async Task ShouldNotRequestSelfie_WhenSelfieResubmitAttemptsExceeded()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(UserNames.FacialSimilarityCheckFailedWithDecisionResubmitUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey))
                         .WithIpAddress(IpAddresses.Gb))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Selfie))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Selfie))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Selfie))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .CreateWithoutPostCondition(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, Pending, null),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido, Pending, null),
                                                        }))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Selfie))))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.Selfie, DocumentTypes.Photo))
                                  .AddPostCondition(Resolve<ChecksConditionFactory>()
                                                        .Create(new[]
                                                        {
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FacialSimilarity, Onfido, result: Failed, decision: Resubmit),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                            UserCheck.Create(FaceDuplication, Onfido),
                                                        })));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationState(AdminApi.ApplicationState.Applied);
    }

    [Theory]
    public async Task ShouldFailNameAndDobDuplicationCheck()
    {
        var duplicationFullName = new FullName
        {
            FirstName = RandomProvider.GenerateString(),
            LastName = RandomProvider.GenerateString()
        };

        _flow.AddStep(Resolve<RegisterUserStep>()
                          .WithAddress(UserAddresses.Gb)
                          .WithFullName(duplicationFullName)
 );
        await _flow.Execute();
        
        StepContext.Reset();
        _flow = new BaseFlow()
                .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Gb)
                         .WithFullName(duplicationFullName)
)
                .AddStep(Resolve<SignInStep>())
                .AddStep(Resolve<CreateApplicationStep>()
                             .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey)));

        await _flow.Execute();

        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var checks = await adminApiClient.ExecuteUntilAsync(
            client => client.Checks.GetAllAsync(userId),
            checks => checks.FirstOrDefault(check => check.Type is NameAndDoBDuplication && check.Result is Failed) is not null);

        checks.Should().ContainSingle(check => check.Type == NameAndDoBDuplication &&
                                               check.State == Complete &&
                                               check.Result == Failed);

        await VerifyApplicationState(AdminApi.ApplicationState.Applied);
    }

    [Theory]
    public async Task ShouldFailIdDocNumberDuplicationCheckCheck()
    {
        var duplicationIdDocNumber = RandomProvider.GenerateIdDocNumber();

        _flow.AddStep(Resolve<RegisterUserStep>()
                          .WithAddress(UserAddresses.Gb)
 )
             .AddStep(Resolve<SignInStep>())
             .AddStep(Resolve<CreateApplicationStep>()
                          .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey)))
             .AddStep(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                          .Create(builder => builder.WithIdDocNumber(duplicationIdDocNumber, DocumentTypes.Passport)));
        await _flow.Execute();

        StepContext.Reset();
        _flow = new BaseFlow()
                .AddStep(Resolve<RegisterUserStep>()
                             .WithAddress(UserAddresses.GbWithIdDocNumber(duplicationIdDocNumber)))
                .AddStep(Resolve<SignInStep>())
                .AddStep(Resolve<CreateApplicationStep>()
                             .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.OnboardingSurvey)))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.Selfie, DocumentTypes.Photo));

        await _flow.Execute();

        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var checks = await adminApiClient.ExecuteUntilAsync(
            client => client.Checks.GetAllAsync(userId),
            checks => checks.FirstOrDefault(check => check.Type is IdDocNumberDuplication && check.Result is Failed) is not null);

        checks.Should().ContainSingle(check => check.Type == IdDocNumberDuplication &&
                                               check.State == Complete &&
                                               check.Result == Failed);
        checks.Should().ContainSingle(check => check.Type == RiskListsScreening &&
                                               check.Variant.Provider == PassFort &&
                                               check.State == Pending);

        await VerifyApplicationState(AdminApi.ApplicationState.Applied);
    }

    [TestCase(RiskRating.Low, UserAges.LowRiskFactorAge)]
    [TestCase(RiskRating.Medium, UserAges.MediumRiskFactorAge)]
    public async Task ShouldApproveApplication_WhenUserReachFirstThreshold(RiskRating riskRating, int userAge)
    {
        await ShouldApproveApplication_WhenUserHasLowOrMediumRiskRating(riskRating, userAge);

        var (firstThresholdTriggerVariantId, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Gb, ThresholdInfo.First);

        _flow = new BaseFlow()
                .AddStep(Resolve<AdminSetRiskLevelStepFactory>().Create(riskRating))
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(firstThresholdTriggerVariantId, firstThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.FinancialCondition))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.OccupationSurvey)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.InReview)))
                .AddStep(Resolve<SubmitSurveyStepFactory>().Create(SurveyType.OccupationSurvey));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }


    [TestCase(RiskRating.Low, UserAges.LowRiskFactorAge)]
    [TestCase(RiskRating.Medium, UserAges.MediumRiskFactorAge)]
    public async Task ShouldApproveApplication_WhenUserReachSecondThreshold(RiskRating riskRating, int userAge)
    {
        await ShouldApproveApplication_WhenUserReachFirstThreshold(riskRating, userAge);

        var (_, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Gb, ThresholdInfo.First);
        var (secondThresholdTriggerVariantId, secondThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Gb, ThresholdInfo.Second);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<TopUpTurnoverStepFactory>()
                                                  .Create(secondThresholdTriggerVariantId, secondThresholdTurnoverAmount - firstThresholdTurnoverAmount))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.SoFSurvey)))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.InReview)))
                .AddStep(Resolve<SubmitSurveyStepFactory>()
                             .Create(SurveyType.SoFSurvey))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfFunds));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserReachSecondThresholdAndUserRiskLevelChangedToMedium()
    {
        await ShouldApproveApplication_WhenUserReachFirstThreshold(RiskRating.Low, UserAges.LowRiskFactorAge);
        
        var (secondThresholdTriggerVariantId, _) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.Medium, Region.Gb, ThresholdInfo.Second);
        var (reapingThresholdTriggerVariantId, repeatingTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.Medium, Region.Gb, ThresholdInfo.RepeatingTurnoverThresholdStep);

        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var eventSink = Resolve<EventSink>();
        var adminApiClient = await _adminApiClientFactory.Create();
        var application = await adminApiClient.Applications.GetDefaultAsync(userId);

        var triggerCompleted = eventSink.SubscribeTo<TriggerCompletedEvent>(@event => @event.EventArgs.ApplicationId == application.Id &&
                                                                                      @event.EventArgs.UserId == userId &&
                                                                                      @event.EventArgs.VariantId == secondThresholdTriggerVariantId);
        
        _flow = new BaseFlow()
            .AddStep(Resolve<AdminSetRiskLevelStepFactory>()
                         .Create(RiskRating.Medium));
        await _flow.Execute();

        triggerCompleted.Wait(Timeouts.VerificationWaitTimeout).Should().BeTrue();
        var tasks = await adminApiClient.ExecuteUntilAsync(
            client => client.Tasks.GetAllAsync(userId),
            tasks => tasks.Any(task => task.Type is AdminApi.TaskType.ProofOfFunds && task.State is AdminApi.TaskState.Incomplete));

        tasks.Should().ContainSingle(task => task.Type == AdminApi.TaskType.ProofOfFunds &&
                                             task.State == AdminApi.TaskState.Incomplete);
        await VerifyApplicationState(AdminApi.ApplicationState.InReview);

        _flow = new BaseFlow()
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.SoFSurvey))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
            .AddStep(StepDecorator.Create(Resolve<AdminReviewDocumentStepFactory>()
                                              .Create(AdminApi.DocumentCategory.ProofOfFunds))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.Approved)))
            .AddStep(Resolve<TopUpTurnoverStepFactory>()
                         .Create(reapingThresholdTriggerVariantId, repeatingTurnoverAmount))
            .AddStep(Resolve<TopUpTurnoverStepFactory>()
                         .Create(reapingThresholdTriggerVariantId, repeatingTurnoverAmount));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserBecameHighRiskAfterFirstThreshold()
    {
        await ShouldApproveApplication_WhenUserReachFirstThreshold(RiskRating.Medium, UserAges.MediumRiskFactorAge);
        
        var (_, firstThresholdTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.Medium, Region.Gb, ThresholdInfo.First);
        var (reapingThresholdTriggerVariantId, repeatingThresholdStepAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Gb, ThresholdInfo.RepeatingTurnoverThresholdStep);
        var (_, repeatingTurnoverInitialThresholdAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.High, Region.Gb, ThresholdInfo.RepeatingTurnoverInitialThreshold);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<AdminSetRiskLevelStepFactory>()
                                                  .Create(RiskRating.High))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.SoFSurvey)))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds)))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfAddress)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.InReview)))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
                .AddStep(Resolve<SubmitSurveyStepFactory>()
                             .Create(SurveyType.SoFSurvey))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfAddress))
                .AddStep(StepDecorator.Create(Resolve<AdminReviewDocumentStepFactory>()
                                                  .Create(AdminApi.DocumentCategory.ProofOfFunds))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingTurnoverInitialThresholdAmount + 
                                                                       repeatingThresholdStepAmount - 
                                                                       firstThresholdTurnoverAmount))
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingThresholdStepAmount));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }
    
    [Theory]
    public async Task ShouldNotChangeApplicationState_WhenHighRiskUserBecameExtraHighAndReachRepeatingThreshold()
    {
        await ShouldApproveApplication_WhenUserHasHighRiskLevel();
        
        var (reapingThresholdTriggerVariantId, repeatingThresholdStepAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.ExtraHigh, Region.Gb, ThresholdInfo.RepeatingTurnoverThresholdStep);
        var (_, repeatingTurnoverInitialThresholdAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(RiskRating.ExtraHigh, Region.Gb, ThresholdInfo.RepeatingTurnoverInitialThreshold);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<AdminSetRiskLevelStepFactory>()
                                                  .Create(RiskRating.ExtraHigh))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.Approved)))
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingTurnoverInitialThresholdAmount + 
                                                                       repeatingThresholdStepAmount))
                .AddStep(Resolve<TopUpTurnoverStepFactory>()
                             .Create(reapingThresholdTriggerVariantId, repeatingThresholdStepAmount));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [TestCase(RiskRating.High)]
    [TestCase(RiskRating.ExtraHigh)]
    public async Task ShouldApproveApplication_WhenUserBecameHighOrExtraHighRiskOnMonitoring(RiskRating riskRating)
    {
        await ShouldApproveApplication_WhenUserHasLowOrMediumRiskRating(RiskRating.Low, UserAges.LowRiskFactorAge);

        _flow = new BaseFlow()
                .AddStep(StepDecorator.Create(Resolve<AdminSetRiskLevelStepFactory>()
                                                  .Create(riskRating))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.FinancialCondition))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.OccupationSurvey)))
                                      .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                            .Create(AdminApi.TaskType.ProofOfFunds))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.SoFSurvey)))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfFunds)))
                                      .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                            .Create(new UserAction(ActionType.ProofOfAddress)))
                                      .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                            .Create(AdminApi.ApplicationState.InReview)))
                .AddStep(Resolve<SubmitSurveyStepFactory>()
                             .Create(SurveyType.OccupationSurvey))
                .AddStep(Resolve<SubmitSurveyStepFactory>()
                             .Create(SurveyType.SoFSurvey))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfFunds, DocumentTypes.CompanyBankStatement))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                             .Create(DocumentCategory.ProofOfAddress, DocumentTypes.BankStatement))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfFunds))
                .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                             .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [TestCase(RiskRating.Low, UserAges.LowRiskFactorAge)]
    [TestCase(RiskRating.Medium, UserAges.MediumRiskFactorAge)]
    public async Task ShouldNotChangeApplicationState_WhenRepeatingThresholdReachedSeveralTimes(RiskRating riskRating, int userAge)
    {
        await ShouldApproveApplication_WhenUserReachSecondThreshold(riskRating, userAge);

        var (reapingThresholdTriggerVariantId, repeatingTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Gb, ThresholdInfo.RepeatingTurnoverThresholdStep);
        _flow = new BaseFlow()
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
    public async Task ShouldNotChangeApplicationState_WhenRepeatingThresholdReachedSeveralTimesAndUserIsHighRisk(RiskRating riskRating)
    {
        await ShouldApproveApplication_WhenUserBecameHighOrExtraHighRiskOnMonitoring(riskRating);

        var (reapingThresholdTriggerVariantId, repeatingTurnoverAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Gb, ThresholdInfo.RepeatingTurnoverThresholdStep);
        var (_, repeatingThresholdInitialThresholdAmount) =
            Resolve<TurnoverProvider>().GetTurnoverInfo(riskRating, Region.Gb, ThresholdInfo.RepeatingTurnoverInitialThreshold);

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
}
