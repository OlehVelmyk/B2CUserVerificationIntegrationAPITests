using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class CollectionStepReviewProhibitedException : CollectionStepException
    {
        public CollectionStepReviewProhibitedException(Guid id, string xPath)
            : base(id, xPath, $"Collection step {id}:{xPath} does not require manual review.")
        {
        }

        protected CollectionStepReviewProhibitedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}