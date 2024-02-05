using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Provider.Contracts.Exceptions
{
    [Serializable]
    [KnownType(typeof(string[]))]
    public class CheckInputValidationException : B2CVerificationException
    {
        public CheckInputValidationException(params string[] missingData)
            : base("Not all required data for check is provided.")
        {
            MissingData = missingData;
        }

        protected CheckInputValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
            MissingData = (string[]) info.GetValue(nameof(MissingData), typeof(string[]));
        }

        public string[] MissingData { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(MissingData), MissingData);
            base.GetObjectData(info, context);
        }
    }
}
