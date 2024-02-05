namespace WX.B2C.User.Verification.Api.Admin.Client
{
    using System;
    using Microsoft.Rest;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using WX.B2C.User.Verification.Api.Admin.Client.Models;

    /// <summary>
    /// Documents operations.
    /// </summary>
    public partial interface IDocuments
    {
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="ErrorResponseException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<UploadedFileDto>> UploadWithHttpMessagesAsync(Guid userId, DocumentCategory documentCategory, string documentType, FileToUpload file, bool uploadToProvider = false, ExternalProviderType? provider = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}
