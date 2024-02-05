using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class NewCollectionStepDto
    {
        public Guid? Id { get; set; }

        public string XPath { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReviewNeeded { get; set; }

    }
}