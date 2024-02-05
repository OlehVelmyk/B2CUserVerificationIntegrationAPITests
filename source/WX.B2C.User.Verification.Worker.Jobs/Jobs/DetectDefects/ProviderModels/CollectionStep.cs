using System;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels
{
    internal class CollectionStep
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public DateTime CreatedAt { get; set; }

        public CollectionStepState State { get; set; }

        public CollectionStepReviewResult? ReviewResult { get; set; }

        public Guid? TaskId { get; set; }
    }
}