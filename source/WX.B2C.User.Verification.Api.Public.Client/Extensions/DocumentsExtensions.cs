namespace WX.B2C.User.Verification.Api.Public.Client
{
    using System.Threading;
    using System.Threading.Tasks;
    using WX.B2C.User.Verification.Api.Public.Client.Models;

    /// <summary>
    /// Extension methods for Documents.
    /// </summary>
    public static partial class DocumentsExtensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<UploadedFileDto> UploadAsync(this IDocuments operations, DocumentCategory documentCategory, string documentType, FileToUpload file, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UploadWithHttpMessagesAsync(documentCategory, documentType, file, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
