using System;

namespace WX.B2C.User.Verification.Domain.Models
{
    public class TaskCheck : IEquatable<TaskCheck>
    {
        public TaskCheck(Guid id, Guid variantId, CheckType type, CheckState state, CheckResult? result)
        {
            Id = id;
            VariantId = variantId;
            Type = type;
            State = state;
            Result = result;
        }

        public Guid Id { get; }

        public Guid VariantId { get; }

        public CheckType Type { get; }

        public CheckState State { get; }

        public CheckResult? Result { get; }

        public static TaskCheck Create(Check check) =>
            new (check.Id, check.Variant.Id, check.Type, check.State, check.ProcessingResult?.Result);

        public bool Equals(TaskCheck other) => other is not null && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}