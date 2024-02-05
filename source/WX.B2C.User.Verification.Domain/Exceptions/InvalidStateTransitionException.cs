using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class InvalidStateTransitionException : B2CVerificationException
    {
        public InvalidStateTransitionException(string modelName, Enum fromState, Enum toState)
            : base($"{modelName} state cannot be changed from {fromState} to {toState}.")
        {
            ModelName = modelName;
            FromState = fromState.ToString();
            ToState = toState.ToString();
        }

        protected InvalidStateTransitionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            ModelName = info.GetString(nameof(ModelName));
            FromState = info.GetString(nameof(FromState));
            ToState = info.GetString(nameof(ToState));
        }

        public static InvalidStateTransitionException For<TModel>(Enum currentState, Enum newState)
            where TModel : AggregateRoot =>
            new(typeof(TModel).Name, currentState, newState);

        public string ModelName { get; set; }

        public string FromState { get; set; }

        public string ToState { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ModelName), ModelName);
            info.AddValue(nameof(FromState), FromState);
            info.AddValue(nameof(ToState), ToState);

            base.GetObjectData(info, context);
        }
    }
}