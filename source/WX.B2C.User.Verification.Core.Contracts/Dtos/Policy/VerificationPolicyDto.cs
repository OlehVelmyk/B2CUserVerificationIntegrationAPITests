using System;
using WX.B2C.User.Verification.Core.Contracts.Conditions;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Policy
{
    /// <summary>
    /// TODO Open questions:
    /// Do we need domain model for policy?
    /// How to create tasks checks etc. Maybe we need to have id's in checks and required data to detect which one is different
    /// Or we can make it comparable
    /// Do we need to add collection of Required data and checks for easier creating entities for application
    /// </summary>
    public class VerificationPolicyDto
    {
        /// <summary>
        /// Maybe we need to store on which policy application was created
        /// </summary>
        public Guid Id { get; set; }

        public TaskVariantDto[] Tasks { get; set; }

        public RejectionPolicy RejectionPolicy { get; set; }
    }

    public class RejectionPolicy
    {
        public Condition[] Conditions { get; set; }
    }
}