using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class TaskAlreadyCompletedException : B2CVerificationException
    {
        public TaskAlreadyCompletedException(Guid taskId, TaskResult result)
            : base($"Task {taskId} already completed with result {result}.")
        {
            TaskId = taskId.ToString();
            TaskResult = result.ToString();
        }

        protected TaskAlreadyCompletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            TaskId = info.GetString(nameof(TaskId));
            TaskResult = info.GetString(nameof(TaskResult));
        }

        public string TaskId { get; set; }

        public string TaskResult { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(TaskId), TaskId);
            info.AddValue(nameof(TaskResult), TaskResult);

            base.GetObjectData(info, context);
        }
    }
}