using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using WX.B2C.Risks.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Models;
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

internal class UsaTests : BaseTest
{
    private IFlow _flow;

    [SetUp]
    public void SetUp()
    {
        StepContext.Reset();
        _flow = new BaseFlow();
    }

    [TestCase(RiskRating.Low, UserAges.LowRiskFactorAge)]
    [TestCase(RiskRating.Medium, UserAges.MediumRiskFactorAge)]
    [TestCase(RiskRating.High, UserAges.HighRiskFactorAge)]
    public async Task ShouldApproveApplication(RiskRating riskRating, int userAge)
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Us)
                         .WithFullName(UserNames.UsaSuccessfulFlowUserName)
                         .WithAge(userAge))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                         .AddAction(new UserAction(ActionType.Tin))
                         .WithIpAddress(IpAddresses.Us))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTinStep>())
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd, riskRating));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
        await VerifyUserRiskLevel(riskRating);
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenAdminReviewPoaDocument()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Us)
                         .WithFullName(UserNames.UsaSuccessfulFlowUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                         .AddAction(new UserAction(ActionType.Tin))
                         .WithIpAddress(IpAddresses.Us))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTinStep>())
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd))
            .AddStep(StepDecorator.Create(Resolve<AdminRequestDocumentStepFactory>()
                                              .Create(AdminApi.TaskType.Address, 
                                                      AdminApi.DocumentCategory.ProofOfAddress,
                                                      DocumentTypes.CertificateOfResidency,
                                                      true, true))
                                  .AddPrecondition(Resolve<ApplicationStateConditionFactory>()
                                                       .Create(AdminApi.ApplicationState.Approved))
                                  .AddPostCondition(Resolve<ApplicationStateConditionFactory>()
                                                        .Create(AdminApi.ApplicationState.InReview)))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfAddress, DocumentTypes.CertificateOfResidency))
            .AddStep(Resolve<AdminReviewDocumentStepFactory>()
                         .Create(AdminApi.DocumentCategory.ProofOfAddress));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenIpMatchCheckFailed()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.Us)
                         .WithFullName(UserNames.UsaSuccessfulFlowUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                         .AddAction(new UserAction(ActionType.Tin))
                         .WithIpAddress(IpAddresses.Gb))
                .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTinStep>())
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd))
            .AddStep(StepDecorator.Create(Resolve<SubmitDocumentStepFactory>()
                                              .Create(DocumentCategory.ProofOfAddress, DocumentTypes.CertificateOfResidency))
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
        await VerifyApplicationApproved(new [] { IpMatch });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsNotResident()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.UsNotResident)
                         .WithFullName(UserNames.UsaNotResidentUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                         .AddAction(new UserAction(ActionType.Tin))
                         .WithIpAddress(IpAddresses.Us))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(StepDecorator.Create(Resolve<SubmitTinStep>()
                                              .With(TinType.ITIN, "911501111"))
                                  .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                        .Create(AdminApi.TaskType.RiskListsScreening))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaEdd))))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaEdd))
            .AddStep(Resolve<AdminReviewSurveyStepFactory>()
                         .Create(SurveyType.UsaEdd));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved();
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsPep()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.UsPepUserAddress)
                         .WithFullName(UserNames.UsaPepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                         .AddAction(new UserAction(ActionType.Tin))
                         .WithIpAddress(IpAddresses.Us))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTinStep>()
                         .With(TinType.SSN, "530201236"))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd))
            .AddStep(StepDecorator.Create(Resolve<AdminRequestSurveyStepFactory>()
                                              .Create(SurveyType.PepSurvey, AdminApi.TaskType.RiskListsScreening))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, LexisNexis, result: Failed)
                                                       })))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(Resolve<AdminReviewSurveyStepFactory>()
                         .Create(SurveyType.PepSurvey))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(true)
                                                                        .WithIsSanctioned(false)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPostCondition(Resolve<TaskAssignedConditionFactory>()
                                                        .Create(AdminApi.TaskType.RiskListsScreening))
                                  .AddPostCondition(Resolve<ActionOpenConditionFactory>()
                                                        .Create(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaEdd))))
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaEdd))
            .AddStep(Resolve<AdminReviewSurveyStepFactory>()
                         .Create(SurveyType.UsaEdd))
            .AddStep(Resolve<AdminApproveTaskStepFactory>()
                         .Create(AdminApi.TaskType.RiskListsScreening));

        await _flow.Execute();

        using var _ = new AssertionScope();
        await VerifyApplicationApproved(new [] { RiskListsScreening });
    }

    [Theory]
    public async Task ShouldApproveApplication_WhenUserIsNotPepAndPepSurveyIsNotRequested()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                         .WithAddress(UserAddresses.UsPepUserAddress)
                         .WithFullName(UserNames.UsaPepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                         .AddAction(new UserAction(ActionType.Tin))
                         .WithIpAddress(IpAddresses.Us))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTinStep>()
                         .With(TinType.SSN, "530201236"))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd))
            .AddStep(StepDecorator.Create(Resolve<AdminUpdateVerificationDetailsStepFactory>()
                                              .Create(builder => builder.WithIsPep(false)
                                                                        .WithIsSanctioned(false)
                                                                        .WithIsAdverseMedia(false)))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, LexisNexis, result: Failed)
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
                         .WithAddress(UserAddresses.UsPepUserAddress)
                         .WithFullName(UserNames.UsaPepUserName))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                         .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                         .AddAction(new UserAction(ActionType.Tin))
                         .WithIpAddress(IpAddresses.Us))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTinStep>()
                         .With(TinType.SSN, "530201236"))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd))
            .AddStep(StepDecorator.Create(Resolve<AdminRequestSurveyStepFactory>()
                                              .Create(SurveyType.PepSurvey, AdminApi.TaskType.RiskListsScreening))
                                  .AddPrecondition(Resolve<ChecksConditionFactory>()
                                                       .Create(new[]
                                                       {
                                                           UserCheck.Create(RiskListsScreening, LexisNexis, result: Failed)
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
    public async Task ShouldRejectApplication_WhenInstantIdCheckFailed()
    {
        _flow
            .AddStep(Resolve<RegisterUserStep>()
                     .WithAddress(new Address
                     {
                         Country = "US",
                         City = "Spring Creek",
                         State = "NV",
                         Line1 = "639 Rock Island Dr",
                         ZipCode = "89815",
                     })
                     .WithFullName(new FullName
                     {
                         FirstName = "Dolly",
                         LastName = "O'Hara"
                     })
                     .WithUserSection(Resolve<Settings>().User))
            .AddStep(Resolve<SignInStep>())
            .AddStep(Resolve<CreateApplicationStep>()
                     .AddAction(new UserAction(ActionType.Survey, surveyType: SurveyType.UsaCdd))
                     .AddAction(new UserAction(ActionType.Tin))
                     .WithCustomHeaders(new Dictionary<string, List<string>>
                     {
                         { Headers.TestIpAddress, new List<string> { IpAddresses.Us } }
                     }))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.ProofOfIdentity, DocumentTypes.Passport))
            .AddStep(Resolve<SubmitDocumentStepFactory>()
                         .Create(DocumentCategory.Selfie, DocumentTypes.Video))
            .AddStep(Resolve<SubmitTinStep>()
                         .With(TinType.SSN, "546908734"))
            .AddStep(Resolve<SubmitTaxResidenceStep>())
            .AddStep(Resolve<SubmitSurveyStepFactory>()
                         .Create(SurveyType.UsaCdd));

        await _flow.Execute();

        const string Decision = "InstantIdClosing";
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await Resolve<VerificationAdminApiClientFactory>().Create();

        var application = await adminApiClient.ExecuteUntilAsync(
            client => client.Applications.GetDefaultAsync(userId),
            application => application.State is AdminApi.ApplicationState.Rejected);
        
        using var _ = new AssertionScope();
        application.State.Should().Be(AdminApi.ApplicationState.Rejected);
        application.DecisionReasons.Should().HaveCount(1);
        application.DecisionReasons.Should().HaveElementAt(0, Decision);
        
        var checks = await adminApiClient.Checks.GetAllAsync(userId);
        var fraudScreeningCheck = checks.First(check => check.Type == AdminApi.CheckType.FraudScreening);
        fraudScreeningCheck.State.Should().Be(AdminApi.CheckState.Complete);
        fraudScreeningCheck.Result.Should().Be(AdminApi.CheckResult.Failed);
        fraudScreeningCheck.Decision.Should().Be(Decision);
    }
}
