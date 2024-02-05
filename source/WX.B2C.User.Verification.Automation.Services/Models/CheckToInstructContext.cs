using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class CheckToInstructContext
    {
        public AcceptanceCheck Check { get; private set; }

        public CheckVariantInfo Variant { get; private set; }

        public CheckDto[] PreviousChecks { get; private set; }

        public static CheckToInstructContext Create(AcceptanceCheck check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return new CheckToInstructContext { Check = check };
        }

        public CheckToInstructContext With(CheckVariantInfo checkVariant)
        {
            Variant = checkVariant ?? throw new ArgumentNullException(nameof(checkVariant));
            return this;
        }

        public CheckToInstructContext With(IEnumerable<CheckDto> previousChecks)
        {
            PreviousChecks = previousChecks?.ToArray() ?? throw new ArgumentNullException(nameof(previousChecks));
            return this;
        }
    }
}