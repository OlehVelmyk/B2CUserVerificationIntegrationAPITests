using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.PassFort.Exceptions
{
    [Serializable]
    public class PassFortApiException : B2CVerificationException
    {
        public PassFortApiException(string message)
            : base(message)
        {
        }

        protected PassFortApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
