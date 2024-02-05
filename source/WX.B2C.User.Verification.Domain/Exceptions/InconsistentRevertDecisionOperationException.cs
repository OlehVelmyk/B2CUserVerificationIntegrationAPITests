using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class InconsistentRevertDecisionOperationException : B2CVerificationException
    {
        public InconsistentRevertDecisionOperationException(Guid applicationId)
            : base("Cannot revert decision when previous state is undefined.")
        {
            ApplicationId = applicationId.ToString();
        }

        protected InconsistentRevertDecisionOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            ApplicationId = info.GetString(nameof(ApplicationId));
        }

        public string ApplicationId { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ApplicationId), ApplicationId);

            base.GetObjectData(info, context);
        }
    }
}