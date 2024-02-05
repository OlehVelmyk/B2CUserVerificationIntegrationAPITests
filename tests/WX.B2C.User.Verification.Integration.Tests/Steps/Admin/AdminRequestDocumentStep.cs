using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Providers;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.Admin;

internal class AdminRequestDocumentStep : BaseStep
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly TaskType _targetTaskType;
    private readonly DocumentCategory _documentCategory;
    private readonly string _documentType;
    private readonly bool _isRequired;
    private readonly bool _isReviewNeeded;
    
    public AdminRequestDocumentStep(VerificationAdminApiClientFactory adminApiClientFactory,
                                    TaskType targetTaskType, 
                                    DocumentCategory documentCategory,
                                    string documentType,  
                                    bool isRequired = true,
                                    bool isReviewNeeded = false)
    {
        _adminApiClientFactory = adminApiClientFactory;
        _targetTaskType = targetTaskType;
        _documentCategory = documentCategory;
        _documentType = documentType;
        _isRequired = isRequired;
        _isReviewNeeded = isReviewNeeded;
    }

    public override async Task Execute()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();
        
        var tasks = await adminApiClient.Tasks.GetAllAsync(userId);
        var targetTask = tasks.First(task => task.Type == _targetTaskType);
        await adminApiClient.CollectionStep.RequestAsync(new DocumentCollectionStepRequest
        {
            Type = CollectionStepType.Document,
            Reason = ReasonProvider.Create(callerMethod: nameof(AdminRequestDocumentStep)),
            DocumentCategory = _documentCategory,
            DocumentType = _documentType,
            IsRequired = _isRequired,
            IsReviewNeeded = _isReviewNeeded,
            TargetTasks = new List<Guid> { targetTask.Id }
        }, userId);
    }
}

internal class AdminRequestDocumentStepFactory
{
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public AdminRequestDocumentStepFactory(VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _adminApiClientFactory = adminApiClientFactory;
    }

    public AdminRequestDocumentStep Create(TaskType targetTaskType,
                                           DocumentCategory documentCategory,
                                           string documentType,
                                           bool isRequired = true,
                                           bool isReviewNeeded = false) =>
        new (_adminApiClientFactory, targetTaskType, documentCategory, documentType, isRequired, isReviewNeeded);
}
