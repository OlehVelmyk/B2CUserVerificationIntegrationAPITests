using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain
{
    public class CheckError : ValueObject
    {
        private CheckError(string code, string message, IReadOnlyDictionary<string, object> additionalData)
        {
            Code = code;
            Message = message;
            AdditionalData = additionalData;
        }

        public string Code { get; }

        public string Message { get; }

        public IReadOnlyDictionary<string, object> AdditionalData { get; }

        public static CheckError Create(string code, string message, IReadOnlyDictionary<string, object> additionalData = null) =>
            new (code, message, additionalData);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Code;
            yield return Message;
        }
    }
}