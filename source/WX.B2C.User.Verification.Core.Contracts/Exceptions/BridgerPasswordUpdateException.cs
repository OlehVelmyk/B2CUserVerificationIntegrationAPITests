using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Exceptions;

namespace WX.B2C.User.Verification.Core.Contracts.Exceptions
{
    [Serializable]
    public class BridgerPasswordUpdateException : B2CVerificationException
    {
        public BridgerPasswordUpdateException(string userId)
            : base($"Bridger password update failed for user {userId}.")
        {
        }

        protected BridgerPasswordUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            UserId = info.GetString(nameof(UserId));
        }

        public string UserId { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(UserId), UserId);

            base.GetObjectData(info, context);
        }
    }
}
