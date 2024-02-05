using Newtonsoft.Json;
using RestEase;
using WX.B2C.User.Verification.Integration.Tests.Constants;

namespace WX.B2C.User.Verification.Integration.Tests.Api;

public interface IOnfidoApi
{
    [Header("Authorization", General.OnfidoToken)]
    [Header("Content-Disposition", "form-data")]
    [Post("v3.2/documents")]
    Task<OnfidoUploadDocumentResponse> UploadDocument([Body]MultipartFormDataContent content);
    
    [Header("Authorization", General.OnfidoToken)]
    [Header("Content-Disposition", "form-data")]
    [Post("v3.2/live_photos")]
    Task<OnfidoUploadDocumentResponse> UploadLivePhoto([Body]MultipartFormDataContent content);
    
    [Header("Authorization", General.OnfidoToken)]
    [Header("Content-Disposition", "form-data")]
    [Post("v3.2/live_videos")]
    Task<OnfidoUploadDocumentResponse> UploadLiveVideo([Body]MultipartFormDataContent content);
}

/// <summary>
/// type inside MultipartFormDataContent
/// </summary>
public class OnfidoUploadDocumentRequest
{
    [JsonProperty("applicant_id")]
    public string ApplicantId { get; set; }
    
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("file")]
    public string File { get; set; }
}

/// <summary>
/// type inside MultipartFormDataContent
/// </summary>
public class OnfidoUploadLivePhotoRequest
{
    [JsonProperty("applicant_id")]
    public string ApplicantId { get; set; }
    
    [JsonProperty("file")]
    public string File { get; set; }
}

public class OnfidoUploadDocumentResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
}
