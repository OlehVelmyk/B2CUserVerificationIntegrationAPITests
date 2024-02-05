using System;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CollectionStepDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public CollectionStepState State { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }
        
        public CollectionStepReviewResult? ReviewResult { get; set; }
        
        public DateTime RequestedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}