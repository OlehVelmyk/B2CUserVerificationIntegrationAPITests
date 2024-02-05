using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminReviewDocumentStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly DocumentCategory _documentCategory;
    private readonly CollectionStepReviewResult _reviewResult;

    public AdminReviewDocumentStep(VerificationAdminApiClientFactory adminApiClientFactory,
                                   DocumentCategory documentCategory,
                                   CollectionStepReviewResult reviewResult)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _documentCategory = documentCategory;
        _reviewResult = reviewResult;
    }
    
    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var collectionsSteps = await adminApiClient.CollectionStep.GetAllAsync(userId);
        var documentStep = collectionsSteps.First(IsDocumentReviewNeeded);
        var request = new ReviewCollectionStepRequest
        {
            Reason = ReasonProvider.Create(callerMethod: nameof(AdminReviewDocumentStep)),
            ReviewResult = _reviewResult
        };

        await adminApiClient.CollectionStep.ReviewAsync(request, userId, documentStep.Id);

        bool IsDocumentReviewNeeded(CollectionStepDto step) =>
            step.Variant is DocumentCollectionStepVariantDto variant &&
            variant.DocumentCategory == _documentCategory &&
            step.IsReviewNeeded && 
            step.State is CollectionStepState.InReview;
    }
}

internal class AdminReviewDocumentStepFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public AdminReviewDocumentStepFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public AdminReviewDocumentStep Create(
        DocumentCategory category, 
        CollectionStepReviewResult reviewResult = CollectionStepReviewResult.Approved) =>
            new(_adminApiClientFactory, category, reviewResult);
}
