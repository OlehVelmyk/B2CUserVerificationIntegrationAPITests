using System;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public class CollectionStepSpecimen
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

        public CollectionStepState State { get; set; }
    }
}
