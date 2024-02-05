using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Clients
{
    internal interface IThrottledOnfidoApiClientFactory
    {
        IOnfidoApiClientWithThrottling Create(int maxRequestsPerMinute);
    }

    internal class ThrottledOnfidoApiClientFactory : IThrottledOnfidoApiClientFactory
    {
        private readonly IOnfidoApiClientFactory _clientFactory;

        public ThrottledOnfidoApiClientFactory(IOnfidoApiClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public IOnfidoApiClientWithThrottling Create(int maxRequestsPerMinute) =>
            new OnfidoApiClientWithThrottling(_clientFactory, maxRequestsPerMinute);
    }

    internal interface IOnfidoApiClientWithThrottling
    {
        Task<Extraction> GetExtractions(ExtractionRequest request);

        Task FindDocumentAsync(string fileExternalId);

        Task<LivePhotoList> ListLivePhotosAsync(string applicantId);

        Task<LiveVideoList> ListLiveVideosAsync(string applicantId);

        Task<byte[]> DownloadLivePhotoAsync(string photoId);

        Task<byte[]> DownloadLiveVideoAsync(string videoId);
    }

    internal class OnfidoApiClientWithThrottling : IOnfidoApiClientWithThrottling
    {
        private readonly SemaphoreSlim _apiClientLock = new(1, 1);
        private readonly Stopwatch _requestProcessingStartStopwatch = new();
        private readonly TimeSpan _requestTimeSlot;
        private readonly IOnfidoApiClientFactory _clientFactory;

        public OnfidoApiClientWithThrottling(IOnfidoApiClientFactory clientFactory, int maxRequestsPerMinute)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _requestTimeSlot = TimeSpan.FromTicks(TimeSpan.FromMinutes(1).Ticks / maxRequestsPerMinute);
        }

        public Task<Extraction> GetExtractions(ExtractionRequest request) =>
            ExecuteWithThrottlingAsync((client) => client.Extractions.FindAsync(request));

        public Task FindDocumentAsync(string fileExternalId) => 
            ExecuteWithThrottlingAsync((client) => client.Documents.FindAsync(fileExternalId));

        public Task<LivePhotoList> ListLivePhotosAsync(string applicantId) =>
            ExecuteWithThrottlingAsync((client) => client.LivePhotos.ListAsync(applicantId));

        public Task<LiveVideoList> ListLiveVideosAsync(string applicantId) =>
            ExecuteWithThrottlingAsync((client) => client.LiveVideos.ListAsync(applicantId));

        public Task<byte[]> DownloadLivePhotoAsync(string photoId) =>
            ExecuteWithThrottlingAsync((client) => client.LivePhotos.DownloadAsync(photoId));

        public Task<byte[]> DownloadLiveVideoAsync(string videoId) =>
            ExecuteWithThrottlingAsync((client) => client.LiveVideos.DownloadAsync(videoId));

        private async Task<TResponse> ExecuteWithThrottlingAsync<TResponse>(Func<IOnfidoApiClient, Task<TResponse>> requestFunc)
        {
            await _apiClientLock.WaitAsync();

            try
            {
                if (_requestProcessingStartStopwatch.IsRunning)
                {
                    _requestProcessingStartStopwatch.Stop();

                    var timeToWait = _requestTimeSlot - _requestProcessingStartStopwatch.Elapsed;
                    if (timeToWait > TimeSpan.Zero)
                    {
                        await Task.Delay(timeToWait);
                    }
                }

                _requestProcessingStartStopwatch.Restart();
                using var client = _clientFactory.Create();
                var response = await requestFunc(client);
                return response;
            }
            catch (OnfidoApiErrorException exc)
            {
                throw new OnfidoApiException(exc.Message);
            }
            finally
            {
                _apiClientLock.Release();
            }
        }
    }
}
