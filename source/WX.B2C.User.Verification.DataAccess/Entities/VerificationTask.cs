using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class VerificationTask
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid VariantId { get; set; }

        public TaskType Type { get; set; }

        public TaskState State { get; set; }

        public TaskResult? Result { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsExpired { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public TaskExpirationReason? ExpirationReason { get; set; }

        // TODO: Remove
        public Guid[] AcceptanceCheckIds { get; set; }

        public HashSet<TaskCheck> PerformedChecks { get; set; }

        public HashSet<TaskCollectionStep> CollectionSteps { get; set; }
    }
}
