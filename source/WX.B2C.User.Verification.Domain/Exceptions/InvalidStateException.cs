using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class InvalidStateException : B2CVerificationException
    {
        public InvalidStateException(string modelName, Guid id, Enum state)
            : base($"{modelName} with {id} must be in {state} state.") { }

        protected InvalidStateException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public static InvalidStateException Create<TModel>(Guid id, Enum state)
            where TModel : AggregateRoot =>
            new(typeof(TModel).Name, id, state);
    }
}
