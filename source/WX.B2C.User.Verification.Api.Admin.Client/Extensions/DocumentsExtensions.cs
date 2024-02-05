namespace WX.B2C.User.Verification.Api.Admin.Client
{
    using System.Threading;
    using System.Threading.Tasks;
    using WX.B2C.User.Verification.Api.Admin.Client.Models;

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
            public static async Task<UploadedFileDto> UploadAsync(this IDocuments operations, System.Guid userId, DocumentCategory documentCategory, string documentType, FileToUpload file, bool uploadToProvider = false, ExternalProviderType? provider = null, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UploadWithHttpMessagesAsync(userId, documentCategory, documentType, file, uploadToProvider, provider, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
