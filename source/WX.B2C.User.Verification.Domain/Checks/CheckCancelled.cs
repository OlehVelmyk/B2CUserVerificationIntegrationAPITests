using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class CheckCancelled : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid VariantId { get; private set; }

        public Guid UserId { get; private set; }

        public CheckType Type { get; private set; }

        public static CheckCancelled Create(Check check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return new CheckCancelled
            {
                Id = check.Id,
                VariantId = check.Variant.Id,
                UserId = check.UserId,
                Type = check.Type
            };
        }
    }
}