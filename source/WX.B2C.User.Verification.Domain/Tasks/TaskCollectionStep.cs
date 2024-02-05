using System;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Domain.Models
{
    public class TaskCollectionStep : IEquatable<TaskCollectionStep>
    {
        public TaskCollectionStep(Guid id, CollectionStepState state, bool isRequired)
        {
            Id = id;
            IsRequired = isRequired;
            State = state;
        }

        public Guid Id { get; }

        public CollectionStepState State { get; }

        public bool IsRequired { get; }

        public bool IsCompleted => State == CollectionStepState.Completed;

        public static TaskCollectionStep Create(CollectionStep step) =>
            new TaskCollectionStep(step.Id, step.State, step.IsRequired);

        public bool Equals(TaskCollectionStep other) => other is not null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}