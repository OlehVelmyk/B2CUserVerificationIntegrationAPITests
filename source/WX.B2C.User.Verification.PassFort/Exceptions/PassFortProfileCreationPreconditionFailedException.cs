using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.PassFort.Exceptions
{
    [Serializable]
    public class PassFortProfileCreationPreconditionFailedException : PassFortApiException
    {
        public PassFortProfileCreationPreconditionFailedException(Guid userId)
            : base($"User: {userId} address is unset.")
        {
        }

        protected PassFortProfileCreationPreconditionFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
}
}
