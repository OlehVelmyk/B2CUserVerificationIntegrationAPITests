using System;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class CollectionStep : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public CollectionStepState State { get; set; }
        
        public CollectionStepReviewResult? ReviewResult { get; set; }
    }
}