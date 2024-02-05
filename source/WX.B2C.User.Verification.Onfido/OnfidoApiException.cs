using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Onfido
{
    [Serializable]
    public class OnfidoApiException : B2CVerificationException
    {
        public OnfidoApiException(string message) : base(message)
        {
        }

        protected OnfidoApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
