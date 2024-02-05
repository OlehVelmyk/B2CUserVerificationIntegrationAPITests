using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface ICsvEntityProvider: IBatchJobDataProvider<Models.EntityIdentifier, CsvBlobJobSettings>
    {
        
    }
    
    internal class CsvEntityProvider : ICsvEntityProvider
    {
        private readonly ICsvBlobStorage _csvBlobStorage;

        public CsvEntityProvider(ICsvBlobStorage storage)
        {
            _csvBlobStorage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<int> GetTotalCountAsync(CsvBlobJobSettings settings, CancellationToken cancellationToken)
        {
            var users = await GetIds(settings);
            return users.Length;
        }

        public async IAsyncEnumerable<ICollection<Models.EntityIdentifier>> GetAsync(
            CsvBlobJobSettings settings,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var items = await GetIds(settings);
            var size = settings.ReadingBatchSize;
            var pageCount = (items.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                yield return items.Skip(page * size).Take(size).ToArray();
            }
        }

        private async Task<Models.EntityIdentifier[]> GetIds(CsvBlobJobSettings settings)
        {
            if (settings is IEntityProvidedSettings entityProvidedSettings && !entityProvidedSettings.Ids.IsNullOrEmpty())
                return entityProvidedSettings.Ids.Select(id => new Models.EntityIdentifier(){Id = id}).ToArray();

            var users = await _csvBlobStorage.GetAsync<Models.EntityIdentifier>(settings.ContainerName, settings.FileName);
            return users.ToArray();
        }
    }
}