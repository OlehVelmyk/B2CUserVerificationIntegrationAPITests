using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class CheckAlreadyCompletedException : B2CVerificationException
    {
        public CheckAlreadyCompletedException(Guid id, CheckState state)
            : base($"Check {id} already completed with state {state}.")
        {
            CheckId = id.ToString();
            State = state.ToString();
        }

        protected CheckAlreadyCompletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            CheckId = info.GetString(nameof(CheckId));
            State = info.GetString(nameof(State));
        }

        public string CheckId { get; set; }

        public string State { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(CheckId), CheckId);
            info.AddValue(nameof(State), State);

            base.GetObjectData(info, context);
        }
    }
}