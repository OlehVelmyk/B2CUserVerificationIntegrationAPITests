using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.LexisNexis.Bridger.Client.Exceptions
{
    [Serializable]
    public class BridgerUnavailableException : BridgerException
    {
        public BridgerUnavailableException()
        {
        }

        public BridgerUnavailableException(string message)
            : base(message)
        {
        }

        public BridgerUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BridgerUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
