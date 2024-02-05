using System;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class TaskDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public TaskType Type { get; set; }

        public TaskState State { get; set; }

        public Guid VariantId { get; set; }

        public TaskCollectionStepDto[] CollectionSteps { get; set; }

        public TaskCheckDto[] Checks { get; set; }

        public TaskResult? Result { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class TaskCheckDto
    {
        public Guid Id { get; set; }

        public Guid VariantId { get; set; }

        public CheckType Type { get; set; }

        public CheckProviderType Provider { get; set; }

        public CheckState State { get; set; }

        public CheckResult? Result { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }

    public class TaskCollectionStepDto
    {
        public Guid Id { get; set; }

        public string XPath { get; set; }

        public CollectionStepState State { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public CollectionStepReviewResult? ReviewResult { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}