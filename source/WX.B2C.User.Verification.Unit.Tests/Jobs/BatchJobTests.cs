using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Quartz;
using Serilog.Core;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using Constants = WX.B2C.User.Verification.Worker.Jobs.Models.Constants;

namespace WX.B2C.User.Verification.Unit.Tests.Jobs
{
    [TestFixture]
    internal class BatchJobTests
    {
        private JobDataMap _dataMap;
        private IJobExecutionContext _context;

        [SetUp]
        public void Setup()
        {
            _context = Substitute.For<IJobExecutionContext>();
            _dataMap = Substitute.For<JobDataMap>();
            _context.MergedJobDataMap.Returns(_ => _dataMap);
        }

        [Theory(MaxTest = 10)]
        public async Task ShouldProcessInCorrectOrderAsync(PositiveInt processBatchSize,
                                                           PositiveInt readingBatchSize,
                                                           PositiveInt totalCount)
        {
            //Given
            var provider = new TestDataProvider(totalCount.Get);
            var job = new TestBatchJob(provider, Logger.None);

            //Arrange
            var batchJobSettings = new BatchJobSettings
            {
                ProcessBatchSize = processBatchSize.Get,
                ReadingBatchSize = readingBatchSize.Get,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0
            };

            _dataMap.GetString(Constants.JobSettings).Returns(JsonConvert.SerializeObject(batchJobSettings));

            //Act
            await job.Execute(_context);

            //Arrange
            job.ProcessedData.Should().BeEquivalentTo(provider.Data);
        }

        [TestCase(3, 10, 21, 7)]
        [TestCase(3, 10, 22, 8)]
        [TestCase(3, 10, 9, 3)]
        [TestCase(3, 10, 2, 1)]
        [TestCase(10, 3, 10, 1)]
        [TestCase(10, 3, 11, 2)]
        public async Task ShouldCorrectlyCalculateTotalBatchCount(int processBatchSize,
                                                                  int readingBatchSize,
                                                                  int totalCount,
                                                                  int expectedProcessingBatchCount)
        {
            //Given
            var provider = new TestDataProvider(totalCount);
            var job = new TestBatchJob(provider, Logger.None);

            //Arrange
            var batchJobSettings = new BatchJobSettings
            {
                ProcessBatchSize = processBatchSize,
                ReadingBatchSize = readingBatchSize,
                DelayInMillisecondsAfterBatch = 0,
                MaxErrorCount = 0
            };

            _dataMap.GetString(Constants.JobSettings).Returns(JsonConvert.SerializeObject(batchJobSettings));

            //Act
            await job.Execute(_context);

            //Arrange
            job.TotalBatchCount.Should().Be(expectedProcessingBatchCount);
            job.ExpectedBatchCount.Should().Be(expectedProcessingBatchCount);
        }
    }
}