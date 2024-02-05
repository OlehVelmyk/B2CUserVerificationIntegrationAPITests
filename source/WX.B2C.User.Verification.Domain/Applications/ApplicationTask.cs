using System;

namespace WX.B2C.User.Verification.Domain.Models
{
    public class ApplicationTask : IEquatable<ApplicationTask>
    {
        public ApplicationTask(Guid id, TaskState state, TaskType taskType)
        {
            Id = id;
            State = state;
            TaskType = taskType;
        }

        public Guid Id { get; }

        public TaskState State { get; }

        public bool Equals(ApplicationTask other) => other is null ? false : Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();

        public TaskType TaskType { get; }

        public static ApplicationTask Create(VerificationTask task)
        {
            return new(task.Id, task.State, task.Type);
        }
    }
}