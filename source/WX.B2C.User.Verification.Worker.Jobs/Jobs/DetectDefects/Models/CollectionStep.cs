using System;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models
{
    internal class CollectionStep
    {
        public Guid Id { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewRequired { get; set; }

        public DateTime CreatedAt { get; set; }

        public CollectionStepState State { get; set; }

        public bool IsCompleted => State == CollectionStepState.Completed;

        public Guid[] RelatedTasks { get; set; } = Array.Empty<Guid>();

        public CollectionStepReviewResult? Result { get; set; }

        public override string ToString() =>
            $"{Id}:{XPath}";
    }
}