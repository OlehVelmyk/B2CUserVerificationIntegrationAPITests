using System;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public abstract class CheckRunner<TData>
    {
        public abstract Task<CheckRunningResult> RunAsync(TData inputData, CancellationToken cancellationToken = default);

        public abstract Task<CheckProcessingResult> GetResultAsync(CheckProcessingContext context, CancellationToken cancellationToken = default);
    }

    public abstract class AsyncCheckRunner<TData> : CheckRunner<TData>
    {
    }

    public abstract class SyncCheckRunner<TData> : CheckRunner<TData>
    {
        private CheckProcessingResult _checkResult;

        public override async Task<CheckRunningResult> RunAsync(TData checkData, CancellationToken cancellationToken = default)
        {
            var checkResult = await RunSync(checkData);

            _checkResult = checkResult ?? throw new ArgumentNullException(nameof(checkResult), "Check processing result should not be null.");

            return CheckRunningResult.Completed(checkResult);
        }

        public override Task<CheckProcessingResult> GetResultAsync(CheckProcessingContext context, CancellationToken cancellationToken = default) =>
            Task.FromResult(_checkResult);

        protected abstract Task<CheckProcessingResult> RunSync(TData data);
    }
}