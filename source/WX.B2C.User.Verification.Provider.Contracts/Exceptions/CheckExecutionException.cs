using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Contracts.Exceptions
{
    [Serializable]
    [KnownType(typeof(CheckError))]
    public class CheckExecutionException : B2CVerificationException
    {
        public CheckExecutionException(string code, string message, IDictionary<string, object> additionalData = null)
            : this(CheckError.Create(code, message, additionalData))
        {
        }

        public CheckExecutionException(CheckError error) 
            : base($"{error?.Code}:{error?.Message}")
        {
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }

        protected CheckExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Error = (CheckError)info.GetValue(nameof(Error), typeof(CheckError));
        }

        public CheckError Error { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Error), Error);
            base.GetObjectData(info, context);
        }
    }
}
