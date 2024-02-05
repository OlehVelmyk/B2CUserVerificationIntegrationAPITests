using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain
{
    public class CheckExecutionContext : ValueObject
    {
        public CheckExecutionContext(
            IReadOnlyDictionary<string, object> inputData,
            IReadOnlyDictionary<string, object> externalData = null)
        {
            InputData = inputData ?? throw new ArgumentNullException(nameof(inputData));
            ExternalData = externalData;
        }

        public IReadOnlyDictionary<string, object> InputData { get; }

        public IReadOnlyDictionary<string, object> ExternalData { get; }

        public static CheckExecutionContext Create(
            IReadOnlyDictionary<string, object> inputData,
            IReadOnlyDictionary<string, object> externalData = null) =>
            new CheckExecutionContext(inputData, externalData);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return InputData;
            yield return ExternalData;
        }
    }
}