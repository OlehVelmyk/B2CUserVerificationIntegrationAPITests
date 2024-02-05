using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Automation.Services
{
    public class PendingCheck
    {
        public Guid Id { get; private set; }

        public CheckProviderType Provider { get; private set; }

        public Guid VariantId { get; private set; }

        public CheckInputParameterDto[] Parameters { get; private set; }

        public bool ShouldBeAggregated => Provider == CheckProviderType.Onfido;

        public static PendingCheck Create(CheckDto check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return new PendingCheck
            {
                Id = check.Id,
                VariantId = check.Variant.Id,
                Provider = check.Variant.Provider
            };
        }

        public PendingCheck With(IEnumerable<CheckInputParameterDto> checkParameters)
        {
            Parameters = checkParameters?.ToArray() ?? throw new ArgumentNullException(nameof(checkParameters));
            return this;
        }
    }
}