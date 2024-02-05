using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class CheckVariantDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public CheckProviderType Provider { get; set; }
    }

    public class CheckDto
    {
        public Guid Id { get; set; }

        public CheckType Type { get; set; }

        public CheckVariantDto Variant { get; set; }

        public CheckState State { get; set; }

        [NotRequired]
        public CheckResult? Result { get; set; }

        [NotRequired]
        public string Decision { get; set; }

        public CollectionStepBriefDataDto[] InputData { get; set; }

        public DocumentDto[] InputDocuments { get; set; }

        /// <summary>
        /// JSON representation of check output
        /// </summary>
        [NotRequired]
        public string OutputData { get; set; }

        [NotRequired]
        public CheckErrorDto[] Errors { get; set; }

        public Guid[] RelatedTasks { get; set; }

        [NotRequired]
        public DateTime CreatedAt { get; set; }

        [NotRequired]
        public DateTime? StartedAt { get; set; }

        [NotRequired]
        public DateTime? PerformedAt { get; set; }

        [NotRequired]
        public DateTime? CompletedAt { get; set; }
    }

    public class CheckErrorDto
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }
}
