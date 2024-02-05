using FluentAssertions;
using Newtonsoft.Json;
using WX.B2C.Risks.Api.Admin.Client.Models;
using WX.B2C.Survey.Api.Public.Client;
using WX.B2C.Survey.Api.Public.Client.Models;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;
using ISurveyPublicClient = WX.B2C.Survey.Api.Public.Client.IB2CSurveyApiClient;
using static WX.B2C.User.Verification.Integration.Tests.Constants.Surveys;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;

internal class SubmitSurveyStep : BaseApplicationActionStep
{
    private readonly ISurveyPublicClient _surveyApiClient;
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly SurveyType _surveyType;
    private readonly RiskRating _desiredRiskFactor;

    private Guid _collectionStepId;

    public SubmitSurveyStep(IPublicClient publicClient,
                            ISurveyPublicClient surveyApiClient,
                            VerificationAdminApiClientFactory adminApiClientFactory,
                            SurveyType surveyType,
                            RiskRating desiredRiskFactor)
        : base(publicClient, new UserAction(ActionType.Survey, surveyType: surveyType))
    {
        _surveyApiClient = surveyApiClient;
        _adminApiClientFactory = adminApiClientFactory;
        _surveyType = surveyType;
        _desiredRiskFactor = desiredRiskFactor;
    }

    public override async Task PreCondition()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var steps = await adminApiClient.CollectionStep.GetAllAsync(userId);
        var surveyStep = steps.FirstOrDefault(step => step.State is CollectionStepState.Requested && 
                                                        step.Variant is SurveyCollectionStepVariantDto variant &&
                                                        variant.TemplateId == SurveyHelper.GetSurveyId(_surveyType));

        surveyStep.Should().NotBeNull();
        _collectionStepId = surveyStep!.Id;
    }

    public override async Task Execute()
    {
        var templateId = SurveyHelper.GetSurveyId(_surveyType);
        var questionsFilePath = GetFilePath();
        var submitAnswersRequest = await GetSurveyQuestions(questionsFilePath);

        await _surveyApiClient.UserSurveys.SaveAnswersAsync(templateId, submitAnswersRequest);
    }

    public override async Task PostCondition()
    {
        await base.PostCondition();

        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var step = await adminApiClient.ExecuteUntilAsync(
            client => client.CollectionStep.GetAsync(userId, _collectionStepId),
            step => step.State is not CollectionStepState.Requested);

        var expectedState = step.IsReviewNeeded ? CollectionStepState.InReview : CollectionStepState.Completed;
        step.State.Should().Be(expectedState);
    }

    private string GetFilePath()
    {
        var baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), FolderName);
        var filename = (_surveyType, _desiredRiskFactor) switch
        {
            (SurveyType.UsaCdd, RiskRating.Low)              => UsaCddLowRiskRating,
            (SurveyType.UsaCdd, RiskRating.Medium)           => UsaCddMediumRiskRating,
            (SurveyType.UsaCdd, RiskRating.High)             => UsaCddMediumRiskRating,
            (SurveyType.OnboardingSurvey, RiskRating.Low)    => OnboardingSurveyLowRiskRating,
            (SurveyType.OnboardingSurvey, RiskRating.Medium) => OnboardingSurveyMediumRiskRating,
            (SurveyType.OnboardingSurvey, RiskRating.High)   => OnboardingSurveyHighRiskRating,
            (SurveyType.OccupationSurvey, RiskRating.Low or
                                          RiskRating.Medium) => OccupationSurveyLowRiskRating,
            (SurveyType.OccupationSurvey, RiskRating.High)   => OccupationSurveyHighRiskRating,
            (SurveyType.UsaEdd, _)                           => UsaEdd,
            (SurveyType.PepSurvey, _)                        => PepSurvey,
            (SurveyType.SoFSurvey, _)                        => SofSurvey,
            _                                                => throw new NotImplementedException($"{_surveyType} {_desiredRiskFactor} survey for them is not implemented")
        };
        return Path.Combine(baseDirectory, filename);
    }

    private static async Task<IList<NewUserAnswerDto>> GetSurveyQuestions(string filepath)
    {
        using var sr = new StreamReader(filepath);
        var answersString = await sr.ReadToEndAsync();
        return JsonConvert.DeserializeObject<NewUserAnswerDto[]>(answersString)!;
    }
}

internal class SubmitSurveyStepFactory
{
    private readonly ISurveyPublicClient _surveyApiClient;
    private readonly IPublicClient _publicClient;
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public SubmitSurveyStepFactory(IPublicClient publicClient,
                                   ISurveyPublicClient surveyApiClient,
                                   VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _publicClient = publicClient;
        _surveyApiClient = surveyApiClient;
        _adminApiClientFactory = adminApiClientFactory;
    }

    public SubmitSurveyStep Create(SurveyType survey, RiskRating riskRating = RiskRating.Low) =>
        new(_publicClient, _surveyApiClient, _adminApiClientFactory, survey, riskRating);
}
