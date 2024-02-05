using System;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class TaskExpiredException : B2CVerificationException
    {
        public TaskExpiredException(Guid applicationId, Guid taskId)
            : base($"Can not add expired task to application {applicationId}, TaskId:{taskId}.")
        {
            ApplicationId = applicationId.ToString();
            TaskId = taskId.ToString();
        }

        protected TaskExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            TaskId = info.GetString(nameof(TaskId));
            ApplicationId = info.GetString(nameof(ApplicationId));
        }

        public string ApplicationId { get; set; }

        public string TaskId { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ApplicationId), ApplicationId);
            info.AddValue(nameof(TaskId), TaskId);

            base.GetObjectData(info, context);
        }
    }
}