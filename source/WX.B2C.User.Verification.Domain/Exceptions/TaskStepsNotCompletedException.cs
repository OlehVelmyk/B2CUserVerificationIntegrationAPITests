using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    [KnownType(typeof(string[]))]
    public class TaskStepsNotCompletedException : B2CVerificationException
    {
        public TaskStepsNotCompletedException(Guid taskId, IEnumerable<Guid> steps)
            : base($"Task {taskId} has incomplete collection steps.")
        {
            TaskId = taskId.ToString();
            CollectionSteps = steps?.Select(id => id.ToString()).ToArray();
        }

        protected TaskStepsNotCompletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            TaskId = info.GetString(nameof(TaskId));
            CollectionSteps = (string[])info.GetValue(nameof(CollectionSteps), typeof(string[]));
        }

        public string TaskId { get; set; }

        public string[] CollectionSteps { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(TaskId), TaskId);
            info.AddValue(nameof(CollectionSteps), CollectionSteps);

            base.GetObjectData(info, context);
        }
    }
}