using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Onfido.Client;

namespace WX.B2C.User.Verification.Onfido
{
    internal static class OnfidoApiClientExtensions
    {
        public static async Task<DownloadedFile> DownloadDocumentAsync(
            this IOnfidoApiClient client, 
            string applicantId, 
            string externalFileId, 
            CancellationToken cancellationToken = default)
        {
            var response = await client.Documents.ListAsync(applicantId, cancellationToken);
            var document = response.Documents.FirstOrDefault(doc => doc.Id == externalFileId);
            if (document == null)
                return null;

            var data = await client.Documents.DownloadAsync(externalFileId, cancellationToken);
            return DownloadedFile.Create(document.FileName, data);
        }

        public static async Task<DownloadedFile> FindLivePhotosAsync(
            this IOnfidoApiClient client, 
            string applicantId, 
            string livePhotoId, 
            CancellationToken cancellationToken = default)
        {
            var response = await client.LivePhotos.ListAsync(applicantId, cancellationToken);
            var livePhoto = response.LivePhotos.FirstOrDefault(photo => photo.Id == livePhotoId);
            if (livePhoto == null)
                return null;

            var data = await client.LivePhotos.DownloadAsync(livePhotoId, cancellationToken);
            return DownloadedFile.Create(livePhoto.FileName, data);
        }

        public static async Task<DownloadedFile> FindLiveVideoAsync(
            this IOnfidoApiClient client, 
            string applicantId, 
            string liveVideoId, 
            CancellationToken cancellationToken = default)
        {
            var response = await client.LiveVideos.ListAsync(applicantId, cancellationToken);
            var liveVideo = response.LiveVideos.FirstOrDefault(video => video.Id == liveVideoId);
            if (liveVideo == null)
                return null;

            var data = await client.LiveVideos.DownloadAsync(liveVideoId, cancellationToken);
            return DownloadedFile.Create(liveVideo.FileName, data);
        }
    }
}