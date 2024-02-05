using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class UserMismatchedException : B2CVerificationException
    {
        public UserMismatchedException(AggregateRoot<Guid> parent, AggregateRoot<Guid> child)
            : base($"{parent.GetType().Name} with id {parent.Id} and {child.GetType().Name} with id {child.Id} linked to different users.")
        {
        }

        protected UserMismatchedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}