using System;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public class CheckSpecimen
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public CheckType Type { get; set; }

        public CheckVariant Variant { get; set; }

        public CheckState State { get; set; }

        public CheckExecutionContext ExecutionContext { get; set; }

        public CheckProcessingResult ProcessingResult { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? PerformedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
