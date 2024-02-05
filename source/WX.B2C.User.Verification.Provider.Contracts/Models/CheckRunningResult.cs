using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Checks;

namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    public abstract class CheckRunningResult
    {
        public static AsyncCheckRunningResult Instructed(string externalId, IReadOnlyDictionary<string, object> additionalData = null)
        {
            if (string.IsNullOrEmpty(externalId))
                throw new ArgumentNullException(nameof(externalId));

            additionalData ??= new Dictionary<string, object>();
            var externalData = new Dictionary<string, object>(additionalData)
            {
                [ExternalCheckProperties.ExternalId] = externalId
            };

            return new AsyncCheckRunningResult(externalData);
        }

        public static SyncCheckRunningResult Completed(CheckProcessingResult processingResult) => new(processingResult);
    }

    public class SyncCheckRunningResult : CheckRunningResult
    {
        public SyncCheckRunningResult(CheckProcessingResult processingResult)
        {
            ProcessingResult = processingResult ?? throw new ArgumentNullException(nameof(processingResult));
        }

        public CheckProcessingResult ProcessingResult { get; }
    }

    public class AsyncCheckRunningResult : CheckRunningResult
    {
        public AsyncCheckRunningResult(IReadOnlyDictionary<string, object> externalData)
        {
            ExternalData = externalData ?? throw new ArgumentNullException(nameof(externalData));
        }

        public IReadOnlyDictionary<string, object> ExternalData { get; }
    }
}