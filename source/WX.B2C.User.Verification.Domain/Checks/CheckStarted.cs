using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class CheckStarted : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid VariantId { get; private set; }

        public CheckExecutionContext ExecutionContext { get; private set; }

        public static CheckStarted Create(Check check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return new CheckStarted
            {
                Id = check.Id,
                UserId = check.UserId,
                VariantId = check.Variant.Id,
                ExecutionContext = check.ExecutionContext
            };
        }
    }
}