namespace WX.B2C.User.Verification.Provider.Contracts.Models
{
    public class CheckProcessingResult
    {
        protected CheckProcessingResult(CheckOutputData outputData, bool isPassed)
        {
            OutputData = outputData;
            IsPassed = isPassed;
        }

        public bool IsPassed { get; }

        public string Decision { get; private set; }

        public CheckOutputData OutputData { get; }

        public static CheckProcessingResult Passed(CheckOutputData outputData) =>
            new CheckProcessingResult(outputData, true);

        public static CheckProcessingResult Failed(CheckOutputData outputData, string decision = null) =>
            new CheckProcessingResult(outputData, false) { Decision = decision };
    }
}