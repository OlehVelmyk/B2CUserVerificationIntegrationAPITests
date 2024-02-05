using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class CollectionStepAlreadyCompletedException : CollectionStepException
    {
        public CollectionStepAlreadyCompletedException(Guid id, string xPath)
            : base(id, xPath, $"Step {id}:{xPath} already completed.")
        {
        }

        protected CollectionStepAlreadyCompletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}