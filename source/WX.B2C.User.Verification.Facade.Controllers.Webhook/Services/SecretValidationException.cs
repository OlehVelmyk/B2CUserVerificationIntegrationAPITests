using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Services
{
    [Serializable]
    internal class SecretValidationException : B2CVerificationException
    {
        public SecretValidationException(string secret)
            : base($"Secret '{secret}' is not valid.")
        {
        }

        public SecretValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
