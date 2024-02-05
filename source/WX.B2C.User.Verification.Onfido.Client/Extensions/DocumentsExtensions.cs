using System;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido.Client
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Documents.
    /// </summary>
    public static partial class DocumentsExtensions
    {
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='applicantId'>
        /// The unique identifier for the applicant.
        /// </param>
        /// <param name='type'>
        /// The type of document. For example, passport.
        /// </param>
        /// <param name='file'>
        /// File to upload.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<Document> UploadAsync(this IDocuments operations, string applicantId, string type, FileToUpload file, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.UploadWithHttpMessagesAsync(applicantId, type, file, null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }

        /// <summary>
        /// Download a documents raw data
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='documentId'>
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<byte[]> DownloadAsync(this IDocuments operations, string documentId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.DownloadWithHttpMessagesAsync(documentId, null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
    }
}
