using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    [KnownType(typeof(string[]))]
    public class ApproveApplicationException : B2CVerificationException
    {
        public ApproveApplicationException(Guid applicationId, IEnumerable<Guid> incompleteTasks)
            : base($"Application {applicationId} cannot be approved because not all tasks are completed.")
        {
            ApplicationId = applicationId.ToString();
            TaskIds = incompleteTasks?.Select(id => id.ToString()).ToArray();
        }

        protected ApproveApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ApplicationId = info.GetString(nameof(ApplicationId));
            TaskIds = (string[])info.GetValue(nameof(TaskIds), typeof(string[]));
        }

        public string ApplicationId { get; set; }

        public string[] TaskIds { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ApplicationId), ApplicationId);
            info.AddValue(nameof(TaskIds), TaskIds);

            base.GetObjectData(info, context);
        }
    }
}