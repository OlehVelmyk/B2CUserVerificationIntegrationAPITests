using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Azure;
using WX.B2C.User.Verification.BlobStorage.Cache;
using WX.B2C.User.Verification.BlobStorage.Configurations;
using WX.B2C.User.Verification.BlobStorage.Factories;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;

namespace WX.B2C.User.Verification.BlobStorage.Storages
{
    internal interface IConfigurationBlobStorage
    {
        Task<T> GetAsync<T>();
    }

    /// <summary>
    /// Temporal implementation for configurations in blob.
    /// Later better to change into a way how implemented in b2c profile via options pattern.
    /// For now we use autofac and it is not suitable solution. 
    /// </summary>
    internal class ConfigurationBlobStorage : IConfigurationBlobStorage
    {
        private readonly IIndex<Type, BlobJsonConfiguration> _jsonConfigurationOptions;
        private readonly IBlobContainerClientFactory _blobContainerClientFactory;
        private readonly IMemoryCache _memoryCache;

        public ConfigurationBlobStorage(IIndex<Type, BlobJsonConfiguration> jsonConfigurationOptions,
                                        IBlobContainerClientFactory blobContainerClientFactory,
                                        IMemoryCache memoryCache)
        {
            _jsonConfigurationOptions = jsonConfigurationOptions ?? throw new ArgumentNullException(nameof(jsonConfigurationOptions));
            _blobContainerClientFactory = blobContainerClientFactory ?? throw new ArgumentNullException(nameof(blobContainerClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task<T> GetAsync<T>()
        {
            if (!_jsonConfigurationOptions.TryGetValue(typeof(T), out var configuration))
                throw new InvalidOperationException($"Cannot find configuration for type {typeof(T).Name}");

            return _memoryCache.GetOrCreate(configuration.PollingInterval, () => LoadFromBlobAsync<T>(configuration));
        }

        private async Task<T> LoadFromBlobAsync<T>(BlobJsonConfiguration configuration)
        {
            var container = await _blobContainerClientFactory.CreateAsync(configuration.ConnectionString, configuration.ContainerName);
            var client = container.GetBlobClient(configuration.BlobPath);
            try
            {
                var download = await client.DownloadAsync();
                var config = await JsonSerializer.DeserializeAsync<T>(download.Value.Content);
                return config;
            }
            catch (RequestFailedException rex)
            {
                if (rex.Status == (int)HttpStatusCode.NotFound)
                    throw new BlobStorageFileNotFoundException(configuration.BlobPath, configuration.ContainerName);

                throw;
            }
        }
    }
}