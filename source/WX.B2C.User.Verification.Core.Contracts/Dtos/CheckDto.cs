using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CheckDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public CheckType Type { get; set; }

        public CheckVariantDto Variant { get; set; }

        public CheckState State { get; set; }

        public CheckResult? Result { get; set; }

        public string Decision { get; set; }

        public CheckInputDataDto InputData { get; set; }

        public CheckExternalDataDto ExternalData { get; set; }

        public string OutputData { get; set; }

        public CheckErrorDto[] Errors { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? PerformedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}