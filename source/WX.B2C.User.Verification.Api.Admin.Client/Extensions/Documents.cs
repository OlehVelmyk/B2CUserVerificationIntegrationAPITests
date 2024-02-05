namespace WX.B2C.User.Verification.Api.Admin.Client
{
    using System;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using WX.B2C.User.Verification.Api.Admin.Client.Models;

    /// <summary>
    /// Documents operations.
    /// </summary>
    public partial class Documents
    {
        public async Task<HttpOperationResponse<UploadedFileDto>> UploadWithHttpMessagesAsync(Guid userId, DocumentCategory documentCategory, string documentType, FileToUpload file, bool uploadToProvider = false, ExternalProviderType? provider = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (file == null)
                throw new ValidationException(ValidationRules.CannotBeNull, nameof(file));

            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add(nameof(userId), userId);
                tracingParameters.Add(nameof(documentCategory), documentCategory);
                tracingParameters.Add(nameof(documentType), documentType);
                tracingParameters.Add(nameof(uploadToProvider), uploadToProvider);
                tracingParameters.Add(nameof(provider), provider);
                tracingParameters.Add(nameof(file), file);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "Upload", tracingParameters);
            }
            // Construct URL
            var _baseUrl = Client.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), $"api/v1/verification/{userId}/documents/files").ToString();
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new System.Uri(_url);
            // Set Headers
            if (_httpRequest.Headers.Contains("correlationId"))
            {
                _httpRequest.Headers.Remove("correlationId");
            }
            _httpRequest.Headers.TryAddWithoutValidation("correlationId", Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(Client.CorrelationId, Client.SerializationSettings).Trim('"'));
            if (Client.OperationId != null)
            {
                if (_httpRequest.Headers.Contains("operationId"))
                {
                    _httpRequest.Headers.Remove("operationId");
                }
                _httpRequest.Headers.TryAddWithoutValidation("operationId", Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(Client.OperationId, Client.SerializationSettings).Trim('"'));
            }


            if (customHeaders != null)
            {
                foreach(var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Prepare Request content
            var content = new MultipartFormDataContent
            {
                { new StringContent(documentCategory.ToString()), "documentCategory" },
                { new StringContent(documentType), "documentType" },
                { new StringContent(uploadToProvider.ToString()), "uploadToProvider" },
            };

            if (provider.HasValue)
                content.Add(new StringContent(provider.ToString()), "provider");

            var fileContent = new StreamContent(file.Data);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = file.FileName ?? "file"
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent);
            _httpRequest.Content = content;

            var _requestContent = await _httpRequest.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Set Credentials
            if (Client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            }
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200)
            {
                var ex = new ErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                try
                {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    ErrorResponse _errorBody =  Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<ErrorResponse>(_responseContent, Client.DeserializationSettings);
                    if (_errorBody != null)
                    {
                        ex.Body = _errorBody;
                    }
                }
                catch (JsonException)
                {
                    // Ignore the exception
                }
                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<UploadedFileDto>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<UploadedFileDto>(_responseContent, Client.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }
    }
}