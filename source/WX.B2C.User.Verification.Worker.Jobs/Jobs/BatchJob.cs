using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs
{
    internal abstract class BatchJob<TData, TSettings> : IJob
        where TData : IJobData
        where TSettings : BatchJobSettings
    {
        private const string ReadingBatchNumber = nameof(ReadingBatchNumber);
        private const string ProcessingBatchNumber = nameof(ProcessingBatchNumber);

        private readonly IBatchJobDataProvider<TData, TSettings> _jobDataProvider;
        private int _totalErrorCount;

        protected BatchJob(IBatchJobDataProvider<TData, TSettings> jobDataProvider, ILogger logger)
        {
            _jobDataProvider = jobDataProvider ?? throw new ArgumentNullException(nameof(jobDataProvider));
            Logger = logger?.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(logger));
        }

        protected ILogger Logger { get; }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobType = context.JobDetail.JobType;
            var settings = context.GetSettings<TSettings>();

            var logger = Logger
                         .ForContext(nameof(context.FireInstanceId), context.FireInstanceId)
                         .ForContext(nameof(context.JobDetail.JobType), jobType);

            var totalItemsToProcess = await _jobDataProvider.GetTotalCountAsync(settings, context.CancellationToken);
            var totalBatchCount = (totalItemsToProcess - 1) / settings.ProcessBatchSize + 1;

            logger.Information(
            "Job execution started. Total items to process: {TotalItems}. Total processing batches: {TotalBatchCount}. Settings: {@Settings}",
            totalItemsToProcess,
            totalBatchCount,
            settings);

            var buffer = new List<TData>();
            var readingBatchNumber = 0;
            var processingBatchNumber = 0;

            var startTime = DateTime.UtcNow;
            var isFault = false;
            var asyncEnumerable = _jobDataProvider.GetAsync(settings, context.CancellationToken);


            try
            {

                await foreach (var readBatch in asyncEnumerable.WithCancellation(context.CancellationToken))
                {
                    try
                    {
                        var items = buffer.RemoveAll().Concat(readBatch).ToArray();
                        if (!items.Any()) break;

                        var batches = PrepareProcessingBatches(items, settings, totalBatchCount);
                        var isLastReadBatch = readBatch.Count != settings.ReadingBatchSize;
                        if (!isLastReadBatch)
                        {
                            var lastBatch = batches.Last();
                            if (lastBatch.Items.Length < settings.ProcessBatchSize)
                            {
                                batches = batches[..^1];
                                buffer.AddRange(lastBatch.Items);
                            }
                        }

                        foreach (var batch in batches)
                        {
                            if (context.CancellationToken.IsCancellationRequested)
                                break;

                            if (settings.ProcessingBatchOffset > processingBatchNumber)
                            {
                                processingBatchNumber++;
                                Logger.Information("Skipped batch {ProcessingBatch}. Offset is {ProcessingBatchOffset}",
                                                   processingBatchNumber,
                                                   settings.ProcessingBatchOffset);
                                continue;
                            }

                            await Execute(batch, settings, context.CancellationToken);

                            if (_totalErrorCount > settings.MaxErrorCount)
                                await context.Scheduler.Interrupt(context.FireInstanceId, context.CancellationToken);

                            await Task.Delay(settings.DelayInMillisecondsAfterBatch, context.CancellationToken);

                            context.Put(ProcessingBatchNumber, ++processingBatchNumber);
                            LogProgress(logger, startTime, totalBatchCount, processingBatchNumber);
                        }

                        context.Put(ReadingBatchNumber, ++readingBatchNumber);
                    }
                    catch (OperationCanceledException)
                    {
                        logger.Warning(
                        "Job has been cancelled on reading batch:{ReadingBatch} and processing batch {ProcessingBatch}. Error count {ErrorCount}",
                        readingBatchNumber,
                        context.Get(ProcessingBatchNumber),
                        _totalErrorCount);
                    }
                    catch (Exception exc)
                    {
                        isFault = true;
                        // Not handled exceptions in ProcessBatch are evaluated like critical. Job is stopped
                        logger.Error(exc, "Job has failed with unexpected exception.");
                        throw new JobExecutionException(exc, false);
                    }
                }
            }
            finally
            {
                try
                {
                    await FinalizeJob(settings, isFault);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Exception during finalizing job");
                }

            }
        }

        protected virtual Task FinalizeJob(TSettings setting, bool isFault) =>
            Task.CompletedTask;

        private static void LogProgress(ILogger logger,
                                        DateTime startTime,
                                        int totalBatchNumbers,
                                        int currentBatchNumber)
        {
            var estimate = Estimate(startTime, totalBatchNumbers, currentBatchNumber);
            logger.Information("Processed batch {CurrentBatchNumber} of {TotalBatchCount}. "
                             + "Estimated finishing time {EstimatedFinish}",
                               currentBatchNumber,
                               totalBatchNumbers,
                               estimate);
        }

        private static DateTime Estimate(DateTime startTime, int totalBatchNumbers, int currentBatchNumber)
        {
            var currentTime = DateTime.UtcNow;
            var averageTimePerBatch = (currentTime - startTime) / currentBatchNumber;
            return startTime.Add(averageTimePerBatch * totalBatchNumbers);
        }

        private static Batch<TData>[] PrepareProcessingBatches(IEnumerable<TData> items, BatchJobSettings settings, int totalBatchCount) =>
            items.Batch(settings.ProcessBatchSize)
                 .Select((batch, index) => Batch.Create(batch.ToArray(), totalBatchCount, index + 1))
                 .ToArray();

        protected abstract Task Execute(Batch<TData> batch, TSettings settings, CancellationToken cancellationToken);

        protected void IncrementErrorCount(int newErrorsCount = 1)
        {
            if (newErrorsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(newErrorsCount), @"Must be positive");

            _totalErrorCount += newErrorsCount;
        }
    }
}