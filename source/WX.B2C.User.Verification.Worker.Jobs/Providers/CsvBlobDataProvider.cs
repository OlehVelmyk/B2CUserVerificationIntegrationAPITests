using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface ICsvBlobDataProvider<TData> : IBatchJobDataProvider<TData, CsvBlobJobSettings> where TData : IJobData {}

    internal class CsvBlobDataProvider<TData> : ICsvBlobDataProvider<TData> where TData : IJobData
    {
        private readonly ICsvBlobStorage _csvBlobStorage;

        public CsvBlobDataProvider(ICsvBlobStorage csvBlobStorage)
        {
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
        }

        public async IAsyncEnumerable<ICollection<TData>> GetAsync(CsvBlobJobSettings settings,
                                                                   [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var items = await _csvBlobStorage.GetAsync<TData>(settings.ContainerName, settings.FileName);
            var size = settings.ReadingBatchSize;
            var pageCount = (items.Length - 1) / size + 1;

            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
                yield return items.Skip(page * size).Take(size).ToArray();
            }
        }

        public async Task<int> GetTotalCountAsync(CsvBlobJobSettings settings, CancellationToken cancellationToken)
        {
            var items = await _csvBlobStorage.GetAsync<TData>(settings.ContainerName, settings.FileName);
            return items.Length;
        }
    }
}
