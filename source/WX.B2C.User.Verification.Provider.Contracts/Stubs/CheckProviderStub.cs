using System;
using System.Threading.Tasks;
using Optional;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Provider.Contracts.Stubs;

namespace WX.B2C.User.Verification.Provider.Contracts
{
    public class CheckProviderStub : CheckProvider
    {
        private CheckProcessingResult _checkResult;
        private readonly ICheckProcessingResultFactory _checkProcessingResultFactory;

        public CheckProviderStub(ICheckProcessingResultFactory checkProcessingResultFactory)
        {
            _checkProcessingResultFactory = checkProcessingResultFactory ?? throw new ArgumentNullException(nameof(checkProcessingResultFactory));
        }

        public override Task<Option<CheckRunningResult, Exception>> RunAsync(CheckInputData inputData)
        {
            _checkResult = _checkProcessingResultFactory.Create(inputData);
            var runningResult = CheckRunningResult.Completed(_checkResult);
            return Task.FromResult(runningResult.Some<CheckRunningResult, Exception>());
        }

        public override Task<Option<CheckProcessingResult, Exception>> GetResultAsync(CheckProcessingContext context) =>
            Task.FromResult(_checkResult.Some<CheckProcessingResult, Exception>());
    }
}