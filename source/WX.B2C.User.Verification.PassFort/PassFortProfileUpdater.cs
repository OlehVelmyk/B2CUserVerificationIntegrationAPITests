using System;
using System.Net;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Serilog;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.Exceptions;
using WX.B2C.User.Verification.PassFort.Models;

namespace WX.B2C.User.Verification.PassFort
{
    public interface IPassFortProfileUpdater
    {
        Task UpdateAsync(string profileId, PassFortProfilePatch patch);
    }

    // TODO: Inherit from BasePassFortGateway
    internal class PassFortProfileUpdater : IPassFortProfileUpdater
    {
        private readonly IPassFortProfilePatcher _patcher;
        private readonly IPassFortApiClientFactory _clientFactory;
        private readonly ILogger _logger;

        public PassFortProfileUpdater(IPassFortProfilePatcher patcher, IPassFortApiClientFactory clientFactory, ILogger logger)
        {
            _patcher = patcher ?? throw new ArgumentNullException(nameof(patcher));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _logger = logger?.ForContext<PassFortProfileUpdater>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task UpdateAsync(string profileId, PassFortProfilePatch patch)
        {
            bool isSuccessful;
            var (profileData, etag) = await GetProfileDataAsync(profileId);
            
            await CreatePolicy(_logger).ExecuteAsync(async () =>
            {
                var (patchedValue, needToPatch) = await _patcher.ApplyPatch(profileData, patch);

                if (!needToPatch)
                    return;
                
                (isSuccessful, profileData, etag) = await TryUpdateAsync(profileId, patchedValue, etag);
                if (!isSuccessful)
                    throw new PassFortCannotUpdateProfileException(profileId);
            });
        }

        private async Task<(IndividualData, string)> GetProfileDataAsync(string profileId)
        {
            using var client = _clientFactory.Create();
            var response = await client.Profiles.GetCollectedDataWithHttpMessagesAsync(profileId);
            var etag = response.Headers.Etag;
            var profileData = response.Body;
            return (profileData, etag);
        }

        private async Task<(bool, IndividualData, string)> TryUpdateAsync(string profileId, IndividualData patchedValue, string etag)
        {
            using var client = _clientFactory.Create();
            var response = await client.Profiles.UpdateCollectedDataWithHttpMessagesAsync(profileId, patchedValue, etag);

            if (response.Response.StatusCode == HttpStatusCode.PreconditionFailed)
                return (false, response.Body, response.Headers.Etag);

            return (true, null, null);
        }

        private static AsyncRetryPolicy CreatePolicy(ILogger logger) =>
            Policy.Handle<PassFortCannotUpdateProfileException>()
                  .WaitAndRetryAsync(
                      5,
                      retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                      (exception, _, retries, _) => logger.Warning(
                          exception,
                          "Begin {RetryNumber} retry on failed request to PassFort API with error message: {ErrorMessage}",
                          retries,
                          exception.Message));
    }
}   