using System;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain.Exceptions
{
    [Serializable]
    public class ApplicationTaskAlreadyExistsException : B2CVerificationException
    {
        public ApplicationTaskAlreadyExistsException(VerificationTask newTask, Guid existingTaskId)
            : base($"Prohibited to add task with the same type {newTask.Type}.")
        {
            ExistingTaskId = existingTaskId.ToString();
            NewTaskId = newTask.Id.ToString();
            TaskVariantId = newTask.VariantId.ToString();
            TaskType = newTask.Type.ToString();
        }

        protected ApplicationTaskAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ExistingTaskId = info.GetString(nameof(ExistingTaskId));
            NewTaskId = info.GetString(nameof(NewTaskId));
            TaskVariantId = info.GetString(nameof(TaskVariantId));
            TaskType = info.GetString(nameof(TaskType));
        }

        public string ExistingTaskId { get; set; }

        public string NewTaskId { get; set; }

        public string TaskVariantId { get; set; }

        public string TaskType { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ExistingTaskId), ExistingTaskId);
            info.AddValue(nameof(NewTaskId), NewTaskId);
            info.AddValue(nameof(TaskVariantId), TaskVariantId);
            info.AddValue(nameof(TaskType), TaskType);

            base.GetObjectData(info, context);
        }
    }
}