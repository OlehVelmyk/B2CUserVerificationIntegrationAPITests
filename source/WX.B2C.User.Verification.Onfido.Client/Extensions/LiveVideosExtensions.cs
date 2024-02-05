using System.Threading;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Onfido.Client
{
    public static partial class LiveVideosExtensions
    {
        /// <summary>
        /// Download live video
        /// </summary>
        /// <remarks>
        /// Live videos are downloaded using this endpoint.
        /// </remarks>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='liveVideoId'>
        /// The live video’s unique identifier.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<byte[]> DownloadAsync(this ILiveVideos operations, string liveVideoId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.DownloadWithHttpMessagesAsync(liveVideoId, null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
    }
}
