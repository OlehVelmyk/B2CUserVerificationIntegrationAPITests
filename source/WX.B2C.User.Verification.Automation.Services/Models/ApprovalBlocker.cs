using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Automation.Services.Approve
{
    public enum ApprovalBlockerType
    {
        IncompleteTask = 1,
        FailedTask = 2,
        RiskLevelNotEvaluated = 3,
        TriggerIsFiring = 4,
    }

    public class ApprovalBlocker
    {
        protected ApprovalBlocker(ApprovalBlockerType type)
        {
            Type = type;
        }

        public ApprovalBlockerType Type { get; }

        public static ApprovalBlocker RiskLevelNotEvaluated() => 
            new(ApprovalBlockerType.RiskLevelNotEvaluated);

        public static IncompleteTaskApprovalBlocker IncompleteTask(TaskType taskType, Guid variantId, bool isManual) =>
            new(taskType, variantId, isManual);

        public static FailedTaskApprovalBlocker FailedTask(TaskType taskType, Guid variantId) => 
            new(taskType, variantId);
        
        public static ApprovalBlocker TriggerIsFiring() => 
            new(ApprovalBlockerType.TriggerIsFiring);
    }

    public class IncompleteTaskApprovalBlocker : ApprovalBlocker
    {
        public IncompleteTaskApprovalBlocker(TaskType taskType, Guid variantId, bool isManual)
            : base(ApprovalBlockerType.IncompleteTask)
        {
            TaskType = taskType;
            VariantId = variantId;
            IsManual = isManual;
        }

        public TaskType TaskType { get; }

        public Guid VariantId { get; }

        public bool IsManual { get; }
    }

    public class FailedTaskApprovalBlocker : ApprovalBlocker
    {
        public FailedTaskApprovalBlocker(TaskType taskType, Guid variantId)
            : base(ApprovalBlockerType.FailedTask)
        {
            TaskType = taskType;
            VariantId = variantId;
        }

        public TaskType TaskType { get; }

        public Guid VariantId { get; }
    }
}