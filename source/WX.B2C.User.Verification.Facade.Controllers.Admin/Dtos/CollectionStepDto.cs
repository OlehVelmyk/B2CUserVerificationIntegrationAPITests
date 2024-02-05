using System;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class CollectionStepDto
    {
        public Guid Id { get; set; }

        public CollectionStepVariantDto Variant { get; set; }

        public CollectionStepState State { get; set; }

        [NotRequired]
        public CollectionStepReviewResult? ReviewResult { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public object Data { get; set; }

        public Guid[] RelatedTasks { get; set; }

        public DateTime RequestedAt { get; set; }

        [NotRequired]
        public DateTime? UpdatedAt { get; set; }
    }
}