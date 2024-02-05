using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class CollectionStepReviewRequiredException : CollectionStepException
    {
        public CollectionStepReviewRequiredException(Guid id, string xPath)
            : base(id, xPath, $"Collection step {id}:{xPath} requires manual review.")
        {
        }

        protected CollectionStepReviewRequiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}