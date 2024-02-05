using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Api.Admin.Client
{

    /// <summary>
    /// Extension methods for Files.
    /// </summary>
    public static partial class FilesExtensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='userId'>
            /// </param>
            /// <param name='fileId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Stream> DownloadAsync(this IFiles operations, System.Guid userId, System.Guid fileId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.DownloadWithHttpMessagesAsync(userId, fileId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}