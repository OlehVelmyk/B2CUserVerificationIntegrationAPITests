using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Api.Admin.Client
{

    /// <summary>
    /// Files operations.
    /// </summary>
    public partial interface IFiles
    {
        /// <param name='userId'>
        /// </param>
        /// <param name='fileId'>
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
        Task<HttpOperationResponse<Stream>> DownloadWithHttpMessagesAsync(System.Guid userId, System.Guid fileId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}