using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class VerificationPolicy
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public RegionType RegionType { get; set; }

        public string Region { get; set; }

        public Task[] Tasks { get; set; }
        
        public Task[] Templates { get; set; }

        public Trigger[] Triggers { get; set; }

        public RejectionPolicy RejectionPolicy { get; set; }

        /// <summary>
        /// All possible check variants for validation.
        /// </summary>
        public CheckVariant[] PossibleCheckVariants { get; private set; }

        /// <summary>
        /// Define all possible check variants for validation.
        /// </summary>
        /// <param name="checkVariants"></param>
        public void DefineCheckVariants(CheckVariant[] checkVariants) =>
            PossibleCheckVariants = checkVariants;
    }
}