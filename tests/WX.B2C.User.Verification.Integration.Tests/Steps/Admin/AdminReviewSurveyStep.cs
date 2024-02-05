using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Providers;
using IAdminClient = WX.B2C.User.Verification.Api.Admin.Client.IUserVerificationApiClient;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminReviewSurveyStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly SurveyType _surveyType;
    private readonly CollectionStepReviewResult _reviewResult;

    public AdminReviewSurveyStep(VerificationAdminApiClientFactory adminApiClientFactory,
                                 SurveyType surveyType, 
                                 CollectionStepReviewResult reviewResult)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _surveyType = surveyType;
        _reviewResult = reviewResult;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var templateId = SurveyHelper.GetSurveyId(_surveyType);
        var adminApiClient = await _adminApiClientFactory.Create();

        var collectionsSteps = await adminApiClient.CollectionStep.GetAllAsync(userId);
        var surveyStep = collectionsSteps.First(IsSurveyReviewNeeded);
        var request = new ReviewCollectionStepRequest
        {
            Reason = ReasonProvider.Create(callerMethod: nameof(AdminReviewSurveyStep)),
            ReviewResult = _reviewResult
        };

        await adminApiClient.CollectionStep.ReviewAsync(request, userId, surveyStep.Id);

        bool IsSurveyReviewNeeded(CollectionStepDto step) =>
            step.Variant is SurveyCollectionStepVariantDto variant &&
            variant.TemplateId == templateId &&
            step.IsReviewNeeded && 
            step.State is CollectionStepState.InReview;
    }
}

internal class AdminReviewSurveyStepFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public AdminReviewSurveyStepFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public AdminReviewSurveyStep Create(
        SurveyType surveyType,
        CollectionStepReviewResult reviewResult = CollectionStepReviewResult.Approved) =>
            new(_adminApiClientFactory, surveyType, reviewResult);
}
