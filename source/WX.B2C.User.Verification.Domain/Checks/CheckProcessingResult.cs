using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Checks
{
    public class CheckProcessingResult : ValueObject
    {
        private CheckProcessingResult(CheckResult result, string decision, string outputData)
        {
            Result = result;
            Decision = decision;
            OutputData = outputData;
        }

        public CheckResult Result { get; }

        public string Decision { get; }

        public string OutputData { get; }

        public static CheckProcessingResult Create(CheckResult result, string decision, string outputData) =>
            new (result, decision, outputData);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Result;
            yield return Decision;
            yield return OutputData;
        }
    }
}
