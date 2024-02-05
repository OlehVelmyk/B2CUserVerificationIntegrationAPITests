using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public abstract class B2CVerificationException : Exception
    {
        protected B2CVerificationException()
        {
        }

        protected B2CVerificationException(string message)
            : base(message)
        {
        }

        protected B2CVerificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected B2CVerificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}