using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Contracts.Stubs
{
    public interface ICheckProcessingResultFactory
    {
        CheckProcessingResult Create(CheckInputData inputData);
    }

    public abstract class PassedCheckProcessingResultFactory : ICheckProcessingResultFactory
    {
        public CheckProcessingResult Create(CheckInputData inputData)
        {
            var checkOutput = CreateCheckOutput(inputData);
            return CheckProcessingResult.Passed(checkOutput);
        }

        protected abstract CheckOutputData CreateCheckOutput(CheckInputData inputData);
    }

    public class PlainCheckProcessingResultFactory<TCheckOutput> : PassedCheckProcessingResultFactory
        where TCheckOutput : CheckOutputData, new()
    {
        protected override CheckOutputData CreateCheckOutput(CheckInputData inputData) =>
            new TCheckOutput();
    }
}