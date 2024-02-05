using Microsoft.Rest;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido.Client
{
    public interface IExtractions
    {
        /// <summary>
        /// Create a check
        /// </summary>
        /// <param name='body'>
        /// body with documentId that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="OnfidoApiErrorException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        Task<HttpOperationResponse<Extraction>> FindWithHttpMessagesAsync(ExtractionRequest body, CancellationToken cancellationToken = default(CancellationToken));
    }
}
