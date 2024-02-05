using System.Net.Http.Headers;
using FluentAssertions;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Integration.Tests.Api;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Extensions;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Models;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

using static WX.B2C.User.Verification.Integration.Tests.Constants.Content;
using static WX.B2C.User.Verification.Integration.Tests.Constants.MediaTypes;

namespace WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;

internal class SubmitDocumentStep : BaseApplicationActionStep
{
    private readonly IOnfidoApi _onfidoApi;
    private readonly IPublicClient _publicClient;
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;
    private readonly DocumentCategory _documentCategory;
    private readonly string _documentType;

    private Guid _collectionStepId;

    public SubmitDocumentStep(IPublicClient publicClient,
                              IOnfidoApi onfidoApi,
                              VerificationAdminApiClientFactory adminApiClientFactory,
                              DocumentCategory documentCategory,
                              string documentType)
        : base(publicClient, new UserAction(ActionType.Selfie))
    {
        _publicClient = publicClient;
        _onfidoApi = onfidoApi;
        _adminApiClientFactory = adminApiClientFactory;
        _documentCategory = documentCategory;
        _documentType = documentType;

        SetUpUserAction();
    }

    public override async Task PreCondition()
    {
        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var steps = await adminApiClient.CollectionStep.GetAllAsync(userId);
        var documentStep = steps.FirstOrDefault(step => step.State is AdminApi.CollectionStepState.Requested && 
                                                        step.Variant is AdminApi.DocumentCollectionStepVariantDto variant &&
                                                        variant.DocumentCategory == (AdminApi.DocumentCategory) _documentCategory);

        documentStep.Should().NotBeNull();
        _collectionStepId = documentStep!.Id;
    }

    public override async Task Execute()
    {
        string uploadedFileId;
        ExternalFileProviderType? externalFileProviderType = null;
        if (_documentCategory is DocumentCategory.Selfie or DocumentCategory.ProofOfIdentity)
        {
            uploadedFileId = await UploadFileToOnfido();
            externalFileProviderType = ExternalFileProviderType.Onfido;
        }
        else
            uploadedFileId = await UploadFileToPublicApi();

        var request = new SubmitDocumentRequest
        {
            Category = _documentCategory,
            Type = _documentType,
            Files = new List<string> { uploadedFileId },
            Provider = externalFileProviderType
        };

        await _publicClient.Documents.SubmitAsync(request);
    }

    public override async Task PostCondition()
    {
        await base.PostCondition();

        var userId = Guid.Parse(StepContext.Instance[General.UserId]);
        var adminApiClient = await _adminApiClientFactory.Create();

        var step = await adminApiClient.ExecuteUntilAsync(
            client => client.CollectionStep.GetAsync(userId, _collectionStepId),
            step => step.State is not AdminApi.CollectionStepState.Requested);

        var expectedState = step.IsReviewNeeded ? AdminApi.CollectionStepState.InReview : AdminApi.CollectionStepState.Completed;
        step.State.Should().Be(expectedState);
    }

    private void SetUpUserAction()
    {
        UserAction = _documentCategory switch
        {
            DocumentCategory.ProofOfIdentity => new UserAction(ActionType.ProofOfIdentity),
            DocumentCategory.ProofOfAddress  => new UserAction(ActionType.ProofOfAddress),
            DocumentCategory.Taxation        => new UserAction(ActionType.W9Form),
            DocumentCategory.ProofOfFunds    => new UserAction(ActionType.ProofOfFunds),
            DocumentCategory.Selfie          => new UserAction(ActionType.Selfie),
            _                                => throw new NotImplementedException($"{nameof(SubmitDocumentStep)} don't support {_documentCategory}")
        };
    }


    private async Task<string> UploadFileToOnfido()
    {
        var applicantId = await GetApplicantId();
        var (filePath, mediaType) = GetFileInfo();
        
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(applicantId), "applicant_id");
        var imageData = await File.ReadAllBytesAsync(filePath);
        var imageContent = new ByteArrayContent(imageData);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mediaType);
        content.Add(imageContent, "file", _documentType);
        if (_documentType == DocumentTypes.Video)
            content.Add(new StringContent("[{\"type\": \"recite\",\"query\": [1,2,3]},{\"type\": \"movement\",\"query\": \"turnRight\"}]"), "challenge");

        var response = _documentType switch
        {
            DocumentTypes.Photo => await _onfidoApi.UploadLivePhoto(content),
            DocumentTypes.Video => await _onfidoApi.UploadLiveVideo(content),
            _                   => await _onfidoApi.UploadDocument(content)
        };

        return response.Id;
    }

    private async Task<string> UploadFileToPublicApi()
    {
        var (filePath, mediaType) = GetFileInfo();
        var imageData = await File.ReadAllBytesAsync(filePath);
        using var imageStream = new MemoryStream(imageData);

        var fileToUpload = new FileToUpload(imageStream, Path.GetFileName(filePath), mediaType);
        var response = await _publicClient.Documents.UploadAsync(_documentCategory, _documentType, fileToUpload);
        return response.FileId.ToString();
    }

    private (string filePath, string mediaType) GetFileInfo()
    {
        var baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), FolderName);

        var (filename, mediaType) = (_documentCategory, _documentType) switch
        {
            (DocumentCategory.Selfie, DocumentTypes.Photo) => (SelfiePhoto, ImagePng),
            (DocumentCategory.Selfie, DocumentTypes.Video) => (SelfieVideo, VideoMp4),
            (DocumentCategory.ProofOfIdentity, _)          => (Passport, ImagePng),
            (DocumentCategory.ProofOfAddress, _)           => (ProofOfAddress, ImagePng),
            (DocumentCategory.ProofOfFunds, _)             => (ProofOfFunds, ImagePng),
            (DocumentCategory.Taxation, _)                 => (ProofOfFunds, ImagePng),
            _                                              => throw new ArgumentOutOfRangeException()
        };
        
        return (Path.Combine(baseDirectory, filename), mediaType);
    }

    private async Task<string> GetApplicantId()
    {
        var request = new SdkTokenRequest
        {
            Type = TokenType.Web
        };
        
        var response = await _publicClient.Providers.PostAsync(request);
        return response.ApplicantId;
    }
}

internal class SubmitDocumentWithoutPostConditionStep : SubmitDocumentStep
{
    public SubmitDocumentWithoutPostConditionStep(IPublicClient publicClient,
                                                  IOnfidoApi onfidoApi,
                                                  VerificationAdminApiClientFactory adminApiClientFactory,
                                                  DocumentCategory documentCategory,
                                                  string documentType) : 
        base(publicClient, onfidoApi, adminApiClientFactory, documentCategory, documentType)
    {
    }

    public override Task PostCondition() =>
        Task.CompletedTask;
}

internal class SubmitDocumentStepFactory
{
    private readonly IPublicClient _publicClient;
    private readonly IOnfidoApi _onfidoApi;
    private readonly VerificationAdminApiClientFactory _adminApiClientFactory;

    public SubmitDocumentStepFactory(IPublicClient publicClient,
                                     IOnfidoApi onfidoApi,
                                     VerificationAdminApiClientFactory adminApiClientFactory)
    {
        _publicClient = publicClient;
        _onfidoApi = onfidoApi;
        _adminApiClientFactory = adminApiClientFactory;
    }

    public SubmitDocumentStep Create(DocumentCategory documentCategory, string documentType) => 
        new(_publicClient, _onfidoApi, _adminApiClientFactory, documentCategory, documentType);
    
    public SubmitDocumentWithoutPostConditionStep CreateWithoutPostCondition(DocumentCategory documentCategory, string documentType) => 
        new(_publicClient, _onfidoApi, _adminApiClientFactory, documentCategory, documentType);
}
