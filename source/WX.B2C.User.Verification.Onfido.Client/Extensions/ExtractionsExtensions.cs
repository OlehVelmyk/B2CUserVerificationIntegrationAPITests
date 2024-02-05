using System;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido.Client
{
    /// <summary>
    /// Extension methods for Extractions.
    /// </summary>
    public static partial class ExtractionsExtensions
    {
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='body'>
        /// The request with document id.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<Extraction> FindAsync(this IExtractions operations, ExtractionRequest body, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.FindWithHttpMessagesAsync(body, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
    }
}
