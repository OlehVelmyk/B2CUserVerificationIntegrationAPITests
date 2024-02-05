using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Worker.Jobs.Jobs;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs
{
    internal class TestBatchJob : BatchJob<TestData, BatchJobSettings>
    {
        public List<TestData> ProcessedData { get; } = new();

        public int TotalBatchCount { get; private set; }

        public int ExpectedBatchCount { get; private set; }

        public TestBatchJob(IBatchJobDataProvider<TestData, BatchJobSettings> jobDataProvider, ILogger logger)
            : base(jobDataProvider, logger)
        {
        }

        protected override Task Execute(Batch<TestData> batch, BatchJobSettings settings, CancellationToken cancellationToken)
        {
            ExpectedBatchCount = batch.TotalCount;
            TotalBatchCount++;
            ProcessedData.AddRange(batch.Items);
            return Task.CompletedTask;
        }
    }

    public class TestDataProvider : IBatchJobDataProvider<TestData, BatchJobSettings>
    {
        private readonly int _totalCount;

        public List<TestData> Data { get; }

        public TestDataProvider(int totalCount)
        {
            _totalCount = totalCount;
            Data = new List<TestData>();
            for (var i = 0; i < _totalCount; i++)
            {
                Data.Add(new TestData { Number = i });
            }
        }

        public Task<int> GetTotalCountAsync(BatchJobSettings settings, CancellationToken cancellationToken)
        {
            return Task.FromResult(_totalCount);
        }

        public async IAsyncEnumerable<ICollection<TestData>> GetAsync(BatchJobSettings settings, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var page = 0;
            var numberOfElementsInLastBatch = 0;
            var readingBatchSize = settings.ReadingBatchSize;
            do
            {
                await Task.Delay(1, cancellationToken);
                var batch = Data.Skip(readingBatchSize * page).Take(readingBatchSize).ToArray();
                numberOfElementsInLastBatch = batch.Length;
                yield return batch;

                page++;
            } while (readingBatchSize == numberOfElementsInLastBatch);
        }
    }

    public class TestData : IJobData
    {
        public int Number { get; set; }
    }
}