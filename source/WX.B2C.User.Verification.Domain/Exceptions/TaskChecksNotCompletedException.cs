using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    [KnownType(typeof(string[]))]
    public class TaskChecksNotCompletedException : B2CVerificationException
    {
        public TaskChecksNotCompletedException(Guid taskId, IEnumerable<Guid> checks)
            : base($"Task {taskId} has incomplete checks.")
        {
            TaskId = taskId.ToString();
            Checks = checks?.Select(id => id.ToString()).ToArray();
        }

        protected TaskChecksNotCompletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            TaskId = info.GetString(nameof(TaskId));
            Checks = (string[])info.GetValue(nameof(Checks), typeof(string[]));
        }

        public string TaskId { get; set; }

        public string[] Checks { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(TaskId), TaskId);
            info.AddValue(nameof(Checks), Checks);

            base.GetObjectData(info, context);
        }
    }
}