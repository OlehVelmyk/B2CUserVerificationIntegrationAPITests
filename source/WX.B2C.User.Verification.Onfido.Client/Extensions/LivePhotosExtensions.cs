using System;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido.Client
{
    /// <summary>
    /// Extension methods for LivePhotos.
    /// </summary>
    public static partial class LivePhotosExtensions
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
        public static async Task<LivePhoto> UploadAsync(this ILivePhotos operations, string applicantId, FileToUpload file, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.UploadWithHttpMessagesAsync(applicantId, file, null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
        
        /// <summary>
        /// Download live photo
        /// </summary>
        /// <remarks>
        /// Live photos are downloaded using this endpoint.
        /// </remarks>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='livePhotoId'>
        /// The live photo’s unique identifier.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task<byte[]> DownloadAsync(this ILivePhotos operations, string livePhotoId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _result = await operations.DownloadWithHttpMessagesAsync(livePhotoId, null, cancellationToken).ConfigureAwait(false))
            {
                return _result.Body;
            }
        }
    }
}
