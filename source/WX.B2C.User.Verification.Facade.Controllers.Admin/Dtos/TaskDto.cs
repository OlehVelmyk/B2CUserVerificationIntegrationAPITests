using System;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class TaskDto
    {
        public Guid Id { get; set; }

        public TaskType Type { get; set; }

        public TaskVariantDto Variant { get; set; }

        public int Priority { get; set; }

        public TaskState State { get; set; }

        [NotRequired]
        public TaskResult? Result { get; set; }

        public TaskCollectionStepDto[] CollectionSteps { get; set; }

        public TaskCheckDto[] Checks { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class TaskVariantDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class TaskCollectionStepDto
    {
        public Guid Id { get; set; }

        public CollectionStepVariantDto Variant { get; set; }

        public CollectionStepState State { get; set; }

        [NotRequired]
        public CollectionStepReviewResult? ReviewResult { get; set; }

        public object Data { get; set; }

        [NotRequired]
        public string FormattedData { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public DateTime RequestedAt { get; set; }

        [NotRequired]
        public DateTime? UpdatedAt { get; set; }
    }

    public class TaskCheckDto
    {
        public Guid Id { get; set; }

        public CheckType Type { get; set; }

        public CheckVariantDto Variant { get; set; }

        public CheckState State { get; set; }

        [NotRequired]
        public CheckResult? Result { get; set; }

        [NotRequired]
        public DateTime? CreatedAt { get; set; }

        [NotRequired]
        public DateTime? CompletedAt { get; set; }
    }
}