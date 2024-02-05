using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class CheckError
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public IReadOnlyDictionary<string, object> AdditionalData { get; set; }
    }

    internal class Check : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public CheckType Type { get; set; }

        public Guid VariantId { get; set; }

        public CheckProviderType Provider { get; set; }

        public CheckState State { get; set; }

        public string ExternalId { get; set; }

        public IReadOnlyDictionary<string, object> ExternalData { get; set; }
        
        public IReadOnlyDictionary<string, object> InputData { get; set; }

        public string OutputData { get; set; }

        public CheckResult? Result { get; set; }

        public string Decision { get; set; }

        public CheckError[] Errors { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? PerformedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}