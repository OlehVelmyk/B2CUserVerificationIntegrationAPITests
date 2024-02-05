using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class CheckCompleted : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid VariantId { get; private set; }

        public CheckType Type { get; private set; }

        public CheckProviderType Provider { get; private set; }

        public CheckResult Result { get; private set; }

        public string Decision { get; private set; }

        public static CheckCompleted Create(Check check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return new CheckCompleted
            {
                Id = check.Id,
                VariantId = check.Variant.Id,
                UserId = check.UserId,
                Type = check.Type,
                Provider = check.Variant.Provider,
                Result = check.ProcessingResult.Result,
                Decision = check.ProcessingResult.Decision
            };
        }
    }
}